using LunaticPanel.Core.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace GameHost.Games.Lib.Steam;

public static class RegisterServicesExt
{
    public static void AddSteamServices(this IServiceCollection services)
    {
        services.AddLinuxCommandUtilityService();
        services.AddCrazyReportUtilityService();
    }

}
