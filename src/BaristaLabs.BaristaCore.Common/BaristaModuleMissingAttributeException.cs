namespace BaristaLabs.BaristaCore
{
    using System;

    public class BaristaModuleMissingAttributeException : Exception
    {
        public BaristaModuleMissingAttributeException(string message = "A type that implements IBaristaModule is expected to be decorated with an BaristaModuleAttribute.")
            :base(message)
        {
        }
    }
}
