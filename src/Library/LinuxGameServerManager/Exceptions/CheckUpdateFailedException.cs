namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class CheckUpdateFailedException : Exception
{
    public CheckUpdateFailedException(string stdOut, string stdErr, string serverName) : base($"Failed to check if new update is available for {serverName}")
    {
        StdOut = stdOut;
        StdErr = stdErr;
        ServerName = serverName;
    }

    public string StdOut { get; }
    public string StdErr { get; }
    public string ServerName { get; }
}
