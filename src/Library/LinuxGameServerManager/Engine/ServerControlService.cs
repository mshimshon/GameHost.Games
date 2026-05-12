using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Exceptions;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

namespace GameHost.Games.Lib.LinuxGameServerManager.Engine;

internal class ServerControlService : IServerControlService
{
    private readonly ILinuxCommand _linuxCommand;
    private readonly IDetailExtratorFactory _detailExtratorFactory;

    public ServerControlService(ILinuxCommand linuxCommand, IDetailExtratorFactory detailExtratorFactory)
    {
        _linuxCommand = linuxCommand;
        _detailExtratorFactory = detailExtratorFactory;
    }
    public Task ConsoleAsync(string serverName, Func<string, Task> consoleStream, CancellationToken ct = default) => throw new NotImplementedException();


    public async Task<IReadOnlyCollection<DetailsResponse>> DetailsAsync(string serverName, bool hideSensitive = false, CancellationToken ct = default)
    {
        string cmd = hideSensitive ? "postdetails" : "details";
        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" {cmd}")
            .AsUser(BaseInfo.USERNAME)
            .ExecAsync(ct);
        if (result.Failed)
            throw new DetailsServerFailedException(result.StandardError);
        var outputList = _detailExtratorFactory.ProcessLines(result.RawStandardOutput.ToArray());


        return outputList.ToArray().AsReadOnly();
    }

    public async Task RestartAsync(string serverName, CancellationToken ct = default)
    {
        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" restart")
            .AsUser(BaseInfo.USERNAME)
            .ExecAsync(ct);
        if (result.Failed)
            throw new RestartServerFailedException(result.StandardError);
    }
    public async Task StartAsync(string serverName, CancellationToken ct = default)
    {
        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" start")
            .AsUser(BaseInfo.USERNAME)
            .ExecAsync(ct);
        if (result.Failed)
            throw new StartServerFailedException(result.StandardError);
    }
    public async Task StopAsync(string serverName, CancellationToken ct = default)
    {
        var result = await _linuxCommand
            .BuildCommand($"cd \"{BaseInfo.HOME}\"")
            .AndCommand($"./\"{serverName}\" stop")
            .AsUser(BaseInfo.USERNAME)
            .ExecAsync(ct);
        if (result.Failed)
            throw new StopServerFailedException(result.StandardError);
    }
}
