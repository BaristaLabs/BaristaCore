namespace BaristaLabs.BaristaCore.Tests.ModuleLoaders
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [BaristaModule("hello_world", "Only the best module ever.")]
    public sealed class HelloWorldModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return context.CreateString("Hello, World!");
        }
    }
}
