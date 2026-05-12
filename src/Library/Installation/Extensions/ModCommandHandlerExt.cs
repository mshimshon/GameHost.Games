using GameHost.Games.Lib.Installation.Services.Engine;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using static GameHost.Games.Lib.Installation.Extensions.CommandLineExt;
namespace GameHost.Games.Lib.Installation.Extensions;

internal static class ModCommandHandlerExt
{
    internal static Command SetModAction(this Command command, IServiceProvider serviceProvider)
    {
        command.SetAction(async (parseResult, ct) =>
        {
            var serviceModControl = serviceProvider.GetRequiredService<IEngineModService>();
            bool success = false;
            if (parseResult.GetValue<bool>("--check"))
                success = await ExecuteCommandResultAsync(async () => await serviceModControl.IsSupportedAsync(ct));
            else if (parseResult.GetValue<bool>("--details"))
                success = await ExecuteCommandResultAsync(async () => await serviceModControl.GetDetailsAsync(ct));
            else if (parseResult.GetValue<bool>("--rm-current"))
                success = await ExecuteCommandAsync(async () => await serviceModControl.RemoveCurrentAsync(ct));
            else
                await command.PrintHelp();
            if (success)
                Environment.Exit(0);
            else
                Environment.Exit(1);
        });
        return command;
    }
}
