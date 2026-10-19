namespace GameHost.Games.Lib.Steam.Engine;

internal class SteamCommand : ISteamCommand
{
    private readonly string? _username = default;
    private readonly string? _password = default;
    public SteamCommand()
    {
        _username = Environment.GetEnvironmentVariable("STEAMCMD_USERNAME");
        _password = Environment.GetEnvironmentVariable("STEAMCMD_PASSWORD");


    }
    public Task DownloadWorkshopItemAsync(string appid, string workshopId, CancellationToken ct = default) => throw new NotImplementedException();
    public Task GetLicenseAppAsync(string appid, CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAppAsync(string appid, string directory, bool validate = true, CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAppAsync(string appid, string directory, string beta, bool validate = true, CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAppAsync(string appid, string directory, string beta, string betaPassword, bool validate = true, CancellationToken ct = default) => throw new NotImplementedException();
    public Task LoginAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task LoginAsync(string username, string password, CancellationToken ct = default) => throw new NotImplementedException();
    public Task SendSteamGuardAsync(string code, CancellationToken ct = default) => throw new NotImplementedException();
    public Task UpdateAppAsync(string appid, CancellationToken ct = default) => throw new NotImplementedException();
}
