namespace GameHost.Games.Lib.Installation.Exceptions;

public class DebugServiceUnavailableException : Exception
{
    public DebugServiceUnavailableException() : base("Debug Service not available.")
    {
    }
}
