using GameHost.Games.Lib.LinuxGameServerManager.Entities.Enums;
using GameHost.Games.Lib.LinuxGameServerManager.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using System.Text.RegularExpressions;

namespace GameHost.Games.Lib.LinuxGameServerManager.Engine;

internal class ServerInstallService : IServerInstallService
{
    private readonly ILinuxCommand _linuxCommand;
    private readonly ICrazyReport _crazyReport;

    public ServerInstallService(ILinuxCommand linuxCommand, ICrazyReport<ServerInstallService> crazyReport)
    {
        _linuxCommand = linuxCommand;
        _crazyReport = crazyReport;
    }
    private Dictionary<ProblemType, string?> ScanConsoleForIssues(string[] output, params ProblemType[] scanFor)
    {
        string failCode = "FAIL";
        string missingCode = "missing";
        string dependencyCode = "dependencies";
        string warningCode = "Warning!";
        var result = new Dictionary<ProblemType, string?>();
        for (int i = output.ToArray().Length - 1; i >= 0; i--)
        {
            bool scanForCritialFail = scanFor.Contains(ProblemType.CritialFailure);
            bool foundCriticalFail = result.ContainsKey(ProblemType.CritialFailure);
            bool scanForMissingDeps = scanFor.Contains(ProblemType.MissingDependency);
            bool foundForMissingDeps = result.ContainsKey(ProblemType.MissingDependency);

            string line = output[i];
            bool inspectCriticalFail = !foundCriticalFail && scanForCritialFail && line.Contains(failCode, StringComparison.OrdinalIgnoreCase);
            bool inspectMissingDeps = !foundForMissingDeps && scanForMissingDeps &&
                line.Contains(missingCode, StringComparison.OrdinalIgnoreCase) &&
                line.Contains(dependencyCode, StringComparison.OrdinalIgnoreCase) &&
                line.Contains(warningCode, StringComparison.OrdinalIgnoreCase);

            bool shouldClean = inspectCriticalFail || inspectMissingDeps;
            if (shouldClean)
            {
                line = Regex.Replace(line, @"\x1B\[[0-9;]*[A-Za-z]", "");
            }

            if (inspectCriticalFail)
            {
                string lineNoSpace = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
                bool foundCriticalFailure = lineNoSpace.StartsWith($"[{failCode}]", StringComparison.OrdinalIgnoreCase);

                if (foundCriticalFailure)
                    result.Add(ProblemType.CritialFailure, default);
            }


            if (inspectMissingDeps)
            {
                string lineNoSpace = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
                bool confirmedDepList = lineNoSpace.StartsWith($"{warningCode}{missingCode}{dependencyCode}:", StringComparison.OrdinalIgnoreCase);
                if (confirmedDepList)
                {
                    var depSplit = line.Split(':').ToList();
                    depSplit.RemoveAt(0);
                    var deps = string.Join(':', depSplit);
                    result.Add(ProblemType.MissingDependency, deps);

                }
            }


            if ((!scanForMissingDeps || foundForMissingDeps) &&
                (!scanForCritialFail || foundCriticalFail)) break;
        }
        return result;
    }
    public async Task InstallDependenciesAsync(string serverName, string[] deps, CancellationToken ct = default)
    {
        var resultDep = await _linuxCommand
           .BuildCommand("dpkg --add-architecture i386")
           .AndCommand("apt-get update")
           .AndCommand($"apt-get install -y {string.Join(' ', deps)}")
            .SetCrazyReport(_crazyReport)
           .ExecAsync(ct);
        if (resultDep.Failed)
            throw new DependenciesInstallationFailedException(resultDep.StandardOutput, resultDep.StandardError, serverName, deps);
    }

    private const string STEAM_DOWNLOAD_REGEX = @"\[\s*(\d+%|----)\s*\]\s*Downloading\s*update\s*\(\s*(.*?)\s*\)";
    private const string STEAM_GAME_DOWNLOAD_REGEX = @"Update\s+state\s+\(.*?\)\s+.*?,\s+progress:\s+([\d.]+)\s*\(\s*(.*?)\s*\)";


    private async Task FilterKeyProgressReport(string line, Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default)
    {
        StringComparison comparer = StringComparison.OrdinalIgnoreCase;
        var matchDownloadSteam = Regex.Match(line, STEAM_DOWNLOAD_REGEX, RegexOptions.IgnoreCase);
        var matchGameDownloadSteam = Regex.Match(line, STEAM_GAME_DOWNLOAD_REGEX, RegexOptions.IgnoreCase);
        if (matchDownloadSteam.Success)
            await updateProgressStatus($"Downloading SteamCMD {matchDownloadSteam.Groups[1].Value}...", ct);
        else if (matchGameDownloadSteam.Success)
            await updateProgressStatus($"Downloading Game Server {matchGameDownloadSteam.Groups[1].Value}%...", ct);
        else if (line.Contains("LinuxGSM_", comparer))
            await updateProgressStatus("Install Linux GSM, Thanks Daniel Gibbs...", ct);
        else if (line.Contains("Dependencies", comparer) && line.Contains("Checking", comparer))
            await updateProgressStatus("Validating Dependencies...", ct);

    }

