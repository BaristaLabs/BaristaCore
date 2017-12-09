namespace ChakraWrapperGeneratorCLI
{
    using ChakraWrapperGeneratorCLI.CodeGen;
    using HandlebarsDotNet;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            //Common type mappings
            var typeMap = new Dictionary<string, string>() {
                { "JsRuntimeHandle", "JavaScriptRuntimeSafeHandle" },
                { "JsContextRef", "JavaScriptContextSafeHandle" },
                { "JsSourceContext", "JavaScriptSourceContext" },
                { "JsRef", "SafeHandle" },
                { "JsWeakRef", "JavaScriptWeakReferenceSafeHandle" },
                { "JsValueRef", "JavaScriptValueSafeHandle" },
                { "JsValueRef[]", "IntPtr[]" },
                { "JsSharedArrayBufferContentHandle", "IntPtr" },

                { "JsPropertyIdRef", "JavaScriptPropertyIdSafeHandle" },
                { "JsPropertyIdType", "JavaScriptPropertyIdType" },
                { "JsModuleRecord", "JavaScriptModuleRecord" },
                { "JsParseModuleSourceFlags", "JavaScriptParseModuleSourceFlags" },
                { "JsModuleHostInfoKind", "JavaScriptModuleHostInfoKind" },

                { "JsRuntimeAttributes", "JavaScriptRuntimeAttributes"},
                { "JsParseScriptAttributes", "JavaScriptParseScriptAttributes"},
                { "JsValueType", "JavaScriptValueType"},
                { "JsTypedArrayType", "JavaScriptTypedArrayType"},
                { "JsDiagBreakOnExceptionAttributes", "JavaScriptDiagBreakOnExceptionAttributes"},
                { "JsDiagStepType", "JavaScriptDiagStepType"},

                { "JsNativeFunction", "JavaScriptNativeFunction"},

                { "JsSerializedLoadScriptCallback", "JavaScriptSerializedLoadScriptCallback" },
                { "JsSerializedScriptLoadSourceCallback", "JavaScriptSerializedScriptLoadSourceCallback" },
                { "JsSerializedScriptUnloadCallback", "JavaScriptSerializedScriptUnloadCallback" },
                { "JsFinalizeCallback", "JavaScriptObjectFinalizeCallback"},
                { "JsPromiseContinuationCallback", "JavaScriptPromiseContinuationCallback"},
                { "JsDiagDebugEventCallback", "JavaScriptDiagDebugEventCallback" },
                { "FetchImportedModuleCallBack", "JavaScriptFetchImportedModuleCallBack" },
                { "NotifyModuleReadyCallback", "JavaScriptNotifyModuleReadyCallback" },

                { "size_t", "ulong" },
                { "UIntPtr", "ulong" },
                { "char*", "byte[]"},
                { "uint16_t*", "byte[]" }
            };

            var paramNameTypeMap = new Dictionary<string, string>()
            {
                { "weakRef", "JavaScriptWeakReferenceSafeHandle" },
                //{ "sharedContents", "JavaScriptSharedArrayBufferSafeHandle" }
            };

            SetupHelpers();

            //Load the extern definition file.
            var defsDoc = XDocument.Load(args[0]);
            var externs = ParseAndGetExterns(typeMap, paramNameTypeMap, defsDoc);

            //Output Native Wrapper
            var templateContents = File.ReadAllText("./templates/LibChakraCore.hbs");
            var generator = Handlebars.Compile(templateContents);
            var libChakraCore = generator(new
            {
                DllName = "ChakraCore/libChakraCore",
                Externs = externs
            });

            File.WriteAllText("../../src/BaristaLabs.BaristaCore.Common/JavaScript/Internal/LibChakraCore.cs", libChakraCore);

            //Output Interfaces
            var interfaceTemplateContents = File.ReadAllText("./templates/ChakraTypedInterface.hbs");
            var interfaceGenerator = Handlebars.Compile(interfaceTemplateContents);

            GenerateInterface(interfaceGenerator, externs, "ChakraCommon.h", "ICommonJavaScriptEngine");
            GenerateInterface(interfaceGenerator, externs, "ChakraCommonWindows.h", "ICommonWindowsJavaScriptEngine");
            GenerateInterface(interfaceGenerator, externs, "ChakraCore.h", "ICoreJavaScriptEngine", e => e.Target == PlatformTarget.Common);
            GenerateInterface(interfaceGenerator, externs, "ChakraDebug.h", "IDebugJavaScriptEngine", e => e.Target == PlatformTarget.Common);
            GenerateInterface(interfaceGenerator, externs, "ChakraDebug.h", "IDebugWindowsJavaScriptEngine", e => e.Target == PlatformTarget.WindowsOnly);

            //Output Engine Implementations
            var implementationTemplateContents = File.ReadAllText("./templates/ChakraTypedSafeImplementation.hbs");
            var implementationGenerator = Handlebars.Compile(implementationTemplateContents);

            GenerateImplementation(implementationGenerator, externs, "public abstract", "ChakraEngineBase", "IJavaScriptEngine", e => e.Target == PlatformTarget.Common);
            GenerateImplementation(implementationGenerator, externs, "public sealed", "LinuxChakraEngine", "ChakraEngineBase", e => false);
            GenerateImplementation(implementationGenerator, externs, "public sealed", "WindowsChakraEngine", "ChakraEngineBase, ICommonWindowsJavaScriptEngine, IDebugWindowsJavaScriptEngine", e => e.Target == PlatformTarget.WindowsOnly);

            Console.WriteLine("Completed!");
        }

        public static void SetupHelpers()
        {
            Handlebars.RegisterHelper("trim", (writer, context, arguments) =>
            {
                if (arguments.Length < 1)
                {
                    throw new HandlebarsException("{{trim}} helper must have at least one argument");
                }

                var str = arguments[0].ToString();
                if (String.IsNullOrWhiteSpace(str))
                    return;

                writer.WriteSafeString(str.Trim());
            });

            Handlebars.RegisterHelper("breaklines", (writer, context, arguments) =>
            {
                if (arguments.Length < 1)
                {
                    throw new HandlebarsException("{{breaklines}} helper must have at least one argument");
                }

                var str = arguments[0].ToString();
                if (String.IsNullOrWhiteSpace(str))
                    return;

                var prefix = "        ///     ";
                if (arguments.Length >= 2)
                    prefix = arguments[1].ToString();

                var sb = new StringBuilder();
                var lines = str.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                for (int i = 0; i < lines.Length; i++)
                {
                    var ln = lines[i];
                    if (ln.Trim() == string.Empty)
                        continue;

                    if (i == lines.Length - 1)
                        sb.Append(prefix + ln);
                    else
                        sb.AppendLine(prefix + ln);
                }

                writer.WriteSafeString(sb.ToString());
            });
        }

        public static List<ChakraExtern> ParseAndGetExterns(IDictionary<string, string> typeMap, IDictionary<string, string> namedTypeMap, XDocument defsDoc)
        {
            var externs = new List<ChakraExtern>();

            foreach (var export in defsDoc.Descendants("Export"))
            {
                var def = new ChakraExtern
                {
                    Name = export.Attribute("name").Value,
                    Target = (PlatformTarget)Enum.Parse(typeof(PlatformTarget), export.Attribute("target").Value),
                    Source = export.Attribute("source").Value,
                };

                //Parse the Description
                var description = export.Element("Description").Value;
                Regex rx = new Regex(@"///\s*?(?<text>.*)", RegexOptions.ExplicitCapture);
                StringBuilder descriptionXmlBuilder = new StringBuilder();
                descriptionXmlBuilder.AppendLine("<description>");
                foreach (Match match in rx.Matches(description))
                {
                    var text = match.Groups["text"].Value.Trim();
                    descriptionXmlBuilder.AppendLine(text);
                }
                descriptionXmlBuilder.AppendLine("</description>");

                var descriptionXml = descriptionXmlBuilder.ToString();
                var descriptionXDoc = XDocument.Parse(descriptionXml);
                if (descriptionXDoc.Root.Element("summary") != null)
                    def.Summary = descriptionXDoc.Root.Element("summary").Value.Trim().Replace("\n", " ").Replace("\r\n", " ");
                if (descriptionXDoc.Root.Element("remarks") != null)
                    def.Remarks = descriptionXDoc.Root.Element("remarks").Value.Trim();
                if (descriptionXDoc.Root.Element("returns") != null)
                    def.ReturnParameter.Description = descriptionXDoc.Root.Element("returns").Value.Trim();
                Dictionary<string, string> paramDescriptions = new Dictionary<string, string>();
                foreach (var parm in descriptionXDoc.Root.Elements("param"))
                {
                    var paramName = parm.Attribute("name").Value.Trim();
                    var paramDescription = parm.Value.Trim();
                    paramDescriptions.Add(paramName, paramDescription);
                }

                if (export.Attribute("dllImportEx") != null)
                {
                    def.DllImportEx = export.Attribute("dllImportEx").Value;
                }

                foreach (var property in export.Element("Parameters").Descendants("Parameter"))
                {
                    var param = new ExternParameter()
                    {
                        Type = property.Attribute("type").Value,
                        Name = property.Attribute("name").Value
                    };

                    var paramName = param.Name.TrimStart('@');
                    if (paramDescriptions.ContainsKey(paramName))
                    {
                        param.Description = paramDescriptions[paramName];
                    }
                    else
                    {
                        param.Description = "NO DESCRIPTION PROVIDED";
                    }

                    if (typeMap.ContainsKey(param.Type))
                    {
                        param.Type = typeMap[param.Type];
                    }
                    else if (namedTypeMap.ContainsKey(param.Name))
                    {
                        param.Type = namedTypeMap[param.Name];
                    }

                    var directionAttribute = property.Attribute("direction");
                    if (directionAttribute != null)
                    {
                        param.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), directionAttribute.Value);
                    }

                    def.Parameters.Add(param);
                }

                externs.Add(def);
            }

            return externs;
        }

        public static void GenerateInterface(Func<object, string> interfaceGenerator, IList<ChakraExtern> externs, string sourceName, string interfaceName, Func<ChakraExtern, bool> predicate = null)
        {
            if (predicate == null)
            {
                predicate = new Func<ChakraExtern, bool>((e) => true);
            }

            var interfaceDefinition = interfaceGenerator(new
            {
                SourceName = sourceName,
                InterfaceName = interfaceName,
                InterfaceExterns = externs.Where(e => e.Source == sourceName && predicate(e)).Select(e => e.InterfaceExtern)
            });
            File.WriteAllText($"../../src/BaristaLabs.BaristaCore.Common/JavaScript/Interfaces/{interfaceName}.cs", interfaceDefinition);
        }

        public static void GenerateImplementation(Func<object, string> implementationGenerator, IList<ChakraExtern> externs, string accessModifier, string className, string interfaces, Func<ChakraExtern, bool> predicate = null)
        {
            if (predicate == null)
            {
                predicate = new Func<ChakraExtern, bool>((e) => true);
            }

            var implementation = implementationGenerator(new
            {
                AccessModifier = accessModifier,
                ClassName = className,
                Interfaces = interfaces,
                Externs = externs.Where(predicate)
            });
            File.WriteAllText($"../../src/BaristaLabs.BaristaCore.Common/JavaScript/Engines/{className}.cs", implementation);
        }
    }
}