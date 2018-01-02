namespace BaristaLabs.BaristaCore.Tests.ModuleLoaders
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    [BaristaModule("hello_world", "Only the best module ever.")]
    public sealed class HelloWorldModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>("Hello, World!");
        }
    }
}
