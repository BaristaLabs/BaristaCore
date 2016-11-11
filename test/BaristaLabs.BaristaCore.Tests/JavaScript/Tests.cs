namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Xunit;

    public class BasicJavaScriptFacts
    {
        [Fact]
        public void JavaScriptRuntimeCanBeConstructed() 
        {
            using (var rt = new JavaScriptRuntime())
            {
            }
            Assert.True(true);
        }

        [Fact]
        public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        {
            ulong memoryUsage;
            using (var rt = new JavaScriptRuntime())
            {
                memoryUsage = rt.RuntimeMemoryUsage;
            }
            Assert.True(memoryUsage > 0);
        }

        [Fact]
        public void JavaScriptContextCanBeCreated()
        {
            using (var rt = new JavaScriptRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    
                }
            }
            Assert.True(true);
        }
    }
}
