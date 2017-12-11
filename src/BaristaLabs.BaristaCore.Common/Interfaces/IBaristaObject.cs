namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Represents an object that acts as a flyweight to a JavaScript reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaristaObject<T>: IDisposable
        where T : JavaScriptReference<T>
    {
        /// <summary>
        /// Event that is raised prior to the underlying runtime collecting the object.
        /// </summary>
        event EventHandler<BaristaObjectBeforeCollectEventArgs> BeforeCollect;

        /// <summary>
        /// Gets the JavaScript Engine associated with the JavaScript object.
        /// </summary>
        IJavaScriptEngine Engine
        {
            get;
        }

        /// <summary>
        /// Gets the underlying JavaScript Reference
        /// </summary>
        T Handle
        {
            get;
        }

        /// <summary>
        /// Gets a value that indicates if this reference has been disposed.
        /// </summary>
         bool IsDisposed
        {
            get;
        }
    }
}
