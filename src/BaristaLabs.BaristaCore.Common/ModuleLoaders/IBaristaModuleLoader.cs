namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    /// <summary>
    /// Represents a service that returns Barista Modules provided a module name.
    /// </summary>
    public interface IBaristaModuleLoader
    {
        IBaristaModule GetModule(string name);
    }
}