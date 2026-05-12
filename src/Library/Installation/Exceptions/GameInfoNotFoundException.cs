namespace GameHost.Games.Lib.Installation.Exceptions;

public class GameInfoNotFoundException : Exception
{
    public GameInfoNotFoundException() : base("game_info.json was not found.")
    {
    }
}
