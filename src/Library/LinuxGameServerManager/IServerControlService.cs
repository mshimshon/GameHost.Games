using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;

namespace GameHost.Games.Lib.LinuxGameServerManager;

public interface IServerControlService
{
    Task ConsoleAsync(string serverName, Func<string, Task> consoleStream, CancellationToken ct = default);
    Task<IReadOnlyCollection<DetailsResponse>> DetailsAsync(string serverName, bool hideSensitive = false, CancellationToken ct = default);
    Task StopAsync(string serverName, CancellationToken ct = default);
    Task StartAsync(string serverName, CancellationToken ct = default);
    Task RestartAsync(string serverName, CancellationToken ct = default);
}
