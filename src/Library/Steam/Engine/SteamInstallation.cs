namespace GameHost.Games.Lib.Steam.Engine;

internal class SteamInstallation : ISteamInstallation
{
    public SteamInstallation()
    {

    }

    public Task<bool> CheckSteamInstallationAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallSteamAsync(CancellationToken ct = default) => throw new NotImplementedException();
}
