namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     User implemented callback to get notification when the module is ready.
    /// </summary>
    /// <remarks>
    /// Notify the host after ModuleDeclarationInstantiation step (15.2.1.1.6.4) is finished. If there was error in the process, exceptionVar
    /// holds the exception. Otherwise the referencingModule is ready and the host should schedule execution afterwards.
    /// </remarks>
    /// <param name="referencingModule</param>The referencing module that have finished running ModuleDeclarationInstantiation step.
    /// <param name="exceptionVar">If nullptr, the module is successfully initialized and host should queue the execution job
    ///                           otherwise it's the exception object.</param>
    /// <returns>
    ///     true if the operation succeeded, false otherwise.
    /// </returns>
    public delegate bool JavaScriptNotifyModuleReadyCallback(IntPtr referencingModule, JavaScriptValueSafeHandle exceptionVar);
}
