namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;

    /// <summary>
    /// Represents a module that returns a script that will be parsed when imported.
    /// </summary>
    public class BaristaScriptModule : IBaristaModule
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Script
        {
            get;
            set;
        }

        public virtual object InstallModule(BaristaContext context, JavaScriptModuleRecord referencingModule)
        {
            return Script;
        }
    }
}
