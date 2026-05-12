using GameHost.Games.Lib.Installation.Payloads.Responses.GameInfo;
using GameHost.Games.Lib.Installation.Payloads.Responses.StatupParameters;

namespace GameHost.Games.Lib.Installation;

public interface IStartParamsHelper
{
    Task<string?> GetStartupValueFor(string key, CancellationToken ct = default);
    Task<ICollection<GameStartupParameterResponse>> GetStartupParametersDefinitionAsync(CancellationToken ct = default);
    Task<string> GetStartupParametersAsync(CancellationToken ct = default);
    Task<ICollection<HostStartupParameterResponse>> GetHostStartupParametersAsync(CancellationToken ct = default);
    Task<ICollection<UserStartupParameterResponse>> GetUserStartupParametersAsync(CancellationToken ct = default);
}
