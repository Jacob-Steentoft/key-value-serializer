using BenchmarkDotNet.Running;
using KeyValueSerializer.Benchmarks.Benchmarks;

BenchmarkRunner.Run<CacheSerializeBenchmark>();
