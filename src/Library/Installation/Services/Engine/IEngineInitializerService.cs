namespace GameHost.Games.Lib.Installation.Services.Engine;

internal interface IEngineInitializerService
{
    Task InitializeAsync(CancellationToken ct = default);
}
