namespace GameHost.Games.Lib.Steam;

public interface ISteamInstallation
{
    Task<bool> CheckSteamInstallationAsync(CancellationToken ct = default);
    Task InstallSteamAsync(CancellationToken ct = default);
}
