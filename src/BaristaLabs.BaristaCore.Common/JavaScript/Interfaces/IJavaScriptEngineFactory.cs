namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    /// Represents a factory for creating JavaScript Engines.
    /// </summary>
    public interface IJavaScriptEngineFactory
    {
        IJavaScriptEngine CreateJavaScriptEngine();
    }
}
