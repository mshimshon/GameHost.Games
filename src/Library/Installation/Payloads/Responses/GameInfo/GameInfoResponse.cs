using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Payloads.Responses.GameInfo;

public sealed class GameInfoResponse
{
    public string Name { get; set; } = default!;
    public bool Steam { get; set; }

    [JsonPropertyName("steam_app_id")]
    public string? SteamAppId { get; set; }
    public bool Modding { get; set; }
    public bool Workshop { get; set; }

    [JsonPropertyName("manual_mod_upload")]
    public bool ManualModUpload { get; set; }
    public List<GameStartupParameterResponse> Parameters { get; init; } = new();
}
