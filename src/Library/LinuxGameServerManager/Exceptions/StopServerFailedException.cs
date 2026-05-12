namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class StopServerFailedException : Exception
{
    public StopServerFailedException(string stdErr) : base("Failed to stop the server.")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
