namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class RestartServerFailedException : Exception
{

    public RestartServerFailedException(string stdErr) : base("Failed to Restart the Server.")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
