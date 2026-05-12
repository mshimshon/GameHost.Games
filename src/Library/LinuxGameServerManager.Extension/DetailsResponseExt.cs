using GameHost.Games.Lib.Installation.Contracts.Responses.Status;
using GameHost.Games.Lib.Installation.Contracts.Responses.Status.Enums;
using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response.Enums;
using System.Text.RegularExpressions;

namespace LinuxGameServerManager.Extension;

public static class DetailsResponseExt
{

    private static void FilterForStatus(DetailsResponse detail, ServerStatusResponse serverStatus)
    {
        if (detail.DetailType != DetailType.Status) return;
        StringComparison comparer = StringComparison.OrdinalIgnoreCase;
        bool isStarted = detail.Raw.Contains("started", comparer) || detail.Raw.Contains("online", comparer);
        bool isStopped = detail.Raw.Contains("stopped", comparer) || detail.Raw.Contains("offline", comparer);
        if (isStarted) serverStatus.Status = ServerStatus.Started;
        else if (isStopped) serverStatus.Status = ServerStatus.Stopped;
    }
    private const string PROTOCOL_REGEX_DETECTION = @"\b(?:(udp|tcp))(?:.*\b(udp|tcp))?";
    private const string PORT_REGEX_DETECTION = @"(\d+)";
    private static void FilterForPort(DetailsResponse detail, ServerStatusResponse serverStatus)
    {
        if (detail.DetailType != DetailType.Port) return;
        var matches = Regex.Matches(detail.Raw, @"([^\s]+)");

        var matchesList = matches.ToList();
        string title = matches.First().Value;
        matchesList.RemoveAt(0);
        string? protocol = default;
        string? port = default;
        foreach (Match m in matchesList)
        {

            if (protocol != default && port != default) break;
            if (protocol == default && Regex.IsMatch(m.Value, PROTOCOL_REGEX_DETECTION))
                protocol = m.Value;
            else if (port == default && Regex.IsMatch(m.Value, PORT_REGEX_DETECTION))
                port = m.Value;
        }
        if (protocol == default || port == default) return;
        PortInfoResponse portInfo = new()
        {
            Name = title,
            Protocol = protocol,
            Port = port
        };
        if (serverStatus.ConnectionInfo == default)
            serverStatus.ConnectionInfo = new()
            {
                PortInfoResponses = new() { portInfo }
            };
        else
            serverStatus.ConnectionInfo.PortInfoResponses.Add(portInfo);
    }

    private static void FilterForIP(DetailsResponse detail, ServerStatusResponse serverStatus)
    {
        if (detail.DetailType != DetailType.PublicIp || !detail.HasKeyPair) return;
        if (serverStatus.ConnectionInfo == default)
            serverStatus.ConnectionInfo = new() { Address = detail.Value! };
        else
            serverStatus.ConnectionInfo.Address = detail.Value!;
    }
    public static ServerStatusResponse ConvertToServerStatus(this IReadOnlyCollection<DetailsResponse> details)
    {
        ServerStatusResponse result = new()
        {
            Status = ServerStatus.Unknown
        };

        foreach (var detail in details)
        {
            //Console.WriteLine(detail);
            FilterForStatus(detail, result);
            FilterForIP(detail, result);
            FilterForPort(detail, result);
        }
        return result;
    }

}
