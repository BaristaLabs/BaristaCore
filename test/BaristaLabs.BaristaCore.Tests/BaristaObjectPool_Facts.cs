namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.JavaScript;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    [Collection("BaristaCore Tests")]
    public class BaristaObjectPool_Facts
    {
        private ServiceCollection m_serviceCollection;
        private IServiceProvider m_provider;

        public BaristaObjectPool_Facts()
        {
            m_serviceCollection = new ServiceCollection();
            m_serviceCollection.AddBaristaCore();
            m_provider = m_serviceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeFactory BaristaRuntimeFactory
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeFactory>(); }
        }

        [Fact]
        public void BaristaObjectPoolCanBeCreated()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            Assert.NotNull(bop);
            Assert.Equal(0, bop.Count);
        }

        [Fact]
        public void BaristaObjectPoolCanTryAddItems()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                bop.TryAdd(value);
                Assert.Equal(1, bop.Count);
            }
        }

        [Fact]
        public void BaristaObjectPoolTryAddItemsCanOnlyBeAddedOnce()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                bop.TryAdd(value);
                var result = bop.TryAdd(value);
                Assert.False(result);
                Assert.Equal(1, bop.Count);
            }
        }

        [Fact]
        public void BaristaObjectPoolTryAddItemsCannotBeNull()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");

                Assert.Throws<ArgumentNullException>(() =>
                {
                    bop.TryAdd(null);
                });
            }
        }

        [Fact]
        public void BaristaObjectPoolTryAddItemsCannotBeInvalid()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                value.Dispose();

                Assert.Throws<ArgumentNullException>(() =>
                {
                    bop.TryAdd(value);
                });
            }
        }

        [Fact]
        public void BaristaObjectPoolTryGetRetrievesAddedItems()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                bop.TryAdd(value);
                bop.TryGet(value.Handle, out JsValue retrievedValue);

                Assert.Same(value, retrievedValue);
            }
        }

        [Fact]
        public void BaristaObjectPoolTryGetCannotRetrieveInvalidHandles()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                bop.TryAdd(value);

                Assert.Throws<ArgumentNullException>(() =>
                {
                    bop.TryGet(JavaScriptValueSafeHandle.Invalid, out JsValue retrievedValue);
                });

                Assert.Throws<ArgumentNullException>(() =>
                {
                    bop.TryGet(default(JavaScriptValueSafeHandle), out JsValue retrievedValue);
                });
            }
        }

        [Fact]
        public void BaristaObjectPoolTryGetRemovedHandlesAreNull()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            var handle = new JavaScriptValueSafeHandle();

            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                using (ctx.Scope())
                {
                    var value = ctx.ValueFactory.CreateString("Test 123");
                    bop.TryAdd(value);
                    handle = value.Handle;
                    value.Dispose();
                    value = null;
                }
                rt.CollectGarbage();
            }

            //m_provider = null;
            //m_serviceCollection.FreeBaristaCoreServices();
            //m_serviceCollection = null;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.WaitForFullGCComplete();
            //GC.Collect();

            bop.RemoveHandle(handle);
            bop.TryGet(handle, out JsValue retrievedValue);
            Assert.Null(retrievedValue);
        }

        [Fact]
        public void BaristaObjectPoolCanAddOrGetItems()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                var addedVal = bop.GetOrAdd(value.Handle, () =>
                {
                    return ctx.ValueFactory.CreateValue<JsString>(value.Handle);
                });

                Assert.Equal(1, bop.Count);
                Assert.Same(value, addedVal);
            }
        }

        [Fact]
        public void BaristaObjectPoolCanAddOrGetItemsThatHaveBeenDisposed()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var str = "Hello, World.";
                var handle = ctx.Engine.JsCreateString(str, (ulong)str.Length);

                var addedVal = bop.GetOrAdd(handle, () =>
                {
                    return new JsString(rt.Engine, ctx, handle);
                });

                addedVal.Dispose();
                addedVal = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                GC.Collect();

                var added2Val = bop.GetOrAdd(handle, () =>
                {
                    return ctx.ValueFactory.CreateValue<JsString>(handle);
                });

                Assert.Equal(1, bop.Count);
            }
        }

        [Fact]
        public void BaristaObjectPoolAddOrGetThrowsOnInvalidHandles()
        {
            var bop = new BaristaObjectPool<JsValue, JavaScriptValueSafeHandle>();
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            using (var ctx = rt.CreateContext())
            using (ctx.Scope())
            {
                var value = ctx.ValueFactory.CreateString("Test 123");
                Assert.Throws<ArgumentNullException>(() =>
                {
                   var addedVal = bop.GetOrAdd(JavaScriptValueSafeHandle.Invalid, () =>
                   {
                       return ctx.ValueFactory.CreateValue<JsString>(value.Handle);
                   });
                });

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var addedVal = bop.GetOrAdd(default(JavaScriptValueSafeHandle), () =>
                    {
                        return ctx.ValueFactory.CreateValue<JsString>(value.Handle);
                    });
                });
            }
        }
    }
}
