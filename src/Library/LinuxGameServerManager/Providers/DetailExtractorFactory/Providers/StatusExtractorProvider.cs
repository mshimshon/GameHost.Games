using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response.Enums;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtractorFactory.Providers;

internal class StatusExtractorProvider : IDetailExtractorProvider<StatusExtractorProvider>
{
    public DetailsResponse? Process(string line)
    {
        StringComparison comparer = StringComparison.OrdinalIgnoreCase;
        bool detected = line.StartsWith("Status:", comparer);
        if (!detected) return default;
        return new DetailsResponse(line, DetailType.Status);
    }
}
