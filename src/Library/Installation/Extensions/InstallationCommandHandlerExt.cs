using GameHost.Games.Lib.Installation.Services.Engine;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using static GameHost.Games.Lib.Installation.Extensions.CommandLineExt;

namespace GameHost.Games.Lib.Installation.Extensions;

internal static class InstallationCommandHandlerExt
{

    internal static Command SetInstallationAction(this Command command, IServiceProvider serviceProvider)
    {
        command.SetAction(async (parseResult, ct) =>
        {

            var engineInstallService = serviceProvider.GetRequiredService<IEngineInstallation>();
            bool success = false;
            if (parseResult.GetValue<bool>("--install"))
                success = await ExecuteCommandAsync(async () => await engineInstallService.InstallAsync((_, _) => Task.CompletedTask, ct));
            else if (parseResult.GetValue<bool>("--update"))
                success = await ExecuteCommandAsync(async () => await engineInstallService.UpdateAsync(ct));
            else if (parseResult.GetValue<bool>("--version"))
                success = await ExecuteCommandResultAsync(async () => await engineInstallService.GetVersionAsync(ct));
            else if (parseResult.GetValue<bool>("--check-update"))
                success = await ExecuteCommandResultAsync(async () => await engineInstallService.CheckUpdateAsync(ct));
            else
                await command.PrintHelp();
            if (success)
                Environment.Exit(0);
            else
                Environment.Exit(1);
        });
        return command;
    }

    internal static Command SetInitializingAction(this Command command, IServiceProvider serviceProvider)
    {
        var linuxCommandService = serviceProvider.GetRequiredService<IEngineInstallation>();
        command.SetAction(async (parseResult, ct) =>
        {
            bool success = await ExecuteCommandAsync(async () => await linuxCommandService.InitializeAsync(ct));

            if (success)
                Environment.Exit(0);
            else
                Environment.Exit(1);
        });
        return command;
    }


}
