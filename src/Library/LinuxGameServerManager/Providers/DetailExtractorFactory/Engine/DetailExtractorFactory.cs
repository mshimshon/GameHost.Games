using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtractorFactory.Engine;

internal class DetailExtractorFactory : IDetailExtratorFactory
{
    private readonly DetailExtractorFactoryOptions _detailExtractorFactoryOptions;
    private ICollection<DetailsResponse> _extracted;
    private List<IDetailExtractorProvider> _resolvedProviders = new List<IDetailExtractorProvider>();

    public DetailExtractorFactory(IServiceProvider sp, DetailExtractorFactoryOptions detailExtractorFactoryOptions)
    {
        _extracted = new List<DetailsResponse>();
        _detailExtractorFactoryOptions = detailExtractorFactoryOptions;
        ResolvProviders(sp);

    }
    private void ResolvProviders(IServiceProvider sp)
    {
        foreach (var provider in _detailExtractorFactoryOptions.Providers)
        {
            _resolvedProviders.Add((IDetailExtractorProvider)sp.GetRequiredService(provider));
        }
    }


    public ICollection<DetailsResponse> ProcessLines(string[] lines)
    {
        foreach (var line in lines)
        {
            var cleanLine = line.CleanFromColorCodes();
            if (cleanLine.Contains("======")) continue;
            DetailsResponse? nextEntry = default;
            foreach (var provider in _resolvedProviders)
            {
                var result = provider.Process(cleanLine);
                if (result != default)
                {
                    nextEntry = result;
                    break;
                }
            }
            if (nextEntry == default)
                nextEntry = new(cleanLine, Contracts.Response.Enums.DetailType.Unknown);
            _extracted.Add(nextEntry);
        }
        return _extracted;
    }
}
