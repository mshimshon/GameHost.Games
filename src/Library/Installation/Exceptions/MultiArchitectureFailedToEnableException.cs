namespace GameHost.Games.Lib.Installation.Exceptions;

public class MultiArchitectureFailedToEnableException : Exception
{
    public MultiArchitectureFailedToEnableException(string stdOut, string stdErr) : base("Failed to enable multi architecture required for some depenendies.")
    {
        StdOut = stdOut;
        StdErr = stdErr;
    }

    public string StdOut { get; }
    public string StdErr { get; }
}
