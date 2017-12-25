namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public sealed class BaristaTypeConversionStrategy : IBaristaTypeConversionStrategy
    {
        private const string BaristaObjectPropertyName = "__baristaObject";
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
            JsFunction fnCtor;

            var publicConstructors = reflector.GetConstructors();
            if (publicConstructors.Any())
            {
                var constructor = new BaristaFunctionDelegate((isConstructCall, thisObj, args) => {
                    if (!isConstructCall)
                    {
                        var ex = context.ValueFactory.CreateTypeError($"Failed to construct '{objectName}': Please use the 'new' operator, this object constructor cannot be called as a function.");
                        context.CurrentScope.SetException(ex);
                        return context.Undefined;
                    }

                    //Use Object.create to create an object bound to this prototype.
                    var jsObj = context.Object.Create(thisObj);

                    JsExternalObject externalObject = null;
                    if (args.Length == 1 && args[0] is JsExternalObject exObj)
                    {
                        externalObject = exObj;
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
                            var convertedArgs = ConvertArgsToParamTypes(args, constructorParams);

                            var newObj = bestConstructor.Invoke(convertedArgs);
                            externalObject = context.ValueFactory.CreateExternalObject(newObj);
                        }
                        catch (Exception ex)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateError(ex.Message));
                            return context.Undefined;
                        }
                    }

                    //TODO: This might work better as a symbol and/or non-enumerable and sealed.
                    jsObj.SetProperty(BaristaObjectPropertyName, externalObject);
                    return jsObj;
                });

                fnCtor = context.ValueFactory.CreateFunction(constructor, objectName);
            }
            else
            {
                var constructor = new BaristaFunctionDelegate((isConstructCall, thisObj, args) => {
                    var ex = context.ValueFactory.CreateTypeError($"Failed to construct '{objectName}': This object cannot be constructed.");
                    context.CurrentScope.SetException(ex);
                    return context.Undefined;
                });

                fnCtor = context.ValueFactory.CreateFunction(constructor, typeToConvert.Name);
            }

            if (superCtor != null && superCtor.Prototype != null)
            {
                fnCtor.Prototype = context.Object.Create(superCtor.Prototype);
                fnCtor.Prototype.Constructor = fnCtor;
            }

            var fnCtorPrototype = fnCtor.Prototype;

            //Project static properties onto the constructor.
            ProjectProperties(context, fnCtor, reflector.GetProperties(false));

            //Project static properties onto the constructor.
            ProjectProperties(context, fnCtorPrototype, reflector.GetProperties(true));

            //Project static methods onto the constructor.
            ProjectMethods(context, fnCtor, reflector, reflector.GetUniqueMethodsByName(false));

            //Project instance methods on to the constructor prototype;
            ProjectMethods(context, fnCtorPrototype, reflector, reflector.GetUniqueMethodsByName(true));

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

                var propertyName = BaristaPropertyAttribute.GetPropertyName(prop);
                var propertyDescriptor = context.ValueFactory.CreateObject();
                propertyDescriptor.SetProperty("enumerable", context.True);

                if (prop.GetMethod != null)
                {
                    var jsGet = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((isConstructCall, thisObj, args) =>
                    {
                        object targetObj = null;

                        if (thisObj == null)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not retrieve property '{propertyName}' because there was an invalid 'this' context."));
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
                    var jsSet = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((isConstructCall, thisObj, args) =>
                    {
                        object targetObj = null;

                        if (thisObj == null)
                        {
                            context.CurrentScope.SetException(context.ValueFactory.CreateTypeError($"Could not set property '{propertyName}' because there was an invalid 'this' context."));
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

                context.Object.DefineProperty(targetObject, context.ValueFactory.CreateString(propertyName), propertyDescriptor);
            }
        }

        private void ProjectMethods(BaristaContext context, JsObject targetObject, ObjectReflector reflector, IDictionary<string, IList<MethodInfo>> methods)
        {
            foreach(var method in methods)
            {
                var methodName = method.Key;
                var methodInfos = method.Value;

                var functionDescriptor = context.ValueFactory.CreateObject();
                functionDescriptor.SetProperty("enumerable", context.True);

                var fn = context.ValueFactory.CreateFunction(new BaristaFunctionDelegate((isConstructCall, thisObj, args) =>
                {
                    dynamic targetObj = null;

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
                        var convertedArgs = ConvertArgsToParamTypes(args, methodParams);

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

                functionDescriptor.SetProperty("value", fn);
                context.Object.DefineProperty(targetObject, context.ValueFactory.CreateString(methodName), functionDescriptor);
            }
        }

        private object[] ConvertArgsToParamTypes(object[] args, ParameterInfo[] parameters)
        {
            var convertedArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var currentParam = parameters[i];
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
            return convertedArgs;
        }
    }
}
