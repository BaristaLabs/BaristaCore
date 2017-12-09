namespace BaristaLabs.BaristaCore
{
    using System;

    public class BaristaRuntimeException : Exception
    {
        public BaristaRuntimeException(JsError error)
            : base(error.Message)
        {
            Line = error.Line;
            Column = error.Column;
            Length = error.Length;
            Source = error.Source;
        }

        public int Line
        {
            get;
            private set;
        }

        public int Column
        {
            get;
            private set;
        }

        public int Length
        {
            get;
            private set;
        }
    }
}
