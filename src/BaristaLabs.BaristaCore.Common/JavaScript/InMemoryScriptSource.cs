using System;
using System.Threading.Tasks;

namespace BaristaLabs.BaristaCore.JavaScript
{
    public class InMemoryScriptSource : IScriptSource
    {
        private readonly IntPtr m_cookie;
        private readonly string m_description;
        private readonly string m_script;

        InMemoryScriptSource(string description, string script)
        {
            m_cookie = SourceContext.GetNextContextId();
            m_description = description;
            m_script = script;
        }

        public int Cookie
        {
            get
            {
                return (int)m_cookie;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
        }

        public Task<string> GetScriptAsync()
        {
            return Task.FromResult(m_script);
        }
    }
}
