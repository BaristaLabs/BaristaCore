namespace BaristaLabs.BaristaCore.JavaScript
{
    using Interfaces;
    using SafeHandles;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class JavaScriptConverter
    {
        private class JavaScriptProjection
        {
            public volatile int RefCount;
            public JavaScriptObject Prototype;
            public JavaScriptFunction Constructor;
            public bool HasStaticEvents;
            public bool HasInstanceEvents;
        }

        private WeakReference<JavaScriptContext> m_engine;
        private IChakraApi m_api;
        private Dictionary<Type, JavaScriptProjection> m_projectionTypes;
        private Dictionary<Type, Expression> m_eventMarshallers;

        public JavaScriptConverter(JavaScriptContext engine)
        {
            m_engine = new WeakReference<JavaScriptContext>(engine);
            m_api = engine.Api;
            m_projectionTypes = new Dictionary<Type, JavaScriptProjection>();
            m_eventMarshallers = new Dictionary<Type, Expression>();
        }

        private JavaScriptContext GetEngine()
        {
            JavaScriptContext result;
            if (!m_engine.TryGetTarget(out result))
                throw new ObjectDisposedException(nameof(JavaScriptContext));

            return result;
        }

        private JavaScriptContext GetEngineAndClaimContext()
        {
            var result = GetEngine();

            return result;
        }

        public bool ToBoolean(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();

            bool result;

            if (value.Type == JavaScriptValueType.Boolean)
            {
                Errors.ThrowIfIs(m_api.JsBooleanToBool(value.m_handle, out result));
            }
            else
            {
                JavaScriptValueSafeHandle tempBool;
                Errors.ThrowIfIs(m_api.JsConvertValueToBoolean(value.m_handle, out tempBool));
                using (tempBool)
                {
                    Errors.ThrowIfIs(m_api.JsBooleanToBool(tempBool, out result));
                }
            }

            return result;
        }

        public JavaScriptValue FromBoolean(bool value)
        {
            var eng = GetEngine();
            if (value)
                return eng.TrueValue;

            return eng.FalseValue;
        }

        public double ToDouble(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();

            double result;

            if (value.Type == JavaScriptValueType.Number)
            {
                Errors.ThrowIfIs(m_api.JsNumberToDouble(value.m_handle, out result));
            }
            else
            {
                JavaScriptValueSafeHandle tempVal;
                Errors.ThrowIfIs(m_api.JsConvertValueToNumber(value.m_handle, out tempVal));
                using (tempVal)
                {
                    Errors.ThrowIfIs(m_api.JsNumberToDouble(tempVal, out result));
                }
            }

            return result;
        }

        public JavaScriptValue FromDouble(double value)
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(m_api.JsDoubleToNumber(value, out result));

            return eng.CreateValueFromHandle(result);
        }

        public int ToInt32(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();

            int result;

            if (value.Type == JavaScriptValueType.Number)
            {
                Errors.ThrowIfIs(m_api.JsNumberToInt(value.m_handle, out result));
            }
            else
            {
                JavaScriptValueSafeHandle tempVal;
                Errors.ThrowIfIs(m_api.JsConvertValueToNumber(value.m_handle, out tempVal));
                using (tempVal)
                {
                    Errors.ThrowIfIs(m_api.JsNumberToInt(tempVal, out result));
                }
            }

            return result;
        }

        public JavaScriptValue FromInt32(int value)
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(m_api.JsIntToNumber(value, out result));

            return eng.CreateValueFromHandle(result);
        }

        public string ToString(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var m_apiWin = m_api as IChakraCommonWindows;
            if (m_apiWin == null)
                throw new InvalidOperationException("This operation only works on windows.");

            var eng = GetEngineAndClaimContext();
            if (value.Type == JavaScriptValueType.String)
            {
                IntPtr resultPtr;
                UIntPtr stringLength;
                Errors.ThrowIfIs(m_apiWin.JsStringToPointer(value.m_handle, out resultPtr, out stringLength));
                if ((int)stringLength > int.MaxValue)
                    throw new OutOfMemoryException("Exceeded maximum string length.");

                return Marshal.PtrToStringUni(resultPtr, (int)stringLength);
            }
            else if (value.Type == JavaScriptValueType.Symbol)
            {
                // Presently, JsRT doesn't have a way for the host to query the description of a Symbol
                // Using JsConvertValueToString resulted in putting the runtime into an exception state
                // Thus, detect the condition and just return a known string.

                return "(Symbol)";
            }
            else
            {
                JavaScriptValueSafeHandle tempStr;
                Errors.ThrowIfIs(m_api.JsConvertValueToString(value.m_handle, out tempStr));
                using (tempStr)
                {
                    IntPtr resultPtr;
                    UIntPtr stringLength;
                    Errors.ThrowIfIs(m_apiWin.JsStringToPointer(tempStr, out resultPtr, out stringLength));
                    if ((int)stringLength > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    return Marshal.PtrToStringUni(resultPtr, (int)stringLength);
                }
            }
        }

        public JavaScriptValue FromString(string value)
        {
            var eng = GetEngineAndClaimContext();

            var m_apiWin = m_api as IChakraCommonWindows;
            if (m_apiWin == null)
                throw new InvalidOperationException("This operation only works on windows.");

            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(m_apiWin.JsPointerToString(value, new UIntPtr((uint)value.Length), out result));

            return eng.CreateValueFromHandle(result);
        }

        public JavaScriptValue FromObject(object o)
        {
            var eng = GetEngine();
            if (o == null)
            {
                return eng.NullValue;
            }

            var jsVal = o as JavaScriptValue;
            if (jsVal != null)
                return jsVal;

            Type t = o.GetType();
            if (t == typeof(string))
            {
                return FromString((string)o);
            }
            else if (t == typeof(double) || t == typeof(float))
            {
                return FromDouble((double)o);
            }
            else if (t == typeof(int) || t == typeof(short) || t == typeof(ushort) || t == typeof(byte) || t == typeof(sbyte))
            {
                return FromInt32((int)o);
            }
            else if (t == typeof(uint))
            {
                return FromDouble((uint)o);
            }
            else if (t == typeof(long))
            {
                return FromDouble((long)o);
            }
            else if (t == typeof(bool))
            {
                bool b = (bool)o;
                return b ? eng.TrueValue : eng.FalseValue;
            }
            else if (t.GetTypeInfo().IsValueType)
            {
                throw new ArgumentException("Non-primitive value types may not be projected to JavaScript directly.  Use a JSON serializer to serialize the value.  For more information, see readme.md.");
            }
            else if (typeof(Task).GetTypeInfo().IsAssignableFrom(t))
            {
                // todo : project as Promise
                return eng.NullValue;
            }
            else if (typeof(Delegate).GetTypeInfo().IsAssignableFrom(t))
            {
                throw new ArgumentException("Use JavaScriptEngine.CreateFunction to marshal a delegate to JavaScript.");
            }
            else
            {
                var result = InitializeProjectionForObject(o);
                return result;
            }
        }

        public object ToObject(JavaScriptValue val)
        {
            switch (val.Type)
            {
                case JavaScriptValueType.Boolean:
                    return val.IsTruthy;
                case JavaScriptValueType.Number:
                    return ToDouble(val);
                case JavaScriptValueType.String:
                    return ToString(val);
                case JavaScriptValueType.Undefined:
                    return null;
                case JavaScriptValueType.Array:
                    JavaScriptArray arr = val as JavaScriptArray;
                    Debug.Assert(arr != null);

                    return arr.Select(v => ToObject(v)).ToArray();

                case JavaScriptValueType.ArrayBuffer:
                    var ab = val as JavaScriptArrayBuffer;
                    Debug.Assert(ab != null);

                    return ab.GetUnderlyingMemory();

                case JavaScriptValueType.DataView:
                    var dv = val as JavaScriptDataView;
                    Debug.Assert(dv != null);

                    return dv.GetUnderlyingMemory();

                case JavaScriptValueType.TypedArray:
                    var ta = val as JavaScriptTypedArray;
                    Debug.Assert(ta != null);

                    return ta.GetUnderlyingMemory();

                case JavaScriptValueType.Object:
                    var obj = val as JavaScriptObject;
                    var external = obj.ExternalObject;
                    return external ?? obj;

                // Unsupported marshaling types
                case JavaScriptValueType.Function:
                case JavaScriptValueType.Date:
                case JavaScriptValueType.Symbol:
                default:
                    throw new NotSupportedException("Unsupported type marshaling value from JavaScript to host: " + val.Type);
            }
        }

        internal JavaScriptObject GetProjectionPrototypeForType(Type t)
        {
            JavaScriptProjection baseTypeProjection;

            if (m_projectionTypes.TryGetValue(t, out baseTypeProjection))
            {
                Interlocked.Increment(ref baseTypeProjection.RefCount);
            }
            else
            {
                baseTypeProjection = InitializeProjectionForType(t);
                baseTypeProjection.RefCount++;
            }
            m_projectionTypes[t] = baseTypeProjection;

            return baseTypeProjection.Prototype;
        }

        private JavaScriptProjection InitializeProjectionForType(Type t)
        {
            var eng = GetEngineAndClaimContext();

            ObjectReflector reflector = ObjectReflector.Create(t);
            JavaScriptProjection baseTypeProjection = null;
            if (reflector.HasBaseType)
            {
                Type baseType = reflector.GetBaseType();
                if (!m_projectionTypes.TryGetValue(baseType, out baseTypeProjection))
                {
                    baseTypeProjection = InitializeProjectionForType(baseType);
                    baseTypeProjection.RefCount += 1;
                    m_projectionTypes[baseType] = baseTypeProjection;
                }
            }

            var publicConstructors = reflector.GetConstructors();
            var publicInstanceProperties = reflector.GetProperties(instance: true);
            var publicStaticProperties = reflector.GetProperties(instance: false);
            var publicInstanceMethods = reflector.GetMethods(instance: true);
            var publicStaticMethods = reflector.GetMethods(instance: false);
            var publicInstanceEvents = reflector.GetEvents(instance: true);
            var publicStaticEvents = reflector.GetEvents(instance: false);

            if (AnyHaveSameArity(publicConstructors, publicInstanceMethods, publicStaticMethods, publicInstanceProperties, publicStaticProperties))
                throw new InvalidOperationException("The specified type cannot be marshaled; some publicly accessible members have the same arity.  Projected methods can't differentiate only by type (e.g., Add(int, int) and Add(float, float) would cause this error).");

            JavaScriptFunction ctor;
            if (publicConstructors.Any())
            {
                // e.g. var MyObject = function() { [native code] };
                ctor = eng.CreateFunction((engine, constr, thisObj, args) =>
                {
                    // todo
                    return FromObject(publicConstructors.First().Invoke(new object[] { }));
                }, t.FullName);
            }
            else
            {
                ctor = eng.CreateFunction((engine, constr, thisObj, args) =>
                {
                    return eng.UndefinedValue;
                }, t.FullName);
            }

            // MyObject.prototype = Object.create(baseTypeProjection.PrototypeObject);
            var prototypeObj = CreateObjectFor(eng, baseTypeProjection);
            ctor.SetPropertyByName("prototype", prototypeObj);
            // MyObject.prototype.constructor = MyObject;
            prototypeObj.SetPropertyByName("constructor", ctor);

            // MyObject.CreateMyObject = function() { [native code] };
            ProjectMethods(t.FullName, ctor, eng, publicStaticMethods);
            // Object.defineProperty(MyObject, 'Foo', { get: function() { [native code] } });
            ProjectProperties(t.FullName, ctor, eng, publicStaticProperties);
            // MyObject.addEventListener = function() { [native code] };
            if ((baseTypeProjection?.HasStaticEvents ?? false) || publicStaticEvents.Any())
                ProjectEvents(t.FullName, ctor, eng, publicStaticEvents, baseTypeProjection, instance: false);

            // MyObject.prototype.ToString = function() { [native code] };
            ProjectMethods(t.FullName + ".prototype", prototypeObj, eng, publicInstanceMethods);
            // Object.defineProperty(MyObject.prototype, 'baz', { get: function() { [native code] }, set: function() { [native code] } });
            ProjectProperties(t.FullName + ".prototype", prototypeObj, eng, publicInstanceProperties);
            // MyObject.prototype.addEventListener = function() { [native code] };
            if ((baseTypeProjection?.HasInstanceEvents ?? false) || publicInstanceEvents.Any())
                ProjectEvents(t.FullName + ".prototype", prototypeObj, eng, publicInstanceEvents, baseTypeProjection, instance: true);

            prototypeObj.Freeze();

            return new JavaScriptProjection
            {
                RefCount = 0,
                Constructor = ctor,
                Prototype = prototypeObj,
                HasInstanceEvents = (baseTypeProjection?.HasInstanceEvents ?? false) || publicInstanceEvents.Any(),
                HasStaticEvents = (baseTypeProjection?.HasStaticEvents ?? false) || publicStaticEvents.Any(),
            };
        }

        // used by InitializeProjectionForType
        private JavaScriptObject CreateObjectFor(JavaScriptContext engine, JavaScriptProjection baseTypeProjection)
        {
            if (baseTypeProjection != null)
            {
                // todo: revisit to see if there is a better way to do this
                // Can probably get 
                // ((engine.GlobalObject.GetPropertyByName("Object") as JavaScriptObject).GetPropertyByName("create") as JavaScriptFunction).Invoke(baseTypeProjection.Prototype) 
                // but this is so much clearer
                dynamic global = engine.GlobalObject;
                JavaScriptObject result = global.Object.create(baseTypeProjection.Prototype);
                return result;
            }
            else
            {
                return engine.CreateObject();
            }
        }

        private void ProjectMethods(string owningTypeName, JavaScriptObject target, JavaScriptContext engine, IEnumerable<MethodInfo> methods)
        {
            var methodsByName = methods.GroupBy(m => m.Name);
            foreach (var group in methodsByName)
            {
                var method = engine.CreateFunction((eng, ctor, thisObj, args) =>
                {
                    var @this = thisObj as JavaScriptObject;
                    if (@this == null)
                    {
                        eng.SetException(eng.CreateTypeError("Could not call method '" + group.Key + "' because there was an invalid 'this' context."));
                        return eng.UndefinedValue;
                    }

                    var argsArray = args.ToArray();
                    var candidate = GetBestFitMethod(group, thisObj, argsArray);
                    if (candidate == null)
                    {
                        eng.SetException(eng.CreateReferenceError("Could not find suitable method or not enough arguments to invoke '" + group.Key + "'."));
                        return eng.UndefinedValue;
                    }

                    List<object> argsToPass = new List<object>();
                    for (int i = 0; i < candidate.GetParameters().Length; i++)
                    {
                        argsToPass.Add(ToObject(argsArray[i]));
                    }

                    try
                    {
                        return FromObject(candidate.Invoke(@this.ExternalObject, argsToPass.ToArray()));
                    }
                    catch (Exception ex)
                    {
                        eng.SetException(FromObject(ex));
                        return eng.UndefinedValue;
                    }
                }, owningTypeName + "." + group.Key);
                //var propDescriptor = engine.CreateObject();
                //propDescriptor.SetPropertyByName("configurable", engine.TrueValue);
                //propDescriptor.SetPropertyByName("enumerable", engine.TrueValue);
                //propDescriptor.SetPropertyByName("value", method);
                //target.DefineProperty(group.Key, propDescriptor);
                target.SetPropertyByName(group.Key, method);
            }
        }

        // todo: replace with a dynamic method thunk
        private MethodInfo GetBestFitMethod(IEnumerable<MethodInfo> methodCandidates, JavaScriptValue thisObj, JavaScriptValue[] argsArray)
        {
            JavaScriptObject @this = thisObj as JavaScriptObject;
            if (@this == null)
                return null;

            var external = @this.ExternalObject;
            if (external == null)
                return null;

            MethodInfo most = null;
            int arity = -1;
            foreach (var candidate in methodCandidates)
            {
                //if (candidate.DeclaringType != external.GetType())
                //    continue;

                var paramCount = candidate.GetParameters().Length;
                if (argsArray.Length == paramCount)
                {
                    return candidate;
                }
                else if (argsArray.Length < paramCount)
                {
                    if (paramCount > arity)
                    {
                        arity = paramCount;
                        most = candidate;
                    }
                }
            }

            return most;
        }

        private void ProjectProperties(string owningTypeName, JavaScriptObject target, JavaScriptContext engine, IEnumerable<PropertyInfo> properties)
        {
            foreach (var prop in properties)
            {
                if (prop.GetIndexParameters().Length > 0)
                    throw new NotSupportedException("Index properties not supported for projecting CLR to JavaScript objects.");

                JavaScriptFunction jsGet = null, jsSet = null;
                if (prop.GetMethod != null)
                {
                    jsGet = engine.CreateFunction((eng, ctor, thisObj, args) =>
                    {
                        var @this = thisObj as JavaScriptObject;
                        if (@this == null)
                        {
                            eng.SetException(eng.CreateTypeError("Could not retrieve property '" + prop.Name + "' because there was an invalid 'this' context."));
                            return eng.UndefinedValue;
                        }

                        try
                        {
                            return FromObject(prop.GetValue(@this.ExternalObject));
                        }
                        catch (Exception ex)
                        {
                            eng.SetException(FromObject(ex));
                            return eng.UndefinedValue;
                        }
                    }, owningTypeName + "." + prop.Name + ".get");
                }
                if (prop.SetMethod != null)
                {
                    jsSet = engine.CreateFunction((eng, ctor, thisObj, args) =>
                    {
                        var @this = thisObj as JavaScriptObject;
                        if (@this == null)
                        {
                            eng.SetException(eng.CreateTypeError("Could not retrieve property '" + prop.Name + "' because there was an invalid 'this' context."));
                            return eng.UndefinedValue;
                        }

                        try
                        {
                            var val = ToObject(args.First());
                            if (prop.PropertyType == typeof(int))
                            {
                                val = (int)(double)val;
                            }
                            prop.SetValue(@this.ExternalObject, val);
                            return eng.UndefinedValue;
                        }
                        catch (Exception ex)
                        {
                            eng.SetException(FromObject(ex));
                            return eng.UndefinedValue;
                        }
                    }, owningTypeName + "." + prop.Name + ".set");
                }

                var descriptor = engine.CreateObject();
                if (jsGet != null)
                    descriptor.SetPropertyByName("get", jsGet);
                if (jsSet != null)
                    descriptor.SetPropertyByName("set", jsSet);
                descriptor.SetPropertyByName("enumerable", engine.TrueValue);
                target.DefineProperty(prop.Name, descriptor);
            }
        }

        private static bool AnyHaveSameArity(params IEnumerable<MemberInfo>[] members)
        {
            foreach (IEnumerable<MemberInfo> memberset in members)
            {
                IEnumerable<ConstructorInfo> ctors = memberset as IEnumerable<ConstructorInfo>;
                IEnumerable<MethodInfo> methods = memberset as IEnumerable<MethodInfo>;
                IEnumerable<PropertyInfo> props = memberset as IEnumerable<PropertyInfo>;
                HashSet<int> arities = new HashSet<int>();

                if (ctors != null)
                {
                    foreach (var ctor in ctors)
                    {
                        int arity = ctor.GetParameters().Length;
                        if (arities.Contains(arity))
                            return true;
                        arities.Add(arity);
                    }
                }
                else if (methods != null)
                {
                    foreach (var methodGroup in methods.GroupBy(m => m.Name))
                    {
                        arities.Clear();
                        foreach (var method in methodGroup)
                        {
                            int arity = method.GetParameters().Length;
                            if (arities.Contains(arity))
                                return true;
                            arities.Add(arity);
                        }
                    }
                }
                else if (props != null)
                {
                    //foreach (var prop in props)
                    //{
                    //    int arity = prop.GetIndexParameters().Length;
                    //    if (arities.Contains(arity))
                    //        return true;
                    //    arities.Add(arity);
                    //}
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized member type");
                }
            }

            return false;
        }

        private void ProjectEvents(string owningTypeName, JavaScriptObject target, JavaScriptContext engine, IEnumerable<EventInfo> events, JavaScriptProjection baseTypeProjection, bool instance)
        {
            var eventsArray = events.ToArray();
            var eventsLookup = eventsArray.ToDictionary(ei => ei.Name.ToLower());
            // General approach here
            // if there is a base thing, invoke that
            // for each event, register a delegate that marshals it back to JavaScript
            var add = engine.CreateFunction((eng, ctor, thisObj, args) =>
            {
                bool callBase = instance && (baseTypeProjection?.HasInstanceEvents ?? false);
                var @this = thisObj as JavaScriptObject;
                if (@this == null)
                    return eng.UndefinedValue;

                if (callBase)
                {
                    var baseObj = baseTypeProjection.Prototype;
                    var baseFn = baseObj.GetPropertyByName("addEventListener") as JavaScriptFunction;
                    if (baseFn != null)
                    {
                        baseFn.Call(@this, args);
                    }
                }

                var argsArray = args.ToArray();
                if (argsArray.Length < 2)
                    return eng.UndefinedValue;

                string eventName = argsArray[0].ToString();
                JavaScriptFunction callbackFunction = argsArray[1] as JavaScriptFunction;
                if (callbackFunction == null)
                    return eng.UndefinedValue;

                EventInfo curEvent;
                if (!eventsLookup.TryGetValue(eventName, out curEvent))
                    return eng.UndefinedValue;

                MethodInfo targetMethod = curEvent.EventHandlerType.GetTypeInfo().GetMethod("Invoke");

                var paramsExpr = targetMethod.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                int cookie = EventMarshaler.RegisterDelegate(callbackFunction, SynchronizationContext.Current);

                var marshaler = Expression.Lambda(curEvent.EventHandlerType, Expression.Block(
                    Expression.Call(
                        typeof(EventMarshaler).GetTypeInfo().GetMethod(nameof(EventMarshaler.InvokeJavaScriptCallback)),
                        Expression.Constant(cookie),
                        Expression.NewArrayInit(typeof(string), targetMethod.GetParameters().Select(p => Expression.Constant(p.Name))),
                        Expression.NewArrayInit(typeof(object), paramsExpr))
                ), paramsExpr);

                curEvent.AddMethod.Invoke(@this.ExternalObject, new object[] { marshaler.Compile() });

                return eng.UndefinedValue;
            }, owningTypeName + ".addEventListener");
            target.SetPropertyByName("addEventListener", add);
        }

        private JavaScriptObject InitializeProjectionForObject(object target)
        {
            Type t = target.GetType();
            JavaScriptProjection projection;
            if (!m_projectionTypes.TryGetValue(t, out projection))
            {
                projection = InitializeProjectionForType(t);
            }

            Interlocked.Increment(ref projection.RefCount);
            // Avoid race condition in which m_projectionTypes has had projection removed
            m_projectionTypes[t] = projection;

            var eng = GetEngineAndClaimContext();
            var result = eng.CreateExternalObject(target, externalData =>
            {
                if (Interlocked.Decrement(ref projection.RefCount) <= 0)
                {
                    m_projectionTypes.Remove(t);
                }
            });
            result.Prototype = projection.Prototype;

            return result;
        }

        private abstract class ObjectReflector
        {
            public abstract IEnumerable<MethodInfo> GetMethods(bool instance);
            public abstract IEnumerable<PropertyInfo> GetProperties(bool instance);
            public abstract IEnumerable<ConstructorInfo> GetConstructors();
            public abstract IEnumerable<EventInfo> GetEvents(bool instance);
            public abstract IEnumerable<FieldInfo> GetFields(bool instance);

            public static ObjectReflector Create(Type t)
            {
                return (ObjectReflector)Activator.CreateInstance(typeof(ObjectReflector<>).MakeGenericType(t));
            }

            public abstract bool HasBaseType
            {
                get;
            }

            public abstract Type GetBaseType();

            public abstract bool IsDelegateType
            {
                get;
            }
        }

        private class ObjectReflector<T>
            : ObjectReflector
        {
            private static readonly Type s_type = typeof(T);
            private static readonly TypeInfo s_typeInfo = s_type.GetTypeInfo();

            public override IEnumerable<ConstructorInfo> GetConstructors()
            {
                return s_type.GetTypeInfo().GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            }

            public override IEnumerable<EventInfo> GetEvents(bool instance)
            {
                return s_typeInfo.DeclaredEvents.Where(e => (instance ? !e.AddMethod.IsStatic : e.AddMethod.IsStatic));
            }

            public override IEnumerable<FieldInfo> GetFields(bool instance)
            {
                // todo
                yield break;
            }

            public override IEnumerable<MethodInfo> GetMethods(bool instance)
            {
                return s_typeInfo.DeclaredMethods.Where(m => (instance ? !m.IsStatic : m.IsStatic) && !m.Attributes.HasFlag(MethodAttributes.SpecialName));
            }

            public override IEnumerable<PropertyInfo> GetProperties(bool instance)
            {
                return s_typeInfo.DeclaredProperties.Where(p => (instance ? !(p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic) : (p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic)));
            }

            public override bool HasBaseType
            {
                get
                {
                    return s_typeInfo.BaseType != null;
                }
            }

            public override Type GetBaseType()
            {
                return s_typeInfo.BaseType;
            }

            public override bool IsDelegateType
            {
                get
                {
                    return typeof(Delegate).GetTypeInfo().IsAssignableFrom(s_type);
                }
            }
        }
    }

    internal static class EventMarshaler
    {
        private static Dictionary<int, Tuple<JavaScriptFunction, SynchronizationContext>> eventRegistrations = new Dictionary<int, Tuple<JavaScriptFunction, SynchronizationContext>>();
        private static volatile int registrationTokens = 0;

        public static int RegisterDelegate(JavaScriptFunction callback, SynchronizationContext syncContext)
        {
            var cookie = Interlocked.Increment(ref registrationTokens);
            eventRegistrations[cookie] = Tuple.Create(callback, syncContext);
            return cookie;
        }

        public static void RemoveDelegate(int cookie)
        {
            eventRegistrations.Remove(cookie);
        }

        public static void InvokeJavaScriptCallback(int cookie, string[] names, object[] values)
        {
            Tuple<JavaScriptFunction, SynchronizationContext> registration;

            Debug.Assert(names != null);
            Debug.Assert(values != null);
            Debug.Assert(names.Length == values.Length);

            if (!eventRegistrations.TryGetValue(cookie, out registration))
                return;
            if (registration.Item2 == null) // synchronization context
            {
                var eng = registration.Item1.GetEngine();
                using (var context = eng.AcquireContext())
                {
                    var jsObj = eng.CreateObject();
                    for (int i = 0; i < names.Length; i++)
                    {
                        jsObj.SetPropertyByName(names[i], eng.Converter.FromObject(values[i]));
                    }

                    registration.Item1.Invoke(new[] { jsObj });
                }
            }
            else
            {
                registration.Item2.Post((s) =>
                {
                    var eng = registration.Item1.GetEngine();
                    using (var context = eng.AcquireContext())
                    {
                        var jsObj = eng.CreateObject();
                        for (int i = 0; i < names.Length; i++)
                        {
                            jsObj.SetPropertyByName(names[i], eng.Converter.FromObject(values[i]));
                        }

                        registration.Item1.Invoke(new[] { jsObj });
                    }
                }, null);
            }

        }
    }
}
