namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service that returns Barista Modules provided a module name.
    /// </summary>
    public interface IBaristaModuleLoader
    {
        Task<IBaristaModule> GetModule(string name);
    }
}