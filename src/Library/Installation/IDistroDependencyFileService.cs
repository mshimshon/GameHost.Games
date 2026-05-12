using GameHost.Games.Lib.Installation.Contracts.Responses;

namespace GameHost.Games.Lib.Installation;

public interface IDistroDependencyFileService
{
    Task<DistroInformationResponse> GetDistroAsync(CancellationToken ct = default);
    Task InstallDependenciesAsync(string gameName, Func<string, CancellationToken, Task> updateProgressStatus, CancellationToken ct = default);
    Task DownloadOfficialDistroDependencyFile(CancellationToken ct = default);
}
