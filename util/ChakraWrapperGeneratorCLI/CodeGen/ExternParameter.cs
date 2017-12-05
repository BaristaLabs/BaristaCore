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

        /// <summary>
        /// Gets a valud that indicates if the safe handle is "weak" and should not participate in reference counting.
        /// </summary>
        public bool IsWeakSafeHandle
        {
            get
            {
                return
                    Type == "JavaScriptRuntimeSafeHandle" ||
                    Type == "JavaScriptWeakReferenceSafeHandle" ||
                    Type == "JavaScriptSharedArrayBufferSafeHandle";
            }
        }

        public ParameterDirection Direction
        {
            get;
            set;
        }
    }
}
