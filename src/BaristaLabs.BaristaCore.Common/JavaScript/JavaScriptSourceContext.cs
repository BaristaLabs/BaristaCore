namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    ///     A cookie that identifies a script for debugging purposes.
    /// </summary>
    [DebuggerStepThrough]
    public struct JavaScriptSourceContext : IEquatable<JavaScriptSourceContext>
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly IntPtr m_context;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JavaScriptSourceContext"/> struct.
        /// </summary>
        /// <param name="context">The context.</param>
        private JavaScriptSourceContext(IntPtr context)
        {
            m_context = context;
        }

        /// <summary>
        ///     Gets an empty source context.
        /// </summary>
        public static JavaScriptSourceContext None = new JavaScriptSourceContext(new IntPtr(-1));

        #region Operators
        /// <summary>
        ///     The equality operator for source contexts.
        /// </summary>
        /// <param name="left">The first source context to compare.</param>
        /// <param name="right">The second source context to compare.</param>
        /// <returns>Whether the two source contexts are the same.</returns>
        public static bool operator ==(JavaScriptSourceContext left, JavaScriptSourceContext right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     The inequality operator for source contexts.
        /// </summary>
        /// <param name="left">The first source context to compare.</param>
        /// <param name="right">The second source context to compare.</param>
        /// <returns>Whether the two source contexts are not the same.</returns>
        public static bool operator !=(JavaScriptSourceContext left, JavaScriptSourceContext right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Subtracts an offset from the value of the source context.
        /// </summary>
        /// <param name="context">The source context to subtract the offset from.</param>
        /// <param name="offset">The offset to subtract.</param>
        /// <returns>A new source context that reflects the subtraction of the offset from the context.</returns>
        public static JavaScriptSourceContext operator -(JavaScriptSourceContext context, int offset)
        {
            return FromIntPtr(context.m_context - offset);
        }

        /// <summary>
        ///     Subtracts an offset from the value of the source context.
        /// </summary>
        /// <param name="left">The source context to subtract the offset from.</param>
        /// <param name="right">The offset to subtract.</param>
        /// <returns>A new source context that reflects the subtraction of the offset from the context.</returns>
        public static JavaScriptSourceContext Subtract(JavaScriptSourceContext left, int right)
        {
            return left - right;
        }

        /// <summary>
        ///     Decrements the value of the source context.
        /// </summary>
        /// <param name="context">The source context to decrement.</param>
        /// <returns>A new source context that reflects the decrementing of the context.</returns>
        public static JavaScriptSourceContext operator --(JavaScriptSourceContext context)
        {
            return FromIntPtr(context.m_context - 1);
        }

        /// <summary>
        ///     Decrements the value of the source context.
        /// </summary>
        /// <param name="left">The source context to decrement.</param>
        /// <returns>A new source context that reflects the decrementing of the context.</returns>
        public static JavaScriptSourceContext Decrement(JavaScriptSourceContext left)
        {
            return --left;
        }

        /// <summary>
        ///     Adds an offset from the value of the source context.
        /// </summary>
        /// <param name="context">The source context to add the offset to.</param>
        /// <param name="offset">The offset to add.</param>
        /// <returns>A new source context that reflects the addition of the offset to the context.</returns>
        public static JavaScriptSourceContext operator +(JavaScriptSourceContext context, int offset)
        {
            return FromIntPtr(context.m_context + offset);
        }

        /// <summary>
        ///     Adds an offset from the value of the source context.
        /// </summary>
        /// <param name="left">The source context to add the offset to.</param>
        /// <param name="right">The offset to add.</param>
        /// <returns>A new source context that reflects the addition of the offset to the context.</returns>
        public static JavaScriptSourceContext Add(JavaScriptSourceContext left, int right)
        {
            return left + right;
        }

        /// <summary>
        ///     Increments the value of the source context.
        /// </summary>
        /// <param name="context">The source context to increment.</param>
        /// <returns>A new source context that reflects the incrementing of the context.</returns>
        public static JavaScriptSourceContext operator ++(JavaScriptSourceContext context)
        {
            return FromIntPtr(context.m_context + 1);
        }

        /// <summary>
        ///     Increments the value of the source context.
        /// </summary>
        /// <param name="left">The source context to increment.</param>
        /// <returns>A new source context that reflects the incrementing of the context.</returns>
        public static JavaScriptSourceContext Increment(JavaScriptSourceContext left)
        {
            return ++left;
        }

        public static explicit operator int(JavaScriptSourceContext context)
        {
            return (int)context.m_context;
        }
        #endregion

        /// <summary>
        ///     Creates a new source context. 
        /// </summary>
        /// <param name="cookie">
        ///     The cookie for the source context.
        /// </param>
        /// <returns>The new source context.</returns>
        public static JavaScriptSourceContext FromIntPtr(IntPtr cookie)
        {
            return new JavaScriptSourceContext(cookie);
        }

        /// <summary>
        ///     Creates a new source context. 
        /// </summary>
        /// <param name="cookie">
        ///     The cookie for the source context.
        /// </param>
        /// <returns>The new source context.</returns>
        public static JavaScriptSourceContext FromInt(int cookie)
        {
            return new JavaScriptSourceContext(new IntPtr(cookie));
        }

        #region Equality
        /// <summary>
        ///     Checks for equality between source contexts.
        /// </summary>
        /// <param name="other">The other source context to compare.</param>
        /// <returns>Whether the two source contexts are the same.</returns>
        public bool Equals(JavaScriptSourceContext other)
        {
            return m_context == other.m_context;
        }

        /// <summary>
        ///     Checks for equality between source contexts.
        /// </summary>
        /// <param name="obj">The other source context to compare.</param>
        /// <returns>Whether the two source contexts are the same.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is JavaScriptSourceContext && Equals((JavaScriptSourceContext)obj);
        }
        #endregion

        /// <summary>
        ///     The hash code.
        /// </summary>
        /// <returns>The hash code of the source context.</returns>
        public override int GetHashCode()
        {
            return m_context.ToInt32();
        }

        private static IntPtr s_sourceContextId = IntPtr.Zero;

        /// <summary>
        /// Gets the next unique source context.
        /// </summary>
        /// <returns></returns>
        public static JavaScriptSourceContext GetNextSourceContext()
        {
            while (true)
            {
                IntPtr currentSourceContextId = s_sourceContextId;
                IntPtr incremented = (s_sourceContextId + 1);

                Interlocked.CompareExchange(ref s_sourceContextId, incremented, currentSourceContextId);
                if (s_sourceContextId == incremented)
                {
                    return new JavaScriptSourceContext(currentSourceContextId);
                }
            }
        }
    }
}
