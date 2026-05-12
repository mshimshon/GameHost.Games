using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response.Enums;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;
using System.Text.RegularExpressions;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtractorFactory.Providers;

internal class PortExtractorProvider : IDetailExtractorProvider<PortExtractorProvider>
{
    private const string PROTOCOL_REGEX_DETECTION = @"\b(?:(udp|tcp))(?:.*\b(udp|tcp))?";
    private const string PORT_REGEX_DETECTION = @"\d+(?:[-:]\d+)?(?:,\d+(?:[-:]\d+)?)*";
    public DetailsResponse? Process(string line)
    {
        bool port = Regex.IsMatch(line, PORT_REGEX_DETECTION, RegexOptions.IgnoreCase);
        bool proto = Regex.IsMatch(line, PROTOCOL_REGEX_DETECTION, RegexOptions.IgnoreCase);

        if (!port || !proto) return default;
        return new(line, port ? DetailType.Port : DetailType.Unknown);
    }
}
