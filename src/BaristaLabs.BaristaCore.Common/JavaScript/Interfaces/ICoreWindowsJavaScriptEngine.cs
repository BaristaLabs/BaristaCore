namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

    using System;
	using System.Runtime.InteropServices;

    /// <summary>
    /// ChakraCore.h interface
    /// </summary>
    public interface ICoreWindowsJavaScriptEngine
	{
		/// <summary>
		///		Initialize a ModuleRecord from host
		/// </summary>
		/// <remarks>
		///		Bootstrap the module loading process by creating a new module record.
		/// </remarks>
		/// <param name="referencingModule">
		///		The referencingModule as in HostResolveImportedModule (15.2.1.17). nullptr if this is the top level module.
		/// </param>
		/// <param name="normalizedSpecifier">
		///		The host normalized specifier. This is the key to a unique ModuleRecord.
		/// </param>
		/// <returns>
		///		The new ModuleRecord created. The host should not try to call this API twice with the same normalizedSpecifier.
		///		chakra will return an existing ModuleRecord if the specifier was passed in before.
		/// </returns>
		IntPtr JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier);

		/// <summary>
		///		Parse the module source
		/// </summary>
		/// <remarks>
		///		This is basically ParseModule operation in ES6 spec. It is slightly different in that the ModuleRecord was initialized earlier, and passed in as an argument.
		/// </remarks>
		/// <param name="requestModule">
		///		The ModuleRecord that holds the parse tree of the source code.
		/// </param>
		/// <param name="sourceContext">
		///		A cookie identifying the script that can be used by debuggable script contexts.
		/// </param>
		/// <param name="script">
		///		The source script to be parsed, but not executed in this code.
		/// </param>
		/// <param name="scriptLength">
		///		The source length of sourceText. The input might contain embedded null.
		/// </param>
		/// <param name="sourceFlag">
		///		The type of the source code passed in. It could be UNICODE or utf8 at this time.
		/// </param>
		/// <returns>
		///		The error object if there is parse error.
		/// </returns>
		JavaScriptValueSafeHandle JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag);

		/// <summary>
		///		Execute module code.
		/// </summary>
		/// <remarks>
		///		This method implements 15.2.1.1.6.5, "ModuleEvaluation" concrete method.
		///		When this methid is called, the chakra engine should have notified the host that the module and all its dependent are ready to be executed.
		///		One moduleRecord will be executed only once. Additional execution call on the same moduleRecord will fail.
		/// </remarks>
		/// <param name="requestModule">
		///		The module to be executed.
		/// </param>
		/// <returns>
		///		The return value of the module.
		/// </returns>
		JavaScriptValueSafeHandle JsModuleEvaluation(IntPtr requestModule);

		/// <summary>
		///		Set the host info for the specified module.
		/// </summary>
		void JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo);

		/// <summary>
		///		Retrieve the host info for the specified module.
		/// </summary>
		/// <returns>
		///		The host info to be retrieved.
		/// </returns>
		IntPtr JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo);

	}
}