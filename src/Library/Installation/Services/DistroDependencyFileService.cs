using GameHost.Games.Lib.Installation.Contracts.Responses;
using GameHost.Games.Lib.Installation.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Services;

internal class DistroDependencyFileService : IDistroDependencyFileService
{
    private readonly ILinuxCommand _linuxCommand;
    private readonly IPluginUserLocation _pluginUserLocation;
    private readonly ICrazyReport<DistroDependencyFileService> _crazyReport;
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

    public DistroDependencyFileService(ILinuxCommand linuxCommand, IPluginLocation pluginLocation, ICrazyReport<DistroDependencyFileService> crazyReport)
    {
        _linuxCommand = linuxCommand;
        _pluginUserLocation = pluginLocation;
        _crazyReport = crazyReport;
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

    public async Task DownloadOfficialDistroDependencyFile(CancellationToken ct = default)
    {
        _crazyReport.ReportInfo("Downloading Distro Dependency File...");
        var distroInfo = await GetDistroAsync(ct);
        var depFilename = string.Format(BaseInfo.DISTRO_DEP_FILENAME_FORMAT, distroInfo.Id, distroInfo.VersionId);
        string targetLocation = _pluginUserLocation.GetUserDownloadFor(BaseInfo.PLUGIN_MODULE_NAME, BaseInfo.DISTRO_DEP_FILENAME);
        if (_debug)
        {
            _crazyReport.ReportWarning("Debug Build, Using Mockup Location.");

            string sourceLocation = _pluginUserLocation.GetUserDownloadFor(BaseInfo.PLUGIN_MODULE_NAME, [BaseInfo.MOCK_FOLDER], $"{depFilename}.dev.json");
            _crazyReport.ReportInfo("Copying File from {0} to {1}", sourceLocation, targetLocation);
            var depFileCommand = $"cp -f \"{sourceLocation}\" \"{targetLocation}\"";
            var depFileResult = await _linuxCommand.BuildCommand(depFileCommand).ExecAsync(ct);
            if (depFileResult.Failed || !File.Exists(targetLocation))
                throw new DistroDependencyDownloadFailedException(depFileResult.StandardOutput, depFileResult.StandardError, "Failed to fetch distro dependency file or the OS is not supported.");
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
            var dependencyInfo = JsonSerializer.Deserialize<DistroDependencyFileResponse>(json, _jsonSerializerOptions);
            if (dependencyInfo == default)
                throw new NullReferenceException("Dependency Info Cannot be null.");
            _crazyReport.ReportSuccess("Distro Dependency File Successfully Parsed!");

            return dependencyInfo;
        }
        catch (Exception)
        {
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
