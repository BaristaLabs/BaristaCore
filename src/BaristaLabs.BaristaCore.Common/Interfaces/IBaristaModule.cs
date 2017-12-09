namespace BaristaLabs.BaristaCore
{
    using JavaScript;

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
        /// Installs the module within the specified scope.
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        object InstallModule(BaristaContext context, JavaScriptModuleRecord referencingModule);
    }
}
