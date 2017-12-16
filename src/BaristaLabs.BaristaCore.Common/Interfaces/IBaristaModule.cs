namespace BaristaLabs.BaristaCore
{
    using JavaScript;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a BaristaModule that is used by Barista scripts to provide functionality.
    /// </summary>
    public interface IBaristaModule
    {
        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the description of the module.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets the default export of the module given the specified scope and referencing module.
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        Task<object> ExportDefault(BaristaContext context, JavaScriptModuleRecord referencingModule);
    }
}
