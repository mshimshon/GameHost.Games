namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class InstallAlreadyRunningException : Exception
{

    public InstallAlreadyRunningException() : base("Installation Process is already running or lock is stuck (/tmp/lgsm_install.lock).")
    {
    }
}
