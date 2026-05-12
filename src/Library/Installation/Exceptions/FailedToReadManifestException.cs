namespace GameHost.Games.Lib.Installation.Exceptions;

public class FailedToReadManifestException : Exception
{
    public FailedToReadManifestException() : base("Failed to read manifest.json")
    {
    }
}
