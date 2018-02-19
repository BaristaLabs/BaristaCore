namespace BaristaLabs.BaristaCore.Benchmarks
{
    using BaristaLabs.BaristaCore.Extensions;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class Adding
    {
        private IServiceProvider m_serviceProvider;
        private IBaristaRuntimeFactory m_runtimeFactory;

        public Adding()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBaristaCore();

            m_serviceProvider = serviceCollection.BuildServiceProvider();
            m_runtimeFactory = m_serviceProvider.GetRequiredService<IBaristaRuntimeFactory>();
        }

        [Benchmark]
        public void EvalutateWithNoReuse()
        {
            using (var rt = m_runtimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            {
                ctx.EvaluateModule("export default 6*7;");
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Adding>();
        }
    }
}
