using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Contracts.Responses.Mods;

internal class GameInfoResponse
{
    public bool Modding { get; set; }
    public bool ManualModUpload { get; set; }
    [JsonPropertyName("mod_schema")]
    public Dictionary<string, ModSchematicResponse>? Schema { get; set; }
}
