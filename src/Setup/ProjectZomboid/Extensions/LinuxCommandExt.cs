using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using System.Text.RegularExpressions;

namespace GameHost.Games.ProjectZomboid.Console.Extensions;

internal static class LinuxCommandExt
{
    public const string REGEX_IGNORE_COMMENTS = @"^([^\s#][^#=]*=([^#\r\n]*)?)";
    public static async Task WriteIntoConfigFile(this ILinuxCommand linuxCommand, Dictionary<string, string> keyContent, string file, ISafeFileWriter safeFileWriter, CancellationToken ct = default)
    {
        List<string> found = new();
        await safeFileWriter.FindLineFor(file, (line) =>
        {
            foreach (var item in keyContent)
            {
                System.Console.WriteLine("{0} =? {1}", item.Key, line);
                if (line.Contains(item.Key))
                {
                    if (found.Contains(item.Key)) return string.Empty;
                    string newLine = $"{item.Key}={item.Value}";
                    found.Add(item.Key);
                    return newLine;
                }
            }
            return default;
        },
        () =>
        {
            var extraLines = new List<string>();
            foreach (var item in keyContent)
                if (!found.Contains(item.Key))
                    extraLines.Add($"{item.Key}={item.Value}");
            return extraLines.ToArray();
        }, ct);

    }

    private static async Task FindLineFor(this ISafeFileWriter safeFileWriter, string file, Func<string, string?> lineProcess, Func<string[]> finishing, CancellationToken ct = default)
    {
        string[] lines = await File.ReadAllLinesAsync(file, ct);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;
            var matchNoComments = Regex.Match(line, REGEX_IGNORE_COMMENTS);
            if (!matchNoComments.Success) continue;
            line = matchNoComments.Groups[1].Value;
            var replaceWith = lineProcess(line);
            if (replaceWith == default) continue;
            lines[i] = replaceWith;
        }
        var resultToWrite = lines.Where(p => p != string.Empty).ToList();
        resultToWrite.AddRange(finishing());
        string content = string.Join('\n', resultToWrite);
        await safeFileWriter.WriteThenCopyFileAsync(file, content + "\n", ct, $"{BaseInfo.USERNAME}:{BaseInfo.USERNAME}", "640");

    }
}
