namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class DependenciesInstallationFailedException : Exception
{
    public DependenciesInstallationFailedException(string stdOut, string stdErr, string serverName, string[] deps) :
        base($"Couldn't install one or more of the following dependencies for {serverName}: {string.Join(", ", deps)}.")
    {
        StdOut = stdOut;
        StdErr = stdErr;
        ServerName = serverName;
        Deps = deps;
    }

    public string StdOut { get; }
    public string StdErr { get; }
    public string ServerName { get; }
    public string[] Deps { get; }
}
