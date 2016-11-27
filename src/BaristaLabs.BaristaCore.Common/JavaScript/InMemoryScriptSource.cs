namespace BaristaLabs.BaristaCore.JavaScript
{
    using System.Threading.Tasks;

    public class InMemoryScriptSource : IScriptSource
    {
        private readonly JavaScriptSourceContext m_cookie;
        private readonly string m_description;
        private readonly string m_script;

        InMemoryScriptSource(string description, string script)
        {
            m_cookie = JavaScriptSourceContext.GetNextSourceContext();
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
