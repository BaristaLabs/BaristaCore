namespace BaristaLabs.BaristaCore
{
    /// <summary>
    /// Represents a BaristaModule that is used by Barista scripts to provide functionality.
    /// </summary>
    public interface IBaristaModule
    {
        /// <summary>
        /// Gets the default export of the module given the specified scope and referencing module.
        /// </summary>
        /// <param name="context">The current BaristaContext</param>
        /// <param name="referencingModule">The module that is requesting the module.</param>
        /// <returns></returns>
        JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule);
    }
}
