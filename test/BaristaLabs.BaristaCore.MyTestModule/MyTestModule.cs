namespace BaristaLabs.BaristaCore.MyTestModule
{
    using System.Threading.Tasks;

    [BaristaModule("MyTestModule", "This is a test, this is only a test.")]
    public class MyTestModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult((object)"The maze isn't meant for you.");
        }
    }
}
