using GameHost.Core.Features;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation;

public static class BaseInfo
{
    public const string USERNAME = "lgsm";
    public const string MODULE_INFO = "GameInstallLibrary";
    public const string PLUGIN_MODULE_NAME = LinuxGameServerKeys.MODULE_NAME;
    public const string MOCK_FOLDER = "mockup";
    public const string DISTRO_DEP_FILENAME_FORMAT = "dep_{0}_{1}";
    public const string DISTRO_DEP_FILENAME = "distro_dependencies.json";
    public const string SERVER_CONTROL = "server_control";
    public const string SERVER_CONTROL_CONFIG = "config";
    public const string MANIFEST_FILENAME = "manifest.json";
    public const string SERVER_MANIFEST_FILE = "installer_manifest.json";
    public const string GAME_INFO_FILENAME = "game_info.json";
    //$"dep_{distroInfo.Id.ToLower()}_{distroInfo.VersionId}.json"
    public static string[] dependencies = ["sudo"];
    public static JsonSerializerOptions jsonSerializerOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
#if DEBUG
        WriteIndented = true
#endif
    };
}
