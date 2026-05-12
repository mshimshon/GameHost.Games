using System.Text.Json;

namespace GameHost.Games.Lib.Installation.Extensions;

public static class PayloadBuilderExt
{

    public static Task PrintAsync(this object payload, CancellationToken ct = default)
    {
        string json = "";
        if (payload.GetType() == typeof(string))
            json = payload.ToString()!;
        else
            json = JsonSerializer.Serialize(payload, BaseInfo.jsonSerializerOptions);
        string payloadString = $"<<<PAYLOAD_BEGIN>>>\n{json}\n<<<PAYLOAD_END>>>";
        Console.Out.WriteLine(payloadString);
        return Task.CompletedTask;
    }


}
