namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Text;

    /// <summary>
    ///     A property identifier.
    /// </summary>
    /// <remarks>
    ///     Property identifiers are used to refer to properties of JavaScript objects instead of using strings.
    /// </remarks>
    public sealed class JavaScriptPropertyId : JavaScriptReferenceFlyweight<JavaScriptPropertyIdSafeHandle>
    {
        #region Properties
        /// <summary>
        ///     Gets the name associated with the property id.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        public string Name
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(JavaScriptPropertyId));

                //Get the size
                var size = Engine.JsCopyPropertyId(Handle, null, 0);
                if ((int)size > int.MaxValue)
                    throw new OutOfMemoryException("Exceeded maximum string length.");

                byte[] result = new byte[(int)size];
                var written = Engine.JsCopyPropertyId(Handle, result, (ulong)result.Length);
                return Encoding.UTF8.GetString(result, 0, result.Length);
            }
        }
        #endregion

        /// <summary>
        /// Creates a new JavaScript Property Id
        /// </summary>
        public JavaScriptPropertyId(IJavaScriptEngine engine, JavaScriptPropertyIdSafeHandle propertyIdHandle)
            : base(engine, propertyIdHandle)
        {
        }

        #region Object Overrides
        /// <summary>
        ///     Converts the property ID to a string.
        /// </summary>
        /// <returns>The name of the property ID.</returns>
        public override string ToString()
        {
            return Name;
        }
        #endregion

        #region Static Methods
        /// <summary>
        ///     Gets a property id object for the specified property name.
        /// </summary>
        /// <param name="engine">
        ///     The JavaScript engine to use.
        /// </param>
        /// <param name="name">
        ///     The name of the property.
        /// </param>
        /// <returns>The property ID in this runtime for the given name.</returns>
        public static JavaScriptPropertyId FromString(IJavaScriptEngine engine, string name)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var propertyHandle = engine.JsCreatePropertyId(name, (ulong)name.Length);
            return new JavaScriptPropertyId(engine, propertyHandle);
        }
        #endregion
    }
}
