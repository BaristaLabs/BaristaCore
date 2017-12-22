namespace BaristaLabs.BaristaCore
{
    /// <summary>
    /// Decorator interface that indicates a module that returns a script file that should be parsed and executed.
    /// </summary>
    public interface IBaristaScriptModule : IBaristaModule
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
    }
}
