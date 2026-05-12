namespace GameHost.Games.Lib.Installation;

public sealed class ConsoleApplicationRuntime
{
    public IServiceProvider ServiceProvider { get; init; } = default!;
    public string[] Arguments { get; init; } = default!;

    public async Task RunAsync(CancellationToken ct = default)
    {
        await ServiceProvider.RunStartupCommandAsync(ct, Arguments);
    }
}
