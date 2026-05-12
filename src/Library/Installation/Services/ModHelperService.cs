using GameHost.Core.Features;
using GameHost.Games.Lib.Installation.Contracts.Responses.Mods;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Services;

internal class ModHelperService : IModHelperService
{
    private readonly IPluginSystemLocation _pluginSystemLocation;
    private readonly IPluginUserLocation _pluginUserLocation;
    private readonly JsonSerializerOptions _serializerOption = new JsonSerializerOptions()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    public ModHelperService(IPluginLocation pluginLocation)
    {
        _pluginSystemLocation = pluginLocation;
        _pluginUserLocation = pluginLocation;
        pluginLocation.SetUsername(BaseInfo.USERNAME);

    }

    public Task RemoveCurrentAsync(CancellationToken ct = default)
    {
        var currentFile = _pluginUserLocation.GetUserConfigFor(ModListKeys.MODULE_NAME, $"modlist_current.json");
        bool doesNotExist = !File.Exists(currentFile);
        if (doesNotExist)
            return Task.CompletedTask;
        File.Delete(currentFile);
        return Task.CompletedTask;
    }

    public Task RemoveByIdAsync(Guid id, CancellationToken ct = default)
    {
        string filename = _pluginUserLocation.GetUserConfigFor(ModListKeys.MODULE_NAME, [ModListKeys.USER_SAVED_MODLIST_FOLDER_NAME], $"modlist-{id}.json");
        bool fileDoesNotExist = !File.Exists(filename);
        if (fileDoesNotExist)
            return Task.CompletedTask;
        File.Delete(filename);
        return Task.CompletedTask;
    }

    public async Task<ModListResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        string filename = _pluginUserLocation.GetUserConfigFor(ModListKeys.MODULE_NAME, [ModListKeys.USER_SAVED_MODLIST_FOLDER_NAME], $"modlist-{id}.json");
        bool fileDoesNotExist = !File.Exists(filename);
        if (fileDoesNotExist) return default;
        try
        {
            string content = await File.ReadAllTextAsync(filename, ct);
            var dto = JsonSerializer.Deserialize<ModListResponse>(content, _serializerOption);
            if (dto == default)
                return default;
            return dto;
        }
        catch (Exception)
        {
        }
        return default;
    }

    public async Task<ModListResponse?> GetActiveAsync(CancellationToken ct = default)
    {
        var currentFile = _pluginUserLocation.GetUserConfigFor(ModListKeys.MODULE_NAME, $"modlist_current.json");
        bool doesNotExist = !File.Exists(currentFile);
        if (doesNotExist)
            return default;

        try
        {
            string strGuid = await File.ReadAllTextAsync(currentFile, ct);
            bool idInvalid = !Guid.TryParse(strGuid, out Guid result);
            if (idInvalid)
            {
                File.Delete(currentFile);
                return default;
            }
            return await GetByIdAsync(result, ct);
        }
        catch (Exception)
        {

        }
        return default;

    }
}
