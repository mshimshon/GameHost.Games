using GameHost.Games.Lib.Installation.Extensions;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace GameHost.Games.Lib.Installation;

public static class RuntimeExt
{


    private static async Task<bool> HasPreRequisite(this IServiceProvider sp, CancellationToken ct)
    {
        var commandService = sp.GetRequiredService<ILinuxCommand>();

        bool pass = true;
        pass = await commandService.CheckUsernameExistCommand().ExecPayloadAsync<bool>(ct);
        if (!pass) return false;
        pass = await commandService.CheckDependencyCommand(BaseInfo.dependencies).ExecPayloadAsync<bool>(ct);
        if (!pass) return false;
        return pass;
    }

    /// <summary>
    /// This need to be runing first before any subsequent logic
    /// </summary>
    internal static async Task RunStartupCommandAsync(this IServiceProvider services, CancellationToken ct, params string[] args)
    {
        /*
         We were checking --details for mods which didn't work for the panel then getting ready
            to create a modlist set as ative and test if start process the modlist properly.
         */
        // id -u username >/dev/null 2>&1 && echo true || echo false
        bool prerequisite = await services.HasPreRequisite(ct);
        var rootCommand = new RootCommand("Game Server Installation CLI");
        var modCommand = new Command("mod", "All commands related to mods support.")
        .AddOption<bool>("check", "c", "Check if the game server is supporting mods.")
        .AddOption<bool>("details", "dt", "Get the details about mods.")
        .AddOption<bool>("rm-current", "rmc", "Remove current as active modlist.")
        .SetModAction(services);

        var setupCommand = new Command("setup", "All commands related to setup.")
            .AddOption<bool>("install", "i", "Perform installation of the game server.")
            .AddOption<bool>("update", "u", "Perform update for game server.")
            .AddOption<bool>("version", "v", "Perform check for curren version of installed game server.")
            .AddOption<bool>("check-update", "cu", "Perform check if new update for game server is available.")
            .SetInstallationAction(services);

        var serverCommand = new Command("server", "All commands related to server control.")
            .AddOption<bool>("start", "st", "Start the server.")
            .AddOption<bool>("stop", "sp", "Stop the server.")
            .AddOption<bool>("restart", "r", "Restart the server.")
            .AddOption<bool>("status", "s", "Status the server.")
            .SetServerAction(services);
        var initCommand = new Command("initialize", "Command to install and setup all prerequisite for the installation of new game server.")
.SetInitializingAction(services);
        rootCommand.WithSubCommand(modCommand)
            .WithSubCommand(setupCommand)
            .WithSubCommand(serverCommand)
            .WithSubCommand(initCommand);


        var cmdParsed = rootCommand.Parse(args);

        if (cmdParsed.Errors.Count > 0)
        {
            await rootCommand.PrintHelp();
            Environment.Exit(1);
        }
        else
        {
            await cmdParsed.InvokeAsync(null, ct);
        }

    }
}
