using GameHost.Core.Features;
using GameHost.Games.Lib.Installation.Payloads.Responses;
using GameHost.Games.Lib.Installation.Exceptions;
using GameHost.Games.Lib.Installation.Extensions;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using System.Text.Json;

namespace GameHost.Games.Lib.Installation.Services.Engine;

internal sealed class EngineInitializerService : IEngineInitializerService
{
    private readonly ILinuxCommand _linuxCommand;
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly IPluginSystemLocation _pluginSystemLocation;
    private readonly IMetadataService _metadataService;

    public EngineInitializerService(ILinuxCommand linuxCommand, ISafeFileWriter safeFileWriter, IPluginLocation pluginLocation, IMetadataService metadataService)
    {
        _linuxCommand = linuxCommand;
        _safeFileWriter = safeFileWriter;
        _pluginSystemLocation = pluginLocation;
        _metadataService = metadataService;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        string identityFile = _pluginSystemLocation.GetConfigFor(LinuxGameServerKeys.MODULE_NAME, LinuxGameServerKeys.SERVER_MANIFEST_FILE);
        var manifest = await _metadataService.GetManifestAsync(ct);
        var identityContent = new ServerIdentityResponse()
        {
            DisplayName = manifest.DisplayName,
            Id = manifest.Id
        };
        var identityContentJson = JsonSerializer.Serialize(identityContent, BaseInfo.jsonSerializerOptions);
        await _safeFileWriter.WriteThenCopyFileAsync(identityFile, identityContentJson, ct);

        bool isUsernameCreated = await _linuxCommand.CheckUsernameExistCommand().ExecPayloadAsync<bool>(ct);
        if (!isUsernameCreated)
        {
            var resultUsercreate = await _linuxCommand
            .BuildCommand($"adduser --home --system --shell /usr/sbin/nologin {BaseInfo.USERNAME} ")
            .ExecAsync();
            if (resultUsercreate.Failed)
                throw new CreateUsernameFailedException(resultUsercreate.StandardError);
        }
        var resultDepInstall = await _linuxCommand
        .BuildCommand($"apt-get install -y {string.Join(' ', BaseInfo.dependencies)}")
        .ExecAsync();
        if (resultDepInstall.Failed)
            throw new InstallDependenciesFailedException(resultDepInstall.StandardError);
    }
}
