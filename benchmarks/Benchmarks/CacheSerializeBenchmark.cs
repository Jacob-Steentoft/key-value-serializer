using BenchmarkDotNet.Attributes;
using KeyValueSerializer.Benchmarks.Models;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Deserialization;
using KeyValueSerializer.Serialization;

namespace KeyValueSerializer.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class CacheSerializeBenchmark
{
    private const string FileFolder = "SupportFiles";
    private const string CurrentConfigFilePath = $"{FileFolder}/arma3server.server.cfg";
    private const string NewConfigFilePath = $"{FileFolder}/arma3server.server.new.cfg";
    private readonly KeyValueCache _cache = new(typeof(ArmaServerOptions));
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
    public async ValueTask<ArmaServerOptions> CacheDeserializeAsync()
    {
        await using var fileStream = File.OpenRead(CurrentConfigFilePath);
        return await Deserializer.DeserializeStreamAsync<ArmaServerOptions>(fileStream, _cache, _configuration, _cancellationToken);
    }

    [Benchmark]
    public void CacheSerialize()
    {
        using var fileStream = File.OpenWrite(NewConfigFilePath);
        Serializer.Serialize(_armaServerOptions, fileStream, _cache, _configuration);
    }

}
