namespace GameHost.Games.Lib.LinuxGameServerManager;

internal static class BaseInfo
{
    public const string USERNAME = "lgsm";
    public const string HOME = "/home/lgsm/";
    public const string INSTALL_LOCK_FILE = "/tmp/lgsm_install.lock";
    public const string BASH_PATH = "/bin/bash";
    public static string[] Dependencies => ["curl", "jq"];
}
