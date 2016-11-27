namespace BaristaLabs.BaristaCore.JavaScript
{
    using Internal;
    using System;
    using System.Text;

    /// <summary>
    ///     A property identifier.
    /// </summary>
    /// <remarks>
    ///     Property identifiers are used to refer to properties of JavaScript objects instead of using strings.
    /// </remarks>
    public sealed class JavaScriptPropertyId : IDisposable
    {
        private readonly IJavaScriptEngine m_javaScriptEngine;
        private JavaScriptPropertyIdSafeHandle m_javaScriptPropertyIdSafeHandle;

        #region Properties
        /// <summary>
        /// Gets the JavaScript Engine associated with the JavaScript Property Id.
        /// </summary>
        public IJavaScriptEngine Engine
        {
            get { return m_javaScriptEngine; }
        }

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
                var size = Engine.JsCopyPropertyIdUtf8(m_javaScriptPropertyIdSafeHandle, null, UIntPtr.Zero);
                if ((int)size > int.MaxValue)
                    throw new OutOfMemoryException("Exceeded maximum string length.");

                byte[] result = new byte[(int)size];
                var written = Engine.JsCopyPropertyIdUtf8(m_javaScriptPropertyIdSafeHandle, result, new UIntPtr((uint)result.Length));
                return Encoding.UTF8.GetString(result, 0, result.Length);
            }
        }

        /// <summary>
        /// Gets a value that indicates if this runtime has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_javaScriptPropertyIdSafeHandle == null;
            }
        }
        #endregion

        /// <summary>
        /// Private constructor. Property Ids are only creatable through the static factory method.
        /// </summary>
        private JavaScriptPropertyId(IJavaScriptEngine engine, JavaScriptPropertyIdSafeHandle propertyHandle)
        {
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            if (propertyHandle == null || propertyHandle == JavaScriptPropertyIdSafeHandle.Invalid)
                throw new ArgumentNullException(nameof(propertyHandle));

            m_javaScriptEngine = engine;
            m_javaScriptPropertyIdSafeHandle = propertyHandle;
        }

        #region IDisposable

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                //Dispose of the handle
                m_javaScriptPropertyIdSafeHandle.Dispose();
                m_javaScriptPropertyIdSafeHandle = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptPropertyId()
        {
            Dispose(false);
        }
        #endregion;

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

            var propertyHandle = engine.JsCreatePropertyIdUtf8(name, new UIntPtr((uint)name.Length));
            return new JavaScriptPropertyId(engine, propertyHandle);
        }
        #endregion
    }
}
