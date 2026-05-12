using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response.Enums;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory;
using System.Text.RegularExpressions;

namespace GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtractorFactory.Providers;

internal class ServerIpExtractorProvider : IDetailExtractorProvider<ServerIpExtractorProvider>
{
    private const string INTERNET_IP_REGEX = @"^\s*(Internet\s+IP):\s*(\d{1,3}(?:\.\d{1,3}){3}(?::\d+)?)(?:\s*)$";
    private const string LOCAL_IP_REGEX = @"^\s*(Server\s+IP):\s*(\d{1,3}(?:\.\d{1,3}){3}(?::\d+)?)(?:\s*)$";
    public DetailsResponse? Process(string line)
    {
        var internetIp = Regex.Match(line, INTERNET_IP_REGEX);
        var localIp = Regex.Match(line, LOCAL_IP_REGEX, RegexOptions.IgnoreCase);
        Console.WriteLine($"{line} {internetIp.Success} {localIp.Success}");
        if (!internetIp.Success && !localIp.Success) return default;
        Match match = internetIp.Success ? internetIp : localIp;
        DetailType detailType = internetIp.Success ? DetailType.PublicIp : DetailType.LocalIp;
        string key = match.Groups[1].Value;     // "Internet IP"
        string value = match.Groups[2].Value; // "5.2.214.234:16261"
        return new DetailsResponse(line, key, value, detailType);
    }
}
