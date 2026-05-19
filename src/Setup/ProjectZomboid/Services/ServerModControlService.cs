using GameHost.Games.Lib.Installation;
using GameHost.Games.Lib.Installation.Optionals;
using GameHost.Games.Lib.Installation.Payloads.Responses.Mods;
using GameHost.Games.ProjectZomboid.Console.Extensions;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;

namespace GameHost.Games.ProjectZomboid.Console.Services;

internal class ServerModControlService : IServerModControl
{
    private readonly IModHelperService _modHelperService;
    private readonly IStartParamsHelper _startParamsHelper;
    private readonly ILinuxCommand _linuxCommand;
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly ICrazyReport<ServerModControlService> _crazyReport;

    public ServerModControlService(
        IModHelperService modHelperService,
        IStartParamsHelper startParamsHelper, ILinuxCommand linuxCommand, ISafeFileWriter safeFileWriter,
        ICrazyReport<ServerModControlService> crazyReport
        )
    {
        _modHelperService = modHelperService;
        _startParamsHelper = startParamsHelper;
        _linuxCommand = linuxCommand;
        _safeFileWriter = safeFileWriter;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(BaseInfo.MODULE);
    }

    public Task<ModFeatureResponse> GetDetailsAsync(CancellationToken ct = default)
        => Task.FromResult(new ModFeatureResponse()
        {
            ManualModDownload = false
        });

    public Task<bool> IsSupportedAsync(CancellationToken ct = default) => Task.FromResult(true);



    // sed -i "s/^[[:space:]]*#*[[:space:]]*mod=.*/mod=VALUE/; t; \$a mod=VALUE" file.cfg && echo true || echo false
    public async Task ProcessModListAsync(CancellationToken ct = default)
    {
        var configFileTarget = await _startParamsHelper.GetStartupValueFor(BaseInfo.SERVERNAME_KEY, ct);
        if (configFileTarget == default) return;
        string configFilePath = string.Format(BaseInfo.CONFIG_FILE_PATH, configFileTarget);
        _crazyReport.Report("Game Config file is {0}", configFilePath);

        bool configDoesNotExist = !File.Exists(configFilePath);
        if (configDoesNotExist) return;
        try
        {
            ModListResponse? currentModList = await _modHelperService.GetActiveAsync(ct);
            if (currentModList == default) throw new Exception();
            _crazyReport.Report("Current Modlist is {0}", currentModList);
            bool missingPart1 = !currentModList.Mods.ContainsKey("Part_1");
            bool missingPart2 = !currentModList.Mods.ContainsKey("Part_2");
            if (missingPart1 || missingPart2) throw new Exception();
            var p1 = currentModList.Mods["Part_1"];
            var p2 = currentModList.Mods["Part_2"];
            var workshopItems = p1.Select(p => p.Id).ToList();
            var modItems = p2.Select(p => p.Id).ToList();

            string workshopItemsLine = string.Join(';', workshopItems);
            string modItemsLine = string.Join(';', modItems);
            _crazyReport.Report("workshopItems={0}", workshopItemsLine);
            _crazyReport.Report("modItems={0}", modItemsLine);
            await _linuxCommand.WriteIntoConfigFile(new()
            {
                { BaseInfo.PZ_CONFIG_WORKSHOP, workshopItemsLine },
                { BaseInfo.PZ_CONFIG_MODS, modItemsLine }
            }, configFilePath, _safeFileWriter, ct);
        }
        catch (Exception)
        {
            await _linuxCommand.WriteIntoConfigFile(new()
            {
                { BaseInfo.PZ_CONFIG_WORKSHOP, string.Empty },
                { BaseInfo.PZ_CONFIG_MODS, string.Empty }
            }, configFilePath, _safeFileWriter, ct);
        }
    }
}
