namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class UpdateSoftwareFailedException : Exception
{
    public UpdateSoftwareFailedException(string stdErr) : base("Couldn't update LGSM software.")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
