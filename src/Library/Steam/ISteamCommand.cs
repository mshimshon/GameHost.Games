namespace GameHost.Games.Lib.Steam;

public interface ISteamCommand
{
    Task<bool> CheckSteamInstallationAsync(CancellationToken ct = default);
    Task InstallSteamAsync(CancellationToken ct = default);

    Task LoginAsync(string username, string password, CancellationToken ct = default);
    Task SendSteamGuardAsync(string code, CancellationToken ct = default);
    Task InstallAppAsync(string appid, string directory, bool validate = true, CancellationToken ct = default);
    Task InstallAppAsync(string appid, string directory, string beta, bool validate = true, CancellationToken ct = default);
    Task InstallAppAsync(string appid, string directory, string beta, string betaPassword, bool validate = true, CancellationToken ct = default);
    Task UpdateAppAsync(string appid, CancellationToken ct = default);
}
