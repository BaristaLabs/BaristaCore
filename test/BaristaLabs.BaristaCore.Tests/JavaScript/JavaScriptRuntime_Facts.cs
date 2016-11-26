namespace BaristaLabs.BaristaCore.JavaScript.Tests
{
    using Internal;

    using Extensions;
    using System;
    using System.Text;
    using Xunit;

    public class JavaScriptRuntime_Facts
    {
        private IJavaScriptEngine Engine;

        public JavaScriptRuntime_Facts()
        {
            Engine = JavaScriptEngineFactory.CreateChakraEngine();
        }

        //[Fact]
        //public void JavaScriptRuntimeCanBeConstructed() 
        //{
        //    using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Engine))
        //    {
        //    }
        //    Assert.True(true);
        //}

        //[Fact]
        //public void JavaScriptRuntimeShouldReturnRuntimeMemoryUsage()
        //{
        //    ulong memoryUsage;
        //    using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Engine))
        //    {
        //        memoryUsage = rt.RuntimeMemoryUsage;
        //    }
        //    Assert.True(memoryUsage > 0);
        //}

        //[Fact]
        //public void JavaScriptRuntimeShouldFireMemoryChangingCallbacks()
        //{
        //    int changeCount = 0;
        //    EventHandler<JavaScriptMemoryEventArgs> handler = (sender, e) =>
        //    {
        //        changeCount++;
        //    };

        //    using (var rt = JavaScriptRuntime.CreateJavaScriptRuntime(Engine))
        //    {
        //        rt.MemoryChanging += handler;
        //        changeCount = 0;
        //        using (var ctx = rt.CreateContext())
        //        {
        //        }
        //        rt.MemoryChanging -= handler;
        //    }

        //    Assert.True(changeCount > 0);
        //}
    }
}
