using GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response.Enums;

namespace GameHost.Games.Lib.LinuxGameServerManager.Contracts.Response;
/*
Distro:    Debian 7.7
Arch:      x86_64
Kernel:    2.6.32-042stab102.9
Hostname:  vps86887.ovh.net
tmux:      tmux 1.6
GLIBC:     2.13
Uptime:    0d, 17h, 36m
Avg Load:  0.00, 0.02, 0.00
Mem:       total   used   free
Physical:  1.0G    1.0G   17M
Swap:      128M    25M    102M
Disk available:  5.4G
Serverfiles:     1.6G
Backups:         4.8M
Server name:  Fistful of Frags Server
Server IP:    192.168.1.1:27015
RCON password:  rconpassword
Status:       OFFLINE
Service name:  fof-server
User:          fofserver
Location:      /home/fofserver
Config file:   /home/fofserver/serverfiles/fof/cfg/fof-server.cfg
No. of backups:    1
DESCRIPTION  DIRECTION  PORT   PROTOCOL
> Game/RCON  INBOUND    27015  tcp/udp
 */
public sealed record DetailsResponse
{
    public DetailsResponse(string raw, DetailType detailType = DetailType.Unknown)
    {
        Raw = raw;
        DetailType = detailType;
    }
    public DetailsResponse(string raw, string key, string value, DetailType detailType = DetailType.Unknown)
    {
        Raw = raw;
        Key = key;
        Value = value;
        DetailType = detailType;
    }

    public string Raw { get; }
    public string? Key { get; }
    public string? Value { get; }
    public bool HasKeyPair => Key != default && Value != default;
    public DetailType DetailType { get; }
}
