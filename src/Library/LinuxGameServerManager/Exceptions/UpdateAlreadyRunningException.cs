namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class UpdateAlreadyRunningException : Exception
{

    public UpdateAlreadyRunningException() : base("The Update is already running or lock is stuck (/tmp/lgsm_install.lock).")
    {
    }
}
