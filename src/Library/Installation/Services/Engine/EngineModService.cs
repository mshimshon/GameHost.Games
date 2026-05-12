using GameHost.Games.Lib.Installation.Payloads.Responses.Mods;
using GameHost.Games.Lib.Installation.Optionals;

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
    public Task<ModFeatureResponse> GetDetailsAsync(CancellationToken ct = default) => _serverModControl.GetDetailsAsync(ct);
    public Task<bool> IsSupportedAsync(CancellationToken ct = default) => _serverModControl.IsSupportedAsync(ct);
    public Task ProcessModListAsync(CancellationToken ct = default) => _serverModControl.ProcessModListAsync(ct);
    public Task RemoveCurrentAsync(CancellationToken ct = default)
     => _modHelperService.RemoveCurrentAsync(ct);
}
