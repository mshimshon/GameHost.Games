namespace GameHost.Games.Lib.LinuxGameServerManager.Exceptions;

public class GameValidationFailedException : Exception
{
    public GameValidationFailedException(string serverName) : base($"The game {serverName} failed to validate.")
    {
    }
}
