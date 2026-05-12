using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtractorFactory;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtractorFactory.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory.Extensions;

internal static class DetailExtractorServiceExt
{
    private static DetailExtractorFactoryOptions _detailExtractorFactoryOptions = new();
    public static void AddDetailExtratorFactory(this IServiceCollection services)
    {
        services.AddTransient<IDetailExtratorFactory>(sp => new DetailExtractorFactory.Engine.DetailExtractorFactory(sp, _detailExtractorFactoryOptions));
        services.UseDetailExtractor<PortExtractorProvider>();
        services.UseDetailExtractor<ServerIpExtractorProvider>();
        services.UseDetailExtractor<StatusExtractorProvider>();
    }
    public static void UseDetailExtractor<T>(this IServiceCollection services) where T : class, IDetailExtractorProvider
    {
        Type serviceType = typeof(IDetailExtractorProvider<T>);
        services.AddScoped(serviceType, typeof(T));
        _detailExtractorFactoryOptions.Providers.Add(serviceType);
    }
}
