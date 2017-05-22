namespace ChakraWrapperGeneratorCLI.CodeGen
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ChakraExtern
    {
        public string Name = "";
        public PlatformTarget Target = PlatformTarget.Common;
        public string Source = "";
        public string Summary = "";
        public string Remarks = "";

        public List<ExternParameter> Parameters = new List<ExternParameter>();
        public ExternParameter ReturnParameter = new ExternParameter()
        {
            Type = "JavaScriptErrorCode",
            Description = ""
        };
        public string DllImportEx = "";

        public Dictionary<string, string> InterfaceTypeMap = new Dictionary<string, string>() {
            { "JavaScriptRuntimeSafeHandle", "JavaScriptRuntimeSafeHandle" },
            { "JavaScriptContextSafeHandle", "JavaScriptContextSafeHandle" },
            { "JavaScriptValueSafeHandle", "JavaScriptValueSafeHandle" }
        };

        public ChakraExtern InterfaceExtern
        {
            get
            {
                var result = new ChakraExtern()
                {
                    Summary = Summary,
                    Remarks = Remarks,
                    Name = Name
                };

                var firstOutParameter = Parameters.FirstOrDefault(p => p.Direction == ParameterDirection.Out);
                var interfaceParameters = Parameters.Where(p => p != firstOutParameter).ToList();
                foreach (var interfaceParameter in interfaceParameters)
                {
                    var paramCopy = new ExternParameter();
                    paramCopy.Direction = interfaceParameter.Direction;
                    paramCopy.Name = interfaceParameter.Name;
                    paramCopy.Description = interfaceParameter.Description;

                    if (InterfaceTypeMap.ContainsKey(interfaceParameter.Type))
                        paramCopy.Type = InterfaceTypeMap[interfaceParameter.Type];
                    else
                        paramCopy.Type = interfaceParameter.Type;
                    result.Parameters.Add(paramCopy);
                }

                //Make the first out parameter the return parameter.
                result.ReturnParameter = new ExternParameter();
                if (firstOutParameter == null)
                {
                    result.ReturnParameter.Type = "void";
                    result.ReturnParameter.Name = "";
                    result.ReturnParameter.Description = "";
                }
                else
                {
                    if (InterfaceTypeMap.ContainsKey(firstOutParameter.Type))
                        result.ReturnParameter.Type = InterfaceTypeMap[firstOutParameter.Type];
                    else
                        result.ReturnParameter.Type = firstOutParameter.Type;

                    result.ReturnParameter.Name = firstOutParameter.Name;
                    result.ReturnParameter.Description = firstOutParameter.Description;
                }

                return result;
            }
        }

        public string Signature
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var p in Parameters)
                {
                    if (p.Direction == ParameterDirection.Out)
                        sb.Append("out ");
                    else if (p.Direction == ParameterDirection.Ref)
                        sb.Append("ref ");

                    sb.Append(p.Type);
                    sb.Append(" ");
                    sb.Append(p.Name);

                    if (p != Parameters[Parameters.Count - 1])
                        sb.Append(", ");
                }

                return sb.ToString();
            }
        }

        public string CallSignature
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var p in Parameters)
                {
                    if (p.Direction == ParameterDirection.Out)
                        sb.Append("out ");
                    else if (p.Direction == ParameterDirection.Ref)
                        sb.Append("ref ");

                    sb.Append(p.Name);

                    if (p != Parameters[Parameters.Count - 1])
                        sb.Append(", ");
                }

                return sb.ToString();
            }
        }

        public List<ExternParameter> GetOutValueSafeHandles()
        {
            return Parameters.FindAll(p => p.Direction == ParameterDirection.Out && (p.Type == "JavaScriptRuntimeSafeHandle" || p.Type == "JavaScriptContextSafeHandle" || p.Type == "JavaScriptValueSafeHandle" || p.Type == "JavaScriptPropertyIdSafeHandle"));
        }
    }
}
