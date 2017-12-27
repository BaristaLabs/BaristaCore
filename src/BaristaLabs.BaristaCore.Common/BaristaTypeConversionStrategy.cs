namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public sealed class BaristaTypeConversionStrategy : IBaristaTypeConversionStrategy
    {
        private const string BaristaObjectPropertyName = "__baristaObject";
        private const string BaristaEventListenersPropertyName = "__baristaEventListeners";

        private IDictionary<Type, JsFunction> m_prototypes = new Dictionary<Type, JsFunction>();

        public bool TryCreatePrototypeFunction(BaristaContext context, Type typeToConvert, out JsFunction ctor)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (typeToConvert == null)
                throw new ArgumentNullException(nameof(typeToConvert));

            if (m_prototypes.ContainsKey(typeToConvert))
            {
                ctor = m_prototypes[typeToConvert];
                return true;
            }

            var reflector = new ObjectReflector(typeToConvert);

            JsFunction superCtor = null;
            var baseType = reflector.GetBaseType();
            if (baseType != null && !baseType.IsSameOrSubclass(typeof(JsValue)) && TryCreatePrototypeFunction(context, baseType, out JsFunction fnSuper))
            {
                superCtor = fnSuper;
            }

            var objectName = BaristaObjectAttribute.GetBaristaObjectNameFromType(typeToConvert);
            
            //Get all the property descriptors for the specified type.
            var staticPropertyDescriptors = context.ValueFactory.CreateObject();
            var instancePropertyDescriptors = context.ValueFactory.CreateObject();

            //Get static and instance properties.
            ProjectProperties(context, staticPropertyDescriptors, reflector.GetProperties(false));
            ProjectProperties(context, instancePropertyDescriptors, reflector.GetProperties(true));

            //Get static and instance methods.
            ProjectMethods(context, staticPropertyDescriptors, reflector, reflector.GetUniqueMethodsByName(false));
            ProjectMethods(context, instancePropertyDescriptors, reflector, reflector.GetUniqueMethodsByName(true));

            //Get static and instance events.
            ProjectEvents(context, staticPropertyDescriptors, reflector, reflector.GetEventTable(false));
            ProjectEvents(context, instancePropertyDescriptors, reflector, reflector.GetEventTable(true));

            JsFunction fnCtor;
            var publicConstructors = reflector.GetConstructors();
            if (publicConstructors.Any())
            {
                fnCtor = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                {
                    if (superCtor != null)
                    {
                        superCtor.Call(thisObj);
                    }

                    context.Object.DefineProperties(thisObj, instancePropertyDescriptors);

                    if (!isConstructCall)
                        return null;

                    //Set our native object.
                    JsExternalObject externalObject = null;

                    //!!Special condition -- if there's exactly one argument, and if it matches the enclosing type,
                    //don't invoke the type's constructor, rather, just wrap the object with the JsObject.
                    if (args.Length == 1 && args[0].GetType() == typeToConvert)
                    {
                        externalObject = context.ValueFactory.CreateExternalObject(args[0]);
                    }
                    else
                    {
                        try
                        {
                            var bestConstructor = reflector.GetConstructorBestMatch(args);
                            if (bestConstructor == null)
                            {
                                var ex = context.ValueFactory.CreateTypeError($"Failed to construct '{objectName}': Could not find a matching constructor for the provided arguments.");
                                context.CurrentScope.SetException(ex);
                                return context.Undefined;
                            }

                            //Convert the args into the native args of the constructor.
                            var constructorParams = bestConstructor.GetParameters();
                            var convertedArgs = ConvertArgsToParamTypes(context, args, constructorParams);

                            var newObj = bestConstructor.Invoke(convertedArgs);
                            externalObject = context.ValueFactory.CreateExternalObject(newObj);
                        }
                        catch (Exception ex)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateError(ex.Message));
                            return context.Undefined;
                        }
                    }

                    //Set the baristaObject as a non-configurable, non-enumerable, non-writable property
                    var baristaObjectPropertyDescriptor = context.ValueFactory.CreateObject();
                    baristaObjectPropertyDescriptor.SetProperty("value", externalObject);
                    context.Object.DefineProperty(thisObj, context.ValueFactory.CreateString(BaristaObjectPropertyName), baristaObjectPropertyDescriptor);

                    return null;
                }), objectName);
            }
            else
            {
                fnCtor = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                {
                    var ex = context.ValueFactory.CreateTypeError($"Failed to construct '{objectName}': This object cannot be constructed.");
                    context.CurrentScope.SetException(ex);
                    return context.Undefined;
                }), objectName);
            }

            //We've got everything we need.
            fnCtor.Prototype = context.Object.Create(superCtor == null ? context.Object.Prototype : superCtor.Prototype);

            context.Object.DefineProperties(fnCtor, staticPropertyDescriptors);

            m_prototypes.Add(typeToConvert, fnCtor);
            ctor = fnCtor;
            return true;
        }

        private void ProjectProperties(BaristaContext context, JsObject targetObject, IEnumerable<PropertyInfo> properties)
        {
            foreach (var prop in properties)
            {
                if (prop.GetIndexParameters().Length > 0)
                    throw new NotSupportedException("Index properties not supported for projecting CLR to JavaScript objects.");

                var propertyAttribute = BaristaPropertyAttribute.GetAttribute(prop);
                var propertyName = propertyAttribute.Name;
                var propertyDescriptor = context.ValueFactory.CreateObject();

                if (propertyAttribute.Configurable)
                    propertyDescriptor.SetProperty("configurable", context.True);
                if (propertyAttribute.Enumerable)
                    propertyDescriptor.SetProperty("enumerable", context.True);

                if (prop.GetMethod != null)
                {
                    var jsGet = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                    {
                        object targetObj = null;

                        if (thisObj == null)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not retrieve property '{propertyName}' because there was an invalid 'this' context."));
                            return context.Undefined;
                        }

                        //TODO: Migrate to this.
                        //if (thisObj.Prototype is JsExternalObject xoObj)
                        //{
                        //    targetObj = xoObj.Target;
                        //}

                        //If the property exists we're probably an instance -- though we should find a way to check this better.
                        if (thisObj.HasProperty(BaristaObjectPropertyName))
                        {
                            var xoObj = thisObj.GetProperty<JsExternalObject>(BaristaObjectPropertyName);
                            targetObj = xoObj.Target;
                        }

                        try
                        {
                            var result = prop.GetValue(targetObj);
                            if (context.Converter.TryFromObject(context, result, out JsValue resultValue))
                            {
                                return resultValue;
                            }
                            else
                            {
                                return context.Undefined;
                            }
                        }
                        catch (Exception ex)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateError(ex.Message));
                            return context.Undefined;
                        }
                    }));

                    propertyDescriptor.SetProperty("get", jsGet);
                }

                if (prop.SetMethod != null)
                {
                    var jsSet = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                    {
                        object targetObj = null;

                        if (thisObj == null)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not set property '{propertyName}' because there was an invalid 'this' context."));
                            return context.Undefined;
                        }

                        //TODO: Migrate to this.
                        //if (thisObj.Prototype is JsExternalObject xoObj)
                        //{
                        //    targetObj = xoObj.Target;
                        //}

                        //If the property exists we're probably an instance -- though we should find a way to check this better.
                        if (thisObj.HasProperty(BaristaObjectPropertyName))
                        {
                            var xoObj = thisObj.GetProperty<JsExternalObject>(BaristaObjectPropertyName);
                            targetObj = xoObj.Target;
                        }

                        try
                        {
                            var value = Convert.ChangeType(args.ElementAtOrDefault(0), prop.SetMethod.GetParameters().First().ParameterType);
                            prop.SetValue(targetObj, value);
                            return context.Undefined;
                        }
                        catch (Exception ex)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateError(ex.Message));
                            return context.Undefined;
                        }
                    }));

                    propertyDescriptor.SetProperty("set", jsSet);
                }

                //context.Object.DefineProperty(targetObject, context.ValueFactory.CreateString(propertyName), propertyDescriptor);
                targetObject.SetProperty(context.ValueFactory.CreateString(propertyName), propertyDescriptor);
            }
        }

        private void ProjectMethods(BaristaContext context, JsObject targetObject, ObjectReflector reflector, IDictionary<string, IList<MethodInfo>> methods)
        {
            foreach(var method in methods)
            {
                var methodName = method.Key;
                var methodInfos = method.Value;

                var fn = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                {
                    object targetObj = null;

                    if (thisObj == null)
                    {
                        context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not call function '{methodName}' because there was an invalid 'this' context."));
                        return context.Undefined;
                    }

                    //If the property exists we're probably an instance -- though we should find a way to check this better.
                    if (thisObj.HasProperty(BaristaObjectPropertyName))
                    {
                        var xoObj = thisObj.GetProperty<JsExternalObject>(BaristaObjectPropertyName);
                        targetObj = xoObj.Target;
                    }

                    try
                    {
                        var bestMethod = reflector.GetMethodBestMatch(methodInfos, args);
                        if (bestMethod == null)
                        {
                            var ex = context.ValueFactory.CreateTypeError($"Failed to call function '{methodName}': Could not find a matching function for the provided arguments.");
                            context.CurrentScope.SetException(ex);
                            return context.Undefined;
                        }

                        //Convert the args into the native args of the method.
                        var methodParams = bestMethod.GetParameters();
                        var convertedArgs = ConvertArgsToParamTypes(context, args, methodParams);

                        var result = bestMethod.Invoke(targetObj, convertedArgs);
                        if (context.Converter.TryFromObject(context, result, out JsValue resultValue))
                        {
                            return resultValue;
                        }
                        else
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"The call to '{methodName}' was successful, but the result could not be converted into a JavaScript object."));
                            return context.Undefined;
                        }
                    }
                    catch (Exception ex)
                    {
                        context.CurrentScope.SetException(context.ValueFactory.CreateError(ex.Message));
                        return context.Undefined;
                    }

                }));

                var functionDescriptor = context.ValueFactory.CreateObject();

                if (methodInfos.All(mi => BaristaPropertyAttribute.GetAttribute(mi).Configurable))
                    functionDescriptor.SetProperty("configurable", context.True);
                if (methodInfos.All(mi => BaristaPropertyAttribute.GetAttribute(mi).Enumerable))
                    functionDescriptor.SetProperty("enumerable", context.True);
                if (methodInfos.All(mi => BaristaPropertyAttribute.GetAttribute(mi).Writable))
                    functionDescriptor.SetProperty("writable", context.True);

                functionDescriptor.SetProperty("value", fn);

                //context.Object.DefineProperty(targetObject, context.ValueFactory.CreateString(methodName), functionDescriptor);
                targetObject.SetProperty(context.ValueFactory.CreateString(methodName), functionDescriptor);
            }
        }

        private void ProjectEvents(BaristaContext context, JsObject targetObject, ObjectReflector reflector, IDictionary<string, EventInfo> eventsTable)
        {
            if (eventsTable.Count == 0)
                return;

            var fnAddListener = context.ValueFactory.CreateFunction(new Func<JsObject, string, JsFunction, JsValue>((thisObj, eventName, fnCallback) => {

                if (String.IsNullOrWhiteSpace(eventName))
                {
                    context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"The name of the event to register must be specified."));
                    return context.Undefined;
                }

                object targetObj = null;

                if (thisObj == null)
                {
                    context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not register event '{eventName}' because there was an invalid 'this' context."));
                    return context.Undefined;
                }

                //If the property exists we're probably an instance -- though we should find a way to check this better.
                if (thisObj.HasProperty(BaristaObjectPropertyName))
                {
                    var xoObj = thisObj.GetProperty<JsExternalObject>(BaristaObjectPropertyName);
                    targetObj = xoObj.Target;
                }

                if (!eventsTable.TryGetValue(eventName, out EventInfo targetEvent))
                    return context.False;

                Action<object[]> invokeListener = (args) =>
                {
                    //TODO: Object conversion.
                    fnCallback.Call(thisObj, null);
                };

                var targetEventMethod = targetEvent.EventHandlerType.GetMethod("Invoke");
                var targetEventParameters = targetEventMethod.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

                var exprInvokeListener = Expression.Lambda(targetEvent.EventHandlerType, Expression.Block(
                    Expression.Call(
                        Expression.Constant(invokeListener.Target),
                        invokeListener.Method,
                        Expression.NewArrayInit(typeof(object), targetEventParameters))
                ), targetEventParameters);

                var invokeListenerDelegate = exprInvokeListener.Compile();

                IDictionary<string, IList<Delegate>> eventListeners;
                if (thisObj.HasProperty(BaristaEventListenersPropertyName))
                {
                    var xoListeners = thisObj.GetProperty<JsExternalObject>(BaristaEventListenersPropertyName);
                    eventListeners = xoListeners.Target as IDictionary<string, IList<Delegate>>;
                }
                else
                {
                    eventListeners = new Dictionary<string, IList<Delegate>>();

                    //Set the listeners as a non-configurable, non-enumerable, non-writable property
                    var xoListeners = context.ValueFactory.CreateExternalObject(eventListeners);

                    var baristaEventListenersPropertyDescriptor = context.ValueFactory.CreateObject();
                    baristaEventListenersPropertyDescriptor.SetProperty("value", xoListeners);
                    context.Object.DefineProperty(thisObj, context.ValueFactory.CreateString(BaristaEventListenersPropertyName), baristaEventListenersPropertyDescriptor);
                }

                if (eventListeners != null)
                {
                    if (eventListeners.ContainsKey(eventName))
                        eventListeners[eventName].Add(invokeListenerDelegate);
                    else
                        eventListeners.Add(eventName, new List<Delegate>() { invokeListenerDelegate });
                }
                
                targetEvent.AddMethod.Invoke(targetObj, new object[] { invokeListenerDelegate });

                return context.True;
            }), "on");

            var fnRemoveAllListeners = context.ValueFactory.CreateFunction(new Func<JsObject, string, JsValue>((thisObj, eventName) => {

                if (String.IsNullOrWhiteSpace(eventName))
                {
                    context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"The name of the event to remove must be specified."));
                    return context.Undefined;
                }

                object targetObj = null;

                if (thisObj == null)
                {
                    context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not unregister event '{eventName}' because there was an invalid 'this' context."));
                    return context.Undefined;
                }

                //If the property exists we're probably an instance -- though we should find a way to check this better.
                if (thisObj.HasProperty(BaristaObjectPropertyName))
                {
                    var xoObj = thisObj.GetProperty<JsExternalObject>(BaristaObjectPropertyName);
                    targetObj = xoObj.Target;
                }

                if (!eventsTable.TryGetValue(eventName, out EventInfo targetEvent))
                    return context.Undefined;

                //Get the event listeners.
                IDictionary<string, IList<Delegate>> eventListeners = null;
                if (thisObj.HasProperty(BaristaEventListenersPropertyName))
                {
                    var xoListeners = thisObj.GetProperty<JsExternalObject>(BaristaEventListenersPropertyName);
                    eventListeners = xoListeners.Target as IDictionary<string, IList<Delegate>>;
                }

                if (eventListeners == null)
                    return context.False;

                if (eventListeners.ContainsKey(eventName))
                {
                    foreach(var listener in eventListeners[eventName])
                    {
                        targetEvent.RemoveMethod.Invoke(targetObj, new object[] { listener });
                    }

                    eventListeners.Remove(eventName);
                }

                return context.True;
            }), "removeAllListeners");

            var onFunctionDescriptor = context.ValueFactory.CreateObject();
            onFunctionDescriptor.SetProperty("enumerable", context.True);
            onFunctionDescriptor.SetProperty("value", fnAddListener);
            targetObject.SetProperty(context.ValueFactory.CreateString("on"), onFunctionDescriptor);

            var offFunctionDescriptor = context.ValueFactory.CreateObject();
            offFunctionDescriptor.SetProperty("enumerable", context.True);
            offFunctionDescriptor.SetProperty("value", fnRemoveAllListeners);
            targetObject.SetProperty(context.ValueFactory.CreateString("removeAllListeners"), offFunctionDescriptor);
        }

        private object[] ConvertArgsToParamTypes(BaristaContext context, object[] args, ParameterInfo[] parameters)
        {
            var convertedArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var currentParam = parameters[i];
                if (currentParam.ParameterType == typeof(BaristaContext))
                {
                    convertedArgs[i] = context;
                }
                else
                {
                    var arg = args.ElementAtOrDefault(i);
                    try
                    {
                        convertedArgs[i] = Convert.ChangeType(args[i], currentParam.ParameterType);
                    }
                    catch (Exception)
                    {
                        //Something went wrong, use the default value.
                        convertedArgs[i] = currentParam.ParameterType.GetDefaultValue();
                    }
                }
            }
            return convertedArgs;
        }
    }
}
