namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    /// <summary>
    /// Contains the methods of the symbol constructor
    /// </summary>
    /// <remarks>
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Symbol"/>
    /// </remarks>
    public class JsSymbolConstructor : JsObject
    {
        public JsSymbolConstructor(IJavaScriptEngine engine, BaristaContext context, JavaScriptValueSafeHandle valueHandle)
            : base(engine, context, valueHandle)
        {
        }

        public JsSymbol Iterator
        {
            get
            {
                return GetProperty<JsSymbol>("iterator");
            }
        }

        /// <summary>
        /// Searches for existing symbols in a runtime-wide symbol registry with the given key and returns it if found. Otherwise a new symbol gets created in the global symbol registry with this key.
        /// </summary>
        /// <param name="key">The key for the symbol (and also used for the description of the symbol).</param>
        /// <returns>An existing symbol with the given key if found; otherwise, a new symbol is created and returned.</returns>
        public JsSymbol For(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var fnFor = GetProperty<JsFunction>("for");
            return fnFor.Call<JsSymbol>(this, Context.CreateString(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public JsValue KeyFor(JsSymbol sym)
        {
            if (sym == null)
                throw new ArgumentNullException(nameof(sym));

            var fnKeyFor = GetProperty<JsFunction>("keyFor");
            return fnKeyFor.Call(this, sym);
        }
    }
}
