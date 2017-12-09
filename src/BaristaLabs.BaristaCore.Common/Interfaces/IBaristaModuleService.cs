namespace BaristaLabs.BaristaCore
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service that returns Barista Modules provided a module name.
    /// </summary>
    public interface IBaristaModuleService
    {
        IBaristaModule GetModule(string name);
    }
}