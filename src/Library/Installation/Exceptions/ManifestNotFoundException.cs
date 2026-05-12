namespace GameHost.Games.Lib.Installation.Exceptions;

public class ManifestNotFoundException : Exception
{
    public ManifestNotFoundException() : base($"{BaseInfo.MANIFEST_FILENAME} was not found")
    {
    }
}
