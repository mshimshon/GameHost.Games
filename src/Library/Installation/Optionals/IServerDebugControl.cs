namespace GameHost.Games.Lib.Installation.Optionals;

public interface IServerDebugControl
{
    Task<bool> HasDebugSupportAsync(CancellationToken ct = default);

    Task<IEnumerable<string>> GetLogFiles(CancellationToken ct = default);
}
