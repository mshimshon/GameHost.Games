namespace GameHost.Games.Lib.Installation.Exceptions;

public class InstallDependenciesFailedException : Exception
{
    public InstallDependenciesFailedException(string stdErr) : base("Installation of dependencies failed.")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
