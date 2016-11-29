namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     User implemented callback to fetch additional imported modules.
    /// </summary>
    /// <remarks>
    /// Notify the host to fetch the dependent module. This is the "import" part before HostResolveImportedModule in ES6 spec.
    /// This notifies the host that the referencing module has the specified module dependency, and the host need to retrieve the module back.
    /// </remarks>
    /// <param name="referencingModule">The referencing module that is requesting the dependency modules.</param>
    /// <param name="specifier">The specifier coming from the module source code.</param>
    /// <param name="dependentModuleRecord">The ModuleRecord of the dependent module. If the module was requested before from other source, return the
    ///                           existing ModuleRecord, otherwise return a newly created ModuleRecord.</param>
    /// <returns>
    ///     true if the operation succeeded, false otherwise.
    /// </returns>
    public delegate bool JavaScriptFetchImportedModuleCallback(IntPtr referencingModule, IntPtr specifier, out IntPtr activateInterface);
}
