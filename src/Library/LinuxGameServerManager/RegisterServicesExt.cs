using GameHost.Games.Lib.LinuxGameServerManager.Engine;
using GameHost.Games.Lib.LinuxGameServerManager.Providers.DetailExtratorFactory.Extensions;
using LunaticPanel.Core.Utils;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GameHost.Games.Lib.LinuxGameServerManager;

public static class RegisterServicesExt
{
    public static void AddLinuxGameServerManagerServices(this IServiceCollection services)
    {
        services.AddLinuxCommandUtilityService();
        services.AddCrazyReportUtilityService();
        services.AddScoped<ILinuxGameServerManagerService, LinuxGameServerManagerService>();
        services.AddScoped<IServerInstallService, ServerInstallService>();
        services.AddScoped<IServerControlService, ServerControlService>();
        services.AddScoped<IServerBackupService, ServerBackupService>();
        services.AddScoped<ICrazyReportCircuit, CrazyReportCircuit>();
        services.AddDetailExtratorFactory();

    }

}
