namespace GameHost.Games.Lib.Installation.Exceptions;

public class DistroDependencyFileInvalidException : Exception
{
    public DistroDependencyFileInvalidException(string json) : base("Couldn't deserialize the dependency file.")
    {
        Json = json;
    }

    public string Json { get; }
}
