using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaristaLabs.BaristaCore.JavaScript
{
    public class JavaScriptException
    {
        public string Message
        {
            get;
            set;
        }

        public int Line
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public string Stack
        {
            get;
            set;
        }

        public string Source
        {
            get;
            set;
        }
    }
}
