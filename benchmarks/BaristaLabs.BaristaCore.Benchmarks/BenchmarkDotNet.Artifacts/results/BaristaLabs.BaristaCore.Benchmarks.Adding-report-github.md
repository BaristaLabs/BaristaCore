``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 1 [1607, Anniversary Update] (10.0.14393.2007)
Intel Core i5-4690 CPU 3.50GHz (Haswell), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3410012 Hz, Resolution=293.2541 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|               Method |     Mean |     Error |    StdDev | Rank |
|--------------------- |---------:|----------:|----------:|-----:|
| EvalutateWithNoReuse | 3.089 ms | 0.0456 ms | 0.0427 ms |    1 |
