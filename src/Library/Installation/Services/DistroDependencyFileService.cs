using GameHost.Games.Lib.Installation.Exceptions;
using GameHost.Games.Lib.Installation.Payloads.Responses;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using System.Formats.Tar;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Services;

internal class DistroDependencyFileService : IDistroDependencyFileService
{
    private readonly ILinuxCommand _linuxCommand;
    private readonly IPluginUserLocation _pluginUserLocation;
    private readonly ICrazyReport<DistroDependencyFileService> _crazyReport;
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        ReadCommentHandling = JsonCommentHandling.Skip
    };
#if DEBUG
    private readonly bool _debug = true;

#else
    private readonly bool _debug = false;
#endif

    public DistroDependencyFileService(ILinuxCommand linuxCommand, IPluginLocation pluginLocation, ICrazyReport<DistroDependencyFileService> crazyReport, ISafeFileWriter safeFileWriter)
    {
        _linuxCommand = linuxCommand;
        _pluginUserLocation = pluginLocation;
        _crazyReport = crazyReport;
        _safeFileWriter = safeFileWriter;
        _pluginUserLocation.SetUsername(BaseInfo.USERNAME);
    }

    public async Task<DistroInformationResponse> GetDistroAsync(CancellationToken ct = default)
    {
        var result = await _linuxCommand
            .BuildCommand("cat /etc/os-release")
            .PatchInStdOutAsPayload()
            .SetCrazyReport(_crazyReport)
            .ExecPayloadAsync<string>();
        string[] lines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        string? id = default, codename = default, version = default;
        foreach (var item in lines)
        {
            if (!item.Contains('=')) continue;
            string[] keypair = item.Trim().Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (keypair.Length < 2) continue;
            string key = keypair[0].Trim();
            string value = keypair[1].Replace("\"", string.Empty).Replace("\'", string.Empty).Trim();
            if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                id = value.ToLower();
            else if (key.Equals("version_id", StringComparison.OrdinalIgnoreCase))
                version = value;
            else if (key.Equals("version_codename", StringComparison.OrdinalIgnoreCase))
                codename = value;
            if (id != default && codename != default && version != default)
                break;
        }
        if (id == default)
            throw new DistroExtractionFailedException(result, "Distro name could not be extracted.");
        else if (codename == default)
            throw new DistroExtractionFailedException(result, "Distro codename could not be extracted.");
        else if (version == default)
            throw new DistroExtractionFailedException(result, "Distro version could not be extracted.");

        var dto = new DistroInformationResponse()
        {
            Id = id,
            VersionCodename = codename,
            VersionId = version
        };
        _crazyReport.Report("Using Distro {0}", dto);

        return dto;
    }
    private async Task<string> ReadFromTarGzAsync(
        string tarGzPath, string file,
        CancellationToken ct = default)
    {
        await using FileStream fs = new(tarGzPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using GZipStream gz = new(fs, CompressionMode.Decompress, leaveOpen: false);
        using TarReader tar = new(gz);

        TarEntry? entry;
        while ((entry = await tar.GetNextEntryAsync(false, ct)) != null)
        {
            if (!string.Equals(entry.Name, file, StringComparison.OrdinalIgnoreCase))
                continue;

            if (entry.DataStream == null)
                throw new DistroDependencyDownloadFailedException("", "", $"{file} not found in tar.gz");

            using MemoryStream ms = new();
            await entry.DataStream.CopyToAsync(ms, ct);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        throw new DistroDependencyDownloadFailedException("", "", $"{file} not found in tar.gz");
    }
    public async Task DownloadOfficialDistroDependencyFile(CancellationToken ct = default)
    {
        _crazyReport.ReportInfo("Downloading Distro Dependency File...");
        var distroInfo = await GetDistroAsync(ct);
        var depFilename = string.Format(BaseInfo.DISTRO_DEP_FILENAME_FORMAT, distroInfo.Id, distroInfo.VersionId);
        string targetLocation = _pluginUserLocation.GetUserDownloadFor(BaseInfo.PLUGIN_MODULE_NAME, BaseInfo.DISTRO_DEP_FILENAME);
        if (_debug)
        {
            var depCommonFilename = string.Format("dep_{0}_{1}_common.json", distroInfo.Id, distroInfo.VersionId);
            //dep_debian_13_common
            _crazyReport.ReportWarning("Debug Build, Using Mockup Location.");
            string depCommonFile = _pluginUserLocation.GetUserDownloadFor(BaseInfo.PLUGIN_MODULE_NAME, [BaseInfo.MOCK_FOLDER], depCommonFilename);
            if (!File.Exists(depCommonFile))
                throw new DistroDependencyDownloadFailedException("", "", "Failed to get dev common dependency file (File not found).");
            List<string>? common = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(depCommonFile), _jsonSerializerOptions);
            if (common == default)
                throw new DistroDependencyDownloadFailedException("", "", "Cannot read common dep dev file.");
            string installerLocation = _pluginUserLocation.GetUserDownloadBase(BaseInfo.PLUGIN_MODULE_NAME, [BaseInfo.MOCK_FOLDER, "installers"]);
            DistroDependencyFileResponse dependencyFile = new()
            {
                Common = common,
                Specific = new()

            };
            foreach (var item in Directory.GetFiles(installerLocation, "*.tar.gz", SearchOption.TopDirectoryOnly))
            {
                var resultManifest = await ReadFromTarGzAsync(item, "config/manifest.json", ct);
                var manifestLoaded = JsonSerializer.Deserialize<ManifestResponse>(resultManifest, _jsonSerializerOptions);
                if (manifestLoaded == default)
                    throw new DistroDependencyDownloadFailedException("", "", "Failed to extract manifest from archive.");
                var resultDependency = await ReadFromTarGzAsync(item, "config/dependencies.json", ct);
                var dependencyListLoaded = JsonSerializer.Deserialize<List<InstallerDependencyFileResponse>>(resultDependency, _jsonSerializerOptions);
                var dependencyLoaded = dependencyListLoaded?
                     .Where(p => string.Equals(p.OperatingSystem, distroInfo.Id, StringComparison.OrdinalIgnoreCase))
                     .Where(p => string.Equals(p.OperatingSystemVersion, distroInfo.VersionId, StringComparison.OrdinalIgnoreCase))
                     .FirstOrDefault();
                if (dependencyLoaded == default)
                    throw new DistroDependencyDownloadFailedException("", "", "Failed to extract manifest from archive.");

                dependencyFile.Specific[manifestLoaded.Id] = dependencyLoaded.Dependencies.ToArray();
            }

            string mockDepFile = JsonSerializer.Serialize(dependencyFile, _jsonSerializerOptions);
            _crazyReport.ReportInfo("Creating Mock Dep File {1}", targetLocation);
            await _safeFileWriter.WriteThenCopyFileAsync(targetLocation, mockDepFile, ct);
            if (!File.Exists(targetLocation))
                throw new DistroDependencyDownloadFailedException("", "", "Failed to fetch distro dependency file or the OS is not supported.");
            _crazyReport.ReportSuccess("Copied! ({0})", targetLocation);
        }

    }

    private DistroDependencyFileResponse ReadDependencyFile(CancellationToken ct = default)
    {
        string targetLocation = _pluginUserLocation.GetUserDownloadFor(BaseInfo.PLUGIN_MODULE_NAME, BaseInfo.DISTRO_DEP_FILENAME);
        _crazyReport.ReportInfo("Reading Distro Dependency File {0}", targetLocation);

        string json = File.ReadAllText(targetLocation);
        try
        {
            _crazyReport.Report(json);
            var dependencyInfo = JsonSerializer.Deserialize<DistroDependencyFileResponse>(json, _jsonSerializerOptions);
            if (dependencyInfo == default)
                throw new NullReferenceException("Dependency Info Cannot be null.");
            _crazyReport.ReportSuccess("Distro Dependency File Successfully Parsed!");
            return dependencyInfo;
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new DistroDependencyFileInvalidException(json);
        }

    }

    public async Task InstallDependenciesAsync(string gameName, Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default)
    {
        await updateProgressStatus("Installing Dependencies...", ct);
        var dependencyInfo = ReadDependencyFile(ct);
        _crazyReport.ReportInfo("Extracting Dependencies to Install");
        var enableMultiArchitectureResult = await _linuxCommand
            .BuildCommand($"dpkg --add-architecture i386")
                .AndCommand("apt-get update")
            .SetCrazyReport(_crazyReport)
            .ExecAsync(ct);
        if (enableMultiArchitectureResult.Failed)
            throw new MultiArchitectureFailedToEnableException(enableMultiArchitectureResult.StandardOutput, enableMultiArchitectureResult.StandardError);

        if (dependencyInfo.Common.Count > 0)
        {
            var commonDependencies = string.Join(' ', dependencyInfo.Common);

            var commandInstallCommonResult = await _linuxCommand
                .BuildCommand($"apt-get install -y {commonDependencies}")
            .SetCrazyReport(_crazyReport)
                .ExecAsync(ct);
            if (commandInstallCommonResult.Failed)
                throw new DistroDependencyInstallationFailedException(commandInstallCommonResult.StandardOutput,
                    commandInstallCommonResult.StandardError, "Common Distro Dependencies Failed to Install.");

        }

        if (dependencyInfo.Specific != default && dependencyInfo.Specific.ContainsKey(gameName))
        {
            var allDeps = dependencyInfo.Specific[gameName];
            var specificDependencies = string.Join(' ', allDeps);
            var commandInstallSpecificResult = await _linuxCommand
                .BuildCommand($"apt-get install -y {specificDependencies}")
            .SetCrazyReport(_crazyReport)
                .ExecAsync(ct);
            if (commandInstallSpecificResult.Failed)
                throw new DistroDependencyInstallationFailedException(commandInstallSpecificResult.StandardOutput,
                    commandInstallSpecificResult.StandardError, "Game-Specific Distro Dependencies Failed to Install.");
        }

    }

}
