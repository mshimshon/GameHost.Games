namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class UpdateGameFailedException : Exception
{
    public UpdateGameFailedException(string serverName) : base($"Update of {serverName} failed.")
    {
    }
}
