using GameHost.Games.Lib.LinuxGameServerManager.Payloads.Response;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;

internal interface IDetailExtractorProvider<T> : IDetailExtractorProvider
{

}
internal interface IDetailExtractorProvider
{
    public DetailsResponse? Process(string line);
}
