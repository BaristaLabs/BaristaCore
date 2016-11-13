namespace BaristaLabs.BaristaCore.JavaScript
{
    using Interop;
    using System;

    public sealed class JavaScriptRuntimeSettings
    {
        private bool m_backgroundWork;
        private bool m_allowScriptInterrupt;
        private bool m_enableIdle;
        private bool m_disableNativeCode;
        private bool m_disableEval;
        private bool m_used;


        public JavaScriptRuntimeSettings()
        {

        }

        public bool DisableBackgroundWork
        {
            get { return m_backgroundWork; }
            set
            {
                if (m_used)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                m_backgroundWork = value;
            }
        }

        public bool AllowScriptInterrupt
        {
            get { return m_allowScriptInterrupt; }
            set
            {
                if (m_used)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                m_allowScriptInterrupt = value;
            }
        }

        public bool EnableIdle
        {
            get { return m_enableIdle; }
            set
            {
                if (m_used)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                m_enableIdle = value;
            }
        }

        public bool DisableNativeCode
        {
            get { return m_disableNativeCode; }
            set
            {
                if (m_used)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                m_disableNativeCode = value;
            }
        }

        public bool DisableEval
        {
            get { return m_disableEval; }
            set
            {
                if (m_used)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                m_disableEval = value;
            }
        }

        internal bool Used
        {
            get { return m_used; }
            set
            {
                m_used = value;
            }
        }

        internal JsRuntimeAttributes GetRuntimeAttributes()
        {
            var result = JsRuntimeAttributes.None;
            if (m_backgroundWork)
                result |= JsRuntimeAttributes.DisableBackgroundWork;
            if (m_allowScriptInterrupt)
                result |= JsRuntimeAttributes.AllowScriptInterrupt;
            if (m_enableIdle)
                result |= JsRuntimeAttributes.EnableIdleProcessing;
            if (m_disableNativeCode)
                result |= JsRuntimeAttributes.DisableNativeCodeGeneration;
            if (m_disableEval)
                result |= JsRuntimeAttributes.DisableEval;

            return result;
        }
    }
}
