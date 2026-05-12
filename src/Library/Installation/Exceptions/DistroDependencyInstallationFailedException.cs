namespace GameHost.Games.Lib.Installation.Exceptions;

public class DistroDependencyInstallationFailedException : Exception
{

    public DistroDependencyInstallationFailedException(string stdOut, string stdErr, string message) : base(message)
    {
        StdOut = stdOut;
        StdErr = stdErr;
    }

    public string StdOut { get; }
    public string StdErr { get; }
}
