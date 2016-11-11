namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;
    public sealed class JavaScriptMemoryEventArgs
    {
        private bool m_cancelled;

        internal JavaScriptMemoryEventArgs(UIntPtr amount, JavaScriptMemoryEventType type)
        {
            Amount = amount;
            Type = type;
        }

        public UIntPtr Amount
        {
            get;
            private set;
        }

        public JavaScriptMemoryEventType Type
        {
            get;
            private set;
        }

        public bool Cancel
        {
            get { return m_cancelled; }
            set
            {
                // once one event listener cancels, it's cancelled.
                m_cancelled |= value;
            }
        }

        public bool IsCancelable
        {
            get { return Type == JavaScriptMemoryEventType.Allocate; }
        }
    }
}
