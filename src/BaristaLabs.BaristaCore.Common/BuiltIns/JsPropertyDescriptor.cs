namespace BaristaLabs.BaristaCore
{
    public class JsPropertyDescriptor
    {
        public bool Configurable
        {
            get;
            set;
        }

        public bool Enumerable
        {
            get;
            set;
        }

        public bool Writable
        {
            get;
            set;
        }

        public JsFunction Get
        {
            get;
            set;
        }

        public JsFunction Set
        {
            get;
            set;
        }

        public JsValue Value
        {
            get;
            set;
        }

        public JsObject GetDescriptorObject(BaristaContext context)
        {
            var descriptor = context.CreateObject();
            if (Configurable)
                descriptor.SetProperty("configurable", context.True);
            if (Enumerable)
                descriptor.SetProperty("enumerable", context.True);
            if (Writable)
                descriptor.SetProperty("enumerable", context.True);

            if (Get != null)
                descriptor.SetProperty("get", Get);

            if (Set != null)
                descriptor.SetProperty("set", Set);

            if (Value != null)
                descriptor.SetProperty("value", Value);

            return descriptor;
        }
    }
}
