namespace ChakraWrapperGeneratorCLI.CodeGen
{
    public class ExternParameter
    {
        public ExternParameter()
        {
            this.Direction = ParameterDirection.In;
        }

        public string Type
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool IsVoid
        {
            get
            {
                return Type == "void";
            }
        }
        public bool IsJavaScriptRuntimeSafeHandle
        {
            get
            {
                return Type == "JavaScriptRuntimeSafeHandle";
            }
        }

        public ParameterDirection Direction
        {
            get;
            set;
        }
    }
}
