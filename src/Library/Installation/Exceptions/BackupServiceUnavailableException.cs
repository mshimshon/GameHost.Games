namespace GameHost.Games.Lib.Installation.Exceptions;

public class BackupServiceUnavailableException : Exception
{
    public BackupServiceUnavailableException() : base("Backup service not available.")
    {
    }
}
