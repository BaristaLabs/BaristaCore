namespace BaristaLabs.BaristaCore.JavaScript
{
    using Interfaces;
    using SafeHandles;
    using System;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq.Expressions;

    public class JavaScriptValue : DynamicObject, IDisposable
    {
        internal JavaScriptValueSafeHandle m_handle;
        internal JavaScriptValueType m_type;
        internal WeakReference<JavaScriptContext> m_engine;
        internal IChakraApi m_api;

        internal JavaScriptContext GetEngine()
        {
            JavaScriptContext result;
            if (!m_engine.TryGetTarget(out result))
                throw new ObjectDisposedException(nameof(JavaScriptContext));

            return result;
        }

        internal JavaScriptValue(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptContext engine)
        {
            Debug.Assert(handle != null);
            Debug.Assert(engine != null);
            Debug.Assert(Enum.IsDefined(typeof(JavaScriptValueType), type));
            handle.SetEngine(engine);
            m_api = engine.Api;

            uint count;
            Errors.ThrowIfIs(m_api.JsAddRef(handle.DangerousGetHandle(), out count));

            m_handle = handle;
            m_type = type;
            m_engine = new WeakReference<JavaScriptContext>(engine);
        }

        public override string ToString()
        {
            var engine = GetEngine();
            return engine.Converter.ToString(this);
        }

        public JavaScriptValueType Type
        {
            get { return m_type; }
        }

        public bool IsTruthy
        {
            get
            {
                var engine = GetEngine();
                return engine.Converter.ToBoolean(this);
            }
        }

        public bool SimpleEquals(JavaScriptValue other)
        {
            var eng = GetEngine();
            bool result;
            Errors.ThrowIfIs(m_api.JsEquals(m_handle, other.m_handle, out result));

            return result;
        }

        public bool StrictEquals(JavaScriptValue other)
        {
            var eng = GetEngine();
            bool result;
            Errors.ThrowIfIs(m_api.JsStrictEquals(m_handle, other.m_handle, out result));

            return result;
        }

        #region DynamicObject overrides

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var eng = GetEngine();
            if (binder.Type == typeof(int))
            {
                result = eng.Converter.ToInt32(this);
                return true;
            }
            else if (binder.Type == typeof(double))
            {
                result = eng.Converter.ToDouble(this);
                return true;
            }
            else if (binder.Type == typeof(string))
            {
                result = eng.Converter.ToString(this);
                return true;
            }
            else if (binder.Type == typeof(bool))
            {
                result = eng.Converter.ToBoolean(this);
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            var eng = GetEngine();

            switch (binder.Operation)
            {
                case ExpressionType.IsFalse:
                    result = !IsTruthy;
                    return true;
                case ExpressionType.IsTrue:
                    result = IsTruthy;
                    return true;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    switch (Type)
                    {
                        case JavaScriptValueType.Number:
                            double n = eng.Converter.ToDouble(this);
                            result = -n;
                            return true;

                        case JavaScriptValueType.Boolean:
                            if (IsTruthy)
                                result = -1;
                            else
                                result = -0;
                            return true;

                            // TODO
                            // case JavaScriptValueType.String:
                    }

                    result = double.NaN;
                    return true;

                case ExpressionType.UnaryPlus:
                    switch (Type)
                    {
                        case JavaScriptValueType.Number:
                            result = eng.Converter.ToDouble(this);
                            return true;

                        case JavaScriptValueType.Boolean:
                            if (IsTruthy)
                                result = 1;
                            else
                                result = 0;

                            return true;
                    }

                    result = double.NaN;
                    return true;
            }

            return base.TryUnaryOperation(binder, out result);
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_handle != null)
                {
                    m_handle.Dispose();
                    m_handle = null;
                }
            }
        }

        ~JavaScriptValue()
        {
            Dispose(false);
        }
        #endregion
    }
}
