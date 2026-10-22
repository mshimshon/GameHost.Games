namespace GameHost.Games.Lib.Installation.Exceptions;

public class PermissionSetFailedException : Exception
{
    public PermissionSetFailedException() : base("Permission failed to set.")
    {
    }
}