namespace BaristaLabs.BaristaCore.Portafilter
{
    using System;
    using System.Net;

    /// <summary>
    /// Represents a dummy proxy used by RestClient
    /// </summary>
    internal class DummyProxy : IWebProxy
    {
        private static DummyProxy s_defaultDummyProxy = new DummyProxy();

        public ICredentials Credentials
        {
            get;
            set;
        }

        public Uri GetProxy(Uri destination)
        {
            return null;
        }

        public bool IsBypassed(Uri host)
        {
            return true;
        }

        public static DummyProxy GetDefaultProxy()
        {
            return s_defaultDummyProxy;
        }
    }
}
