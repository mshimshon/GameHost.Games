using GameHost.Games.Lib.LinuxGameServerManager.Payloads.Response;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;

internal interface IDetailExtratorFactory
{
    ICollection<DetailsResponse> ProcessLines(string[] lines);
}
