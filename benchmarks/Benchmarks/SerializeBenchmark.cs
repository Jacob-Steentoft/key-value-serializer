using BenchmarkDotNet.Attributes;
using KeyValueSerializer.Benchmarks.Models;

namespace KeyValueSerializer.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class SerializeBenchmark
{
    private const string FileFolder = "SupportFiles";
    private const string CurrentConfigFilePath = $"{FileFolder}/arma3server.server.cfg";
    private const string NewConfigFilePath = $"{FileFolder}/arma3server.server.new.cfg";
    private readonly KeyValueConfiguration _configuration = new();
    private readonly CancellationToken _cancellationToken = new();
    
    private readonly ArmaServerOptions _armaServerOptions = new()
    {
        KickDuplicate = 0,
        Admins = new []{ "dave", "chuck"},
        Hostname = "Håst",
        ForcedDifficulty = "test",
        DisconnectTimeout = 2,
        BattlEye = 0,
        VoteThreshold = 10.33,
        Upnp = true
    };

    [Benchmark]
    public async ValueTask<ArmaServerOptions> DeserializeAsync()
    {
        await using var fileStream = File.OpenRead(CurrentConfigFilePath);
        return await KeyValueSerializer.DeserializeAsync<ArmaServerOptions>(fileStream, _configuration, _cancellationToken);
    }

    [Benchmark]
    public void Serialize()
    {
        using var fileStream = File.OpenWrite(NewConfigFilePath);
        KeyValueSerializer.Serialize(_armaServerOptions, fileStream);
    }
    
}
