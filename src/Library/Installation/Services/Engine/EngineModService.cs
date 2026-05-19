using GameHost.Games.Lib.Installation.Optionals;
using GameHost.Games.Lib.Installation.Payloads.Responses.Mods;

namespace GameHost.Games.Lib.Installation.Services.Engine;

internal class EngineModService : IEngineModService
{
    private readonly IServerModControl _serverModControl;
    private readonly IModHelperService _modHelperService;


    //         string path = _pluginUserLocation.GetUserConfigBase(ModListKeys.MODULE_NAME, [ModListKeys.USER_SAVED_MODLIST_FOLDER_NAME]);
    //var modlists = Directory.EnumerateFiles(path, "modlist-*.json");
    public EngineModService(IServerModControl serverModControl, IModHelperService modHelperService)
    {
        _serverModControl = serverModControl;
        _modHelperService = modHelperService;
    }
    public async Task<ModFeatureResponse> GetDetailsAsync(CancellationToken ct = default)
    {
        var result = await _serverModControl.GetDetailsAsync(ct);
        var enabled = await _serverModControl.IsSupportedAsync(ct);
        return result with { Modding = enabled };
    }
    public Task<bool> IsSupportedAsync(CancellationToken ct = default) => _serverModControl.IsSupportedAsync(ct);
    public Task ProcessModListAsync(CancellationToken ct = default) => _serverModControl.ProcessModListAsync(ct);
    public Task RemoveCurrentAsync(CancellationToken ct = default)
     => _modHelperService.RemoveCurrentAsync(ct);
}
