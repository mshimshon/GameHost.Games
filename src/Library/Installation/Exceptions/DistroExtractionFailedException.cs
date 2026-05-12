namespace GameHost.Games.Lib.Installation.Exceptions;

public class DistroExtractionFailedException : Exception
{

    public DistroExtractionFailedException(string stdOut, string message) : base(message)
    {
    }
}
