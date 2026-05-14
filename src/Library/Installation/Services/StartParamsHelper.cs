using GameHost.Core.Features;
using GameHost.Games.Lib.Installation.Payloads.Responses.GameInfo;
using GameHost.Games.Lib.Installation.Payloads.Responses.StatupParameters;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameHost.Games.Lib.Installation.Services;

internal class StartParamsHelper : IStartParamsHelper
{
    private List<UserStartupParameterResponse>? _userCachedStartupParams;
    private List<HostStartupParameterResponse>? _hostCachedStartupParams;
    private List<GameStartupParameterResponse>? _defCachedStartupParams;
    private readonly IPluginSystemLocation _pluginSystemLocation;
    private readonly IMetadataService _metadataService;
    private readonly IPluginUserLocation _pluginUserLocation;
    private readonly JsonSerializerOptions _serializerOption = new JsonSerializerOptions()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    public StartParamsHelper(IPluginLocation pluginLocation, IMetadataService metadataService)
    {
        _pluginSystemLocation = pluginLocation;
        _metadataService = metadataService;
        _pluginUserLocation = pluginLocation;
        pluginLocation.SetUsername(BaseInfo.USERNAME);

    }

    public async Task<ICollection<UserStartupParameterResponse>> GetUserStartupParametersAsync(CancellationToken ct = default)
    {
        if (_userCachedStartupParams != default) return _userCachedStartupParams;
        string file = _pluginUserLocation.GetUserConfigFor(LifecycleKeys.MODULE_NAME, LifecycleKeys.USER_STARTUP_PARAM_FILE);
        if (!File.Exists(file)) return new List<UserStartupParameterResponse>();
        try
        {
            string jsonString = await File.ReadAllTextAsync(file);
            _userCachedStartupParams = JsonSerializer.Deserialize<List<UserStartupParameterResponse>>(jsonString, _serializerOption)!;
            if (_userCachedStartupParams == default) return new List<UserStartupParameterResponse>();
            return _userCachedStartupParams;
        }
        catch (Exception)
        {

        }
        return new List<UserStartupParameterResponse>();
    }

    public async Task<ICollection<HostStartupParameterResponse>> GetHostStartupParametersAsync(CancellationToken ct = default)
    {
        if (_hostCachedStartupParams != default) return _hostCachedStartupParams;
        string file = _pluginSystemLocation.GetConfigFor(LifecycleKeys.MODULE_NAME, LifecycleKeys.HOST_STARTUP_PARAM_FILE);
        if (!File.Exists(file)) return new List<HostStartupParameterResponse>();
        try
        {
            string jsonString = await File.ReadAllTextAsync(file);
            _hostCachedStartupParams = JsonSerializer.Deserialize<List<HostStartupParameterResponse>>(jsonString, _serializerOption)!;
            if (_hostCachedStartupParams == default) return new List<HostStartupParameterResponse>();
            return _hostCachedStartupParams;
        }
        catch (Exception)
        {

        }
        return new List<HostStartupParameterResponse>();
    }

    public async Task<string> GetStartupParametersAsync(CancellationToken ct = default)
    {
        ICollection<GameStartupParameterResponse>? definition = await GetStartupParametersDefinitionAsync(ct);
        if (definition == default || definition.Count <= 0) return string.Empty;
        var userDefinedParams = await GetUserStartupParametersAsync(ct);
        var hostDefinedParams = await GetHostStartupParametersAsync(ct);
        var finalResultBuilder = new Dictionary<string, string?>();
        List<string> finalLines = new();
        foreach (GameStartupParameterResponse item in definition)
        {
            var userDef = userDefinedParams.LastOrDefault(p => p.Key == item.Key);
            var hostDef = hostDefinedParams.LastOrDefault(p => p.Key == item.Key);
            var line = ConvertStartupParamToInlineString(item, userDef, hostDef);

            if (line == default) continue;
            finalLines.Add(line);
        }
        var result = string.Join(' ', finalLines);
        return result;
    }

    public async Task<ICollection<GameStartupParameterResponse>> GetStartupParametersDefinitionAsync(CancellationToken ct = default)
    {
        if (_defCachedStartupParams != default) return _defCachedStartupParams;
        var result = await _metadataService.GetGameInfoAsync(ct);
        if (result == default) return new List<GameStartupParameterResponse>();
        _defCachedStartupParams = result.Parameters;
        return result.Parameters;
    }

    public async Task<string?> GetStartupValueFor(string key, CancellationToken ct = default)
    {
        ICollection<GameStartupParameterResponse>? definition = await GetStartupParametersDefinitionAsync(ct);
        if (definition == default || definition.Count <= 0) return default;
        var targetKey = definition.FirstOrDefault(p => p.Key == key);
        if (targetKey == default) return default;

        var userDefinedParams = await GetUserStartupParametersAsync(ct);
        var targetUserKey = userDefinedParams.FirstOrDefault(p => p.Key == key);
        var hostDefinedParams = await GetHostStartupParametersAsync(ct);
        var targetHostKey = hostDefinedParams.FirstOrDefault(p => p.Key == key);
        var targetValue = targetHostKey?.ForcedValue ?? targetUserKey?.Value ?? targetHostKey?.DefaultValue ?? targetKey.DefaultValue;
        return targetValue;
    }



    private string? ConvertStartupParamToInlineString(GameStartupParameterResponse definition, UserStartupParameterResponse? userDefined = default, HostStartupParameterResponse? hostDefined = default)
    {
        var targetValue = hostDefined?.ForcedValue ?? userDefined?.Value ?? hostDefined?.DefaultValue ?? definition.DefaultValue;
        if (targetValue == default) return default;
        var def = definition.Type;
        if (def == "bool")
            if (bool.TryParse(targetValue, out bool value) && value)
                return $"{definition.Key}";
            else
                return default;
        else if (def == "bool_E")
            if (bool.TryParse(targetValue, out bool value))
                return $"{definition.Key}={value}";
            else
                return default;
        else if (def == "bool_S")
            if (bool.TryParse(targetValue, out bool value))
                return $"{definition.Key}=\\\"{value}\\\"";
            else
                return default;
        else if (def == "integer" || def == "list_int")
            if (int.TryParse(targetValue, out int value))
                return $"{definition.Key}={value}";
            else
                return default;
        else if (def == "list_decimal" || def == "decimal")
            if (double.TryParse(targetValue, out double value))
                return $"{definition.Key}={value}";
            else
                return default;
        else if (def == "string" || def == "list_str")
            return $"{definition.Key}=\\\"{targetValue}\\\"";
        else
            return default;
    }
}
