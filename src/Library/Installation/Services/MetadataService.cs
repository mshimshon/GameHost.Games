using GameHost.Core.Features;
using GameHost.Games.Lib.Installation.Contracts.Responses;
using GameHost.Games.Lib.Installation.Contracts.Responses.GameInfo;
using GameHost.Games.Lib.Installation.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using System.Text.Json;

namespace GameHost.Games.Lib.Installation.Services;

internal class MetadataService : IMetadataService
{
    private readonly ManifestResponse _manifest;
    private readonly GameInfoResponse _gameInfo;
    private readonly IPluginUserLocation _pluginUserLocation;

    public MetadataService(IPluginLocation pluginLocation)
    {
        _pluginUserLocation = pluginLocation;
        _pluginUserLocation.SetUsername(BaseInfo.USERNAME);
        _manifest = LoadManifest();
        _gameInfo = LoadGameInformation();
    }
    private ManifestResponse LoadManifest()
    {
        string manifestFile = _pluginUserLocation.
            GetUserBashFor(LinuxGameServerKeys.MODULE_NAME, [BaseInfo.SERVER_CONTROL, BaseInfo.SERVER_CONTROL_CONFIG], BaseInfo.MANIFEST_FILENAME);
        if (!File.Exists(manifestFile)) throw new ManifestNotFoundException();
        try
        {
            string json = File.ReadAllText(manifestFile);
            var value = JsonSerializer.Deserialize<ManifestResponse>(json, BaseInfo.jsonSerializerOptions);
            if (value == default)
                throw new FailedToReadManifestException();
            return value;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw new FailedToReadManifestException();
        }
    }

    private GameInfoResponse LoadGameInformation()
    {
        string gameInfoFile = _pluginUserLocation.
            GetUserBashFor(LinuxGameServerKeys.MODULE_NAME, [BaseInfo.SERVER_CONTROL, BaseInfo.SERVER_CONTROL_CONFIG], BaseInfo.GAME_INFO_FILENAME);
        if (!File.Exists(gameInfoFile)) throw new GameInfoNotFoundException();
        try
        {
            string json = File.ReadAllText(gameInfoFile);
            var value = JsonSerializer.Deserialize<GameInfoResponse>(json, BaseInfo.jsonSerializerOptions);
            if (value == default)
                throw new FailedToReadGameInfoException();
            return value;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw new FailedToReadGameInfoException();
        }
    }

    public Task<GameInfoResponse> GetGameInfoAsync(CancellationToken ct = default)
    => Task.FromResult(_gameInfo);

    public Task<ManifestResponse> GetManifestAsync(CancellationToken ct = default)
        => Task.FromResult(_manifest);
}
