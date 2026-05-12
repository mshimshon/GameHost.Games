using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;

internal interface IDetailExtratorFactory
{
    ICollection<DetailsResponse> ProcessLines(string[] lines);
}
