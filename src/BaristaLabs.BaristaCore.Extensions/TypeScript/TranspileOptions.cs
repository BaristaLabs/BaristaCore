namespace BaristaLabs.BaristaCore.TypeScript
{
    using System.Collections.Generic;

    public class TranspileOptions
    {
        public TranspileOptions()
        {
            CompilerOptions = new CompilerOptions();
            RenamedDependencies = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the script that will be transpiled to JavaScript.
        /// </summary>
        public string Script
        {
            get;
            set;
        }

        public CompilerOptions CompilerOptions
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public bool? ReportDiagnostics
        {
            get;
            set;
        }

        public string ModuleName
        {
            get;
            set;
        }

        public IDictionary<string, string> RenamedDependencies
        {
            get;
            set;
        }

        //public CustomTransformers Transformers
        //{
        //    get;
        //    set;
        //}
    }
}
