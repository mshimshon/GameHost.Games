using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

namespace GameHost.Games.Lib.Installation.Extensions;

internal static class HelperCommandExt
{
    public static LinuxCommandBuilderContext CheckUsernameExistCommand(this ILinuxCommand linuxCommand)
    {
        LinuxCommandBuilderContext? result = linuxCommand
            .BuildCommand($"id -u {BaseInfo.USERNAME} >/dev/null")
            .AndPrintPayload(bool.TrueString)
            .OrPrintPayload(bool.FalseString);
        return result;
    }


    public static LinuxCommandBuilderContext CheckDependencyCommand(this ILinuxCommand linuxCommand, string[] deps)
    {
        return linuxCommand
            .BuildCommand($"dpkg -s {string.Join(' ', deps)} >/dev/null")
            .AndPrintPayload(bool.TrueString)
            .OrPrintPayload(bool.FalseString);
    }


}
