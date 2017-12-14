namespace BaristaLabs.BaristaCore
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service that returns Barista Modules provided a module name.
    /// </summary>
    public interface IBaristaModuleLoader
    {
        IBaristaModule GetModule(string name);
    }
}