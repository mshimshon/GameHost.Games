namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class DetailsServerFailedException : Exception
{
    public DetailsServerFailedException(string stdErr) : base("Failed to get the server details.")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