    public async Task InstallGameServerAsync(string serverName, Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default)
    {
        var result = await _linuxCommand
                .BuildCommand($"cd \"{BaseInfo.HOME}\"")
                .AndCommand($"{BaseInfo.BASH_PATH} ./linuxgsm.sh \"{serverName}\"")
                .AndCommand($"{BaseInfo.BASH_PATH} \"./{serverName}\" auto-install")
                .AutoCleanConsoleStream()
                .ReadConsoleOutput((line) => FilterKeyProgressReport(line, updateProgressStatus, ct))
                .AsUser(BaseInfo.USERNAME)
            .SetCrazyReport(_crazyReport)
                .ExecAsync(ct);

        var scanResult = ScanConsoleForIssues(result.RawStandardOutput.ToArray(), ProblemType.MissingDependency, ProblemType.CritialFailure);
        if (scanResult.ContainsKey(ProblemType.MissingDependency))
        {
            var deps = scanResult[ProblemType.MissingDependency]!;
            await InstallDependenciesAsync(serverName, deps.Split(' '), ct);
        }

        if (result.Failed || scanResult.ContainsKey(ProblemType.CritialFailure))
            throw new DownloadHasFailedException(result.StandardOutput, $"Couldn't install the game server for {serverName}");

    }
    public async Task UpdateGameServerAsync(string serverName, CancellationToken ct = default)
    {
        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" force-update")
            .AsUser(BaseInfo.USERNAME)
            .SetCrazyReport(_crazyReport)
            .ExecAsync(ct);
        var scanResult = ScanConsoleForIssues(result.RawStandardOutput.ToArray(), ProblemType.MissingDependency, ProblemType.CritialFailure);
        if (result.Failed || scanResult.ContainsKey(ProblemType.CritialFailure))
            throw new UpdateGameFailedException(serverName);
    }
    public async Task ValidateGameServerAsync(string serverName, CancellationToken ct = default)
    {
        var result = await _linuxCommand
                        .BuildCommand($"cd \"{BaseInfo.HOME}\"")
                        .AndCommand($"./\"{serverName}\" validate")
                        .AsUser(BaseInfo.USERNAME)
            .SetCrazyReport(_crazyReport)
                        .ExecAsync(ct);
        var scanResult = ScanConsoleForIssues(result.RawStandardOutput.ToArray(), ProblemType.CritialFailure);
        if (result.Failed || scanResult.ContainsKey(ProblemType.CritialFailure))
            throw new GameValidationFailedException(serverName);
    }

    public async Task DownloadSoftwareAsync(string serverName, CancellationToken ct = default)
    {
        await InstallDependenciesAsync("linuxgsm", BaseInfo.Dependencies, ct);
        var result = await _linuxCommand
                .BuildCommand($"cd \"{BaseInfo.HOME}\"")
                .AndCommand("curl -Lo linuxgsm.sh https://linuxgsm.sh")
                .AndCommand("chmod +x linuxgsm.sh")
            .SetCrazyReport(_crazyReport)
                .ExecAsync(ct);
        if (result.Failed) throw new DownloadHasFailedException(result.StandardError, $"Couldn't install the LGSM scripts.");
    }

    public async Task UpdateSoftwareAsync(string serverName, CancellationToken ct = default)
    {
        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" update-lgsm")
            .AsUser(BaseInfo.USERNAME)
            .SetCrazyReport(_crazyReport)
            .ExecAsync(ct);
        if (result.Failed)
            throw new UpdateSoftwareFailedException(result.StandardError);
    }

    public async Task<IEnumerable<string>> CheckUpdateGameServerAsync(string serverName, CancellationToken ct = default)
    {

        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" check-update")
            .AsUser(BaseInfo.USERNAME)
            .SetCrazyReport(_crazyReport)
            .ExecAsync(ct);
        var scanResult = ScanConsoleForIssues(result.RawStandardOutput.ToArray(), ProblemType.CritialFailure);
        if (result.Failed || scanResult.ContainsKey(ProblemType.CritialFailure))
            throw new CheckUpdateFailedException(result.StandardOutput, result.StandardError, serverName);
        return result.RawStandardOutput;
    }
}
