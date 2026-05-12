using GameHost.Games.Lib.Installation.Providers;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Payloads.Responses.StatupParameters;

public sealed record HostStartupParameterResponse
{
    public string Key { get; init; } = default!;

    [JsonConverter(typeof(JsonAlwaysStringConverter))]
    public string? ForcedValue { get; set; }

    [JsonConverter(typeof(JsonAlwaysStringConverter))]
    public string? DefaultValue { get; set; }
}
