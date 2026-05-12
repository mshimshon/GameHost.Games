using Microsoft.Extensions.DependencyInjection;

namespace GameHost.Games.Lib.Installation;

public sealed class ConsoleApplicationBuilder
{
    private readonly string[] _args;

    public IServiceCollection Services { get; }
    public ConsoleApplicationBuilder(string[] args)
    {
        Services = new ServiceCollection();
        Services.AddInstallationServices();
        _args = args;
    }

    public ConsoleApplicationRuntime Build()
    {
        var sp = Services.BuildServiceProvider();
        return new ConsoleApplicationRuntime()
        {
            ServiceProvider = sp.CreateScope().ServiceProvider,
            Arguments = _args
        };
    }
}
