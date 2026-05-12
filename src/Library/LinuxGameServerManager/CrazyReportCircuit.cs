using LunaticPanel.Core.Utils.Logging;

namespace GameHost.Games.Lib.LinuxGameServerManager;

internal class CrazyReportCircuit : ICrazyReportCircuit
{
    public Guid CircuitId { get; } = Guid.NewGuid();
}
