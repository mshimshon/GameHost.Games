namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class StartServerFailedException : Exception
{
    public StartServerFailedException(string stdErr) : base("Failed to Start the Server.")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
