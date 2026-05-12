using LunaticPanel.Core.Utils.Logging;

namespace GameHost.Games.Lib.Installation;

internal class CrazyReportCircuit : ICrazyReportCircuit
{
    public Guid CircuitId { get; } = Guid.NewGuid();
}
