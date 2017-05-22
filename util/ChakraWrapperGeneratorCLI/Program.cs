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
        { "JsValueRef", "JavaScriptValueSafeHandle" },
        { "JsValueRef[]", "IntPtr[]" },
        { "JsPropertyIdRef", "JavaScriptPropertyIdSafeHandle" },
        { "JsPropertyIdType", "JavaScriptPropertyIdType" },
        { "JsModuleRecord", "IntPtr" },
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

            var defsDoc = XDocument.Load(args[0]);
            var externs = ParseAndGetExterns(typeMap, defsDoc);

            Handlebars.RegisterHelper("breaklines", (writer, context, arguments) => {
                if (arguments.Length < 1)
                {
                    throw new HandlebarsException("{{breaklines}} helper must have at least one argument");
                }

                var str = arguments[0].ToString();
                if (String.IsNullOrWhiteSpace(str))
                    return;

                var prefix = "\t\t///\t\t";
                if (arguments.Length >= 2)
                    prefix = arguments[1].ToString();

                var sb = new StringBuilder();
                foreach (var ln in str.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    if (ln.Trim() == string.Empty)
                        continue;
                    sb.AppendLine(prefix + ln);
                }

                writer.WriteSafeString(sb.ToString());
            });

            var templateContents = File.ReadAllText("./templates/LibChakraCore.hbs");
            var generator = Handlebars.Compile(templateContents);
            var result = generator(new {
                Externs = externs
            });
            Console.WriteLine(result);
        }


        public static List<ChakraExtern> ParseAndGetExterns(IDictionary<string, string> typeMap, XDocument defsDoc)
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

                    var directionAttribute = property.Attribute("direction");
                    if (directionAttribute != null)
                    {
                        param.Direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), directionAttribute.Value);
                    }

                    def.Parameters.Add(param);
                }

                Console.WriteLine(def.Parameters.Count);
                if (def.Parameters == null || def.Parameters.Count == 0)
                {
                    Console.WriteLine("asdf");
                }
                externs.Add(def);
            }

            return externs;
        }
    }
}