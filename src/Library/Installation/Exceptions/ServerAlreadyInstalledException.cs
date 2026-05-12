namespace GameHost.Games.Lib.Installation.Exceptions;

public class ServerAlreadyInstalledException : Exception
{
    public ServerAlreadyInstalledException() : base("A game server seems to be already installed.")
    {
    }
}
