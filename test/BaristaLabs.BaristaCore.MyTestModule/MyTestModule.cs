namespace BaristaLabs.BaristaCore.MyTestModule
{
    [BaristaModule("MyTestModule", "This is a test, this is only a test.")]
    public class MyTestModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return context.CreateString("The maze isn't meant for you.");
        }
    }
}
