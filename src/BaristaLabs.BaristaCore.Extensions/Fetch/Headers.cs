namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Generic;

    public sealed class Headers
    {
        private readonly BaristaContext m_context;

        public Headers(BaristaContext context)
        {
            m_context = context ?? throw new ArgumentNullException(nameof(context));
            AllHeaders = new Dictionary<string, IList<string>>();
        }

        /// <summary>
        /// Gets the dictionary of headers.
        /// </summary>
        [BaristaIgnore]
        public IDictionary<string, IList<string>> AllHeaders
        {
            get;
            private set;
        }

        public void Append(string name, string value)
        {
            if (AllHeaders.ContainsKey(name))
            {
                AllHeaders[name].Add(value);
            }
            else
            {
                AllHeaders.Add(name, new List<string>() { value });
            }
        }

        public void Delete(string name)
        {
            AllHeaders.Remove(name);
        }

        public IEnumerable<string[]> Entries()
        {
            foreach (var key in AllHeaders.Keys)
            {
                yield return new string[] { key, Get(key) };
            }
        }

        public string Get(string name)
        {
            if (AllHeaders.ContainsKey(name))
            {
                return String.Join(", ", AllHeaders[name]);
            }

            return null;
        }

        public bool Has(string name)
        {
            return AllHeaders.ContainsKey(name);
        }

        public IEnumerable<string> Keys(string name)
        {
            return AllHeaders.Keys;
        }

        public void Set(string name, string value)
        {
            if (!AllHeaders.ContainsKey(name))
            {
                Append(name, value);
            }

            AllHeaders[name].Add(value);
        }

        public IEnumerable<string> Values()
        {
            foreach (var key in AllHeaders.Keys)
            {
                yield return Get(key);
            }
        }
    }
}
