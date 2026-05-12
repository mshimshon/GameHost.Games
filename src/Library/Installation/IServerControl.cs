using GameHost.Games.Lib.Installation.Contracts.Responses.Status;

namespace GameHost.Games.Lib.Installation;

public interface IServerControl
{
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync(CancellationToken ct = default);
    Task RestartAsync(CancellationToken ct = default);
    Task<ServerStatusResponse> StatusAsync(CancellationToken ct = default);
    Task ConsoleAsync(Func<string, Task> consoleStream, CancellationToken ct = default);
}
