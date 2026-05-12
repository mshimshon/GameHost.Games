using GameHost.Games.Lib.Installation.Contracts.Responses.GameInfo.StartupParameters;
using GameHost.Games.Lib.Installation.Providers;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Contracts.Responses.GameInfo;

public sealed class GameStartupParameterResponse
{
    public string Key { get; init; } = default!;
    public string Type { get; init; } = default!;
    public string Label { get; init; } = default!;
    public string Description { get; init; } = default!;
    public bool Required { get; init; }
    public bool Editable { get; init; }

    [JsonConverter(typeof(JsonAlwaysStringConverter))]
    public string? DefaultValue { get; init; }
    public ValidationResponse? Validation { get; init; }
    public RelatedToResponse? RelatedTo { get; init; }
    public string Category { get; init; } = default!;
    public string? Warning { get; init; }
}
