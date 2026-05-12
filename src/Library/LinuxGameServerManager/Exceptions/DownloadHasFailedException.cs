namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class DownloadHasFailedException : Exception
{
    public DownloadHasFailedException(string stdErr, string message) : base(message)
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
