namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the JavaScript "undefined" type and provides the one and only instance of that type.
    /// </summary>
    [Serializable]
    public sealed class Undefined : System.Runtime.Serialization.ISerializable
    {
        /// <summary>
        /// Creates a new Undefined instance.
        /// </summary>
        private Undefined()
        {
        }

        /// <summary>
        /// Creates a new Undefined instance -- used for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private Undefined(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Gets the one and only "undefined" instance.
        /// </summary>
        public static readonly Undefined Value = new Undefined();

        [Serializable]
        private class SerializationHelper : IObjectReference
        {
            public object GetRealObject(StreamingContext context)
            {
                return Value;
            }
        }

        /// <summary>
        /// Sets the SerializationInfo with information about the exception.
        /// </summary>
        /// <param name="info"> The SerializationInfo that holds the serialized object data about
        /// the exception being thrown. </param>
        /// <param name="context"> The StreamingContext that contains contextual information about
        /// the source or destination. </param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Save the object state.
            info.SetType(typeof(SerializationHelper));
        }

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        public override string ToString()
        {
            return "undefined";
        }
    }
}
