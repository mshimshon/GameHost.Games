using GameHost.Games.Lib.Installation.Optionals;
using GameHost.Games.Lib.Installation.Services;
using GameHost.Games.Lib.Installation.Services.Engine;
using LunaticPanel.Core.Utils;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using static GameHost.Core.BaseInfo;

namespace GameHost.Games.Lib.Installation;

public static class ServiceRegistrationExt
{
    public static void AddInstallationServices(this IServiceCollection services)
    {
        services.AddScoped<IServerModControl, DefaultServerModControlService>();
        services.AddScoped<IServerBackupControl, DefaultServerBackupService>();
        services.AddScoped<IServerDebugControl, DefaultServerDebugControlService>();
        services.AddScoped<IDistroDependencyFileService, DistroDependencyFileService>();
        services.AddPluginLocationUtilityService(AssemblyName);
        services.AddLinuxCommandUtilityService();
        services.AddCrazyReportUtilityService();
        services.AddSafeFileWriterUtilityService();
        services.AddScoped<ICrazyReportCircuit, CrazyReportCircuit>();
        services.AddScoped<IEngineInstallation, EngineInstallationService>();
        services.AddScoped<IMetadataService, MetadataService>();
        services.AddScoped<IEngineInitializerService, EngineInitializerService>();
        services.AddScoped<IEngineModService, EngineModService>();
        services.AddScoped<IModHelperService, ModHelperService>();
        services.AddScoped<IStartParamsHelper, StartParamsHelper>();
    }

}
