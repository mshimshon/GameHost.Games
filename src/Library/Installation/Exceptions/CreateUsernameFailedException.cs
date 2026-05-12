namespace GameHost.Games.Lib.Installation.Exceptions;

public class CreateUsernameFailedException : Exception
{
    public CreateUsernameFailedException(string stdErr) : base("Could not create the username lgsm")
    {
        StdErr = stdErr;
    }

    public string StdErr { get; }
}
