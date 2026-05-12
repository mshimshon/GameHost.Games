namespace GameHost.Games.Lib.Installation.Exceptions;

public class FailedToReadGameInfoException : Exception
{
    public FailedToReadGameInfoException() : base("Failed to read game_info.json")
    {
    }
}
