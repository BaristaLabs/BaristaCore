namespace BaristaLabs.BaristaCore.JavaScript.Interfaces
{
    using Callbacks;
    using SafeHandles;

    using System;

    /// <summary>
    /// ChakraCore.h interface
    /// </summary>
    internal interface IChakraCore
	{
		/// <summary>
		///     Creates the property ID associated with the name.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Property IDs are specific to a context and cannot be used across contexts.
		///     </para>
		///     <para>
		///         Requires an active script context.
		///     </para>
		/// </remarks>
		/// <param name="name">
		///     The name of the property ID to get or create. The name may consist of only digits.
		///     The string is expected to be ASCII / utf8 encoded.
		/// </param>
		/// <param name="length">length of the name in bytes</param>
		/// <param name="propertyId">The property ID in this runtime for the given name.</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCreatePropertyIdUtf8(byte[] name, UIntPtr length, out JavaScriptPropertyId propertyId);

		/// <summary>
		///     Copies the name associated with the property ID into a buffer.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Requires an active script context.
		///     </para>
		///     <para>
		///         When size of the `buffer` is unknown,
		///         `buffer` argument can be nullptr.
		///         `length` argument will return the size needed.
		///     </para>
		/// </remarks>
		/// <param name="propertyId">The property ID to get the name of.</param>
		/// <param name="buffer">The buffer holding the name associated with the property ID, encoded as utf8</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <param name="written">Total number of characters written or to be written</param>
		/// <returns>
		///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
		/// </returns>
		JsErrorCode JsCopyPropertyIdUtf8(JavaScriptPropertyId propertyId, out byte[] buffer, UIntPtr bufferSize, out UIntPtr length);

	}
}