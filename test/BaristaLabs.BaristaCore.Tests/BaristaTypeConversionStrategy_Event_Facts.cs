namespace BaristaLabs.BaristaCore.Tests
{
    using System;
    using Xunit;

    public partial class BaristaTypeConversionStrategy_Facts
    {
        [Fact]
        public void ConverterExposesStaticAndInstanceEvents()
        {
            var script = @"
var count = 0;
Foo.addEventListener('myStaticEvent', () => { count++; });
var myFoo = new Foo();
myFoo.addEventListener('myEvent', () => { count++; });
myFoo.bonk();
export default count;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsNumber>(script);
                        Assert.Equal(2, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void EventsListenersCanBeRemoved()
        {
            var script = @"
var count = 0;
var myFoo = new Foo();
myFoo.addEventListener('myEvent', () => { count++; });
myFoo.bonk();
myFoo.removeAllEventListeners('myEvent');
myFoo.removeAllEventListeners('myEvent');
myFoo.bonk();
myFoo.removeAllEventListeners('foo');
export default count;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasInstanceEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsNumber>(script);
                        Assert.Equal(1, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void ConverterDoesNotIncludeIgnoredEvents()
        {
            var script = @"
var count = 0;
Foo.addEventListener('myStaticEvent', () => { count++; });
var myFoo = new Foo();
var result = myFoo.addEventListener('myEvent', () => { count++; });
export default result;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasIgnoredEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsBoolean>(script);
                        Assert.False(result.ToBoolean());
                    }
                }
            }
        }

        [Fact]
        public void ConverterDoesNotAddEventMethodsIfNoEvents()
        {
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasEvents), out JsValue value);
                        var jsObj = value as JsObject;
                        Assert.True(jsObj.HasOwnProperty("addEventListener"));
                        Assert.True(jsObj.HasOwnProperty("removeEventListener"));
                        Assert.True(jsObj.HasOwnProperty("removeAllEventListeners"));

                        ctx.Converter.TryFromObject(ctx, typeof(HasNoExposedEvents), out JsValue value1);
                        var jsObj1 = value1 as JsObject;
                        Assert.False(jsObj1.HasOwnProperty("addEventListener"));
                        Assert.False(jsObj1.HasOwnProperty("removeEventListener"));
                        Assert.False(jsObj1.HasOwnProperty("removeAllEventListeners"));
                    }
                }
            }
        }

        [Fact]
        public void ConverterUsesEventAttributesForNaming()
        {
            var script = @"
var count = 0;
var myFoo = new Foo();
myFoo.addEventListener('mySuperEvent', () => { count++; });
myFoo.bonk();
myFoo.bonk();
export default count;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasNamedEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsNumber>(script);
                        Assert.Equal(2, result.ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void MustSpecifyANameToRegisterAndRemoveListeners()
        {
            var script = @"
var count = 0;
var myFoo = new Foo();
myFoo.addEventListener('', () => { count++; });
export default count;
";
            var script2 = @"
var count = 0;
var myFoo = new Foo();
myFoo.addEventListener('foo', () => { count++; });
myFoo.removeAllListeners('', () => { count++; });
export default count;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasInstanceEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsNumber>(script);
                        });

                        ctx.CurrentScope.GetAndClearException();

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsNumber>(script2);
                        });
                    }
                }
            }
        }

        [Fact]
        public void CanRegisterMultipleEvents()
        {
            var script = @"
var count = 0;
var dracula = 0;
var myFoo = new Foo();
myFoo.addEventListener('mySuperEvent', () => { count++; });
myFoo.addEventListener('mySuperEvent', () => { dracula++; });
myFoo.addEventListener('mySuperDuperEvent', () => { count++; });
myFoo.bonk();
myFoo.bonk();
export default { count, dracula };
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal(4, result["count"].ToInt32());
                        Assert.Equal(2, result["dracula"].ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void CanRemoveIndividualEvents()
        {
            var script = @"
var count = 0;
var dracula = 0;
var myFoo = new Foo();
var incCount = () => { count++; };
var incDrac = () => { dracula++; };
myFoo.addEventListener('mySuperEvent', incCount);
myFoo.addEventListener('mySuperEvent', incDrac);
myFoo.bonk();

myFoo.removeEventListener('mySuperEvent', incCount);
myFoo.bonk();

export default { count, dracula };
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.Equal(1, result["count"].ToInt32());
                        Assert.Equal(2, result["dracula"].ToInt32());
                    }
                }
            }
        }

        [Fact]
        public void WhenRemovingEventListenersEventTypeMustBeSpecified()
        {
            var script = @"
var count = 0;
var dracula = 0;
var myFoo = new Foo();
var incCount = () => { count++; };
var incDrac = () => { dracula++; };
myFoo.addEventListener('mySuperEvent', incCount);
myFoo.addEventListener('mySuperEvent', incDrac);
myFoo.bonk();

myFoo.removeEventListener('', incCount);
myFoo.bonk();

export default { count, dracula };
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script);
                        });
                    }
                }
            }
        }

        [Fact]
        public void RemovingEventListenerWithoutEventThrows()
        {
            var script = @"
var myFoo = new Foo();
myFoo.removeEventListener('foo', null);

export default '123';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script);
                        });
                    }
                }
            }
        }

        [Fact]
        public void WhenRemovingAllEventListenersEventTypeMustBeSpecified()
        {
            var script = @"
var myFoo = new Foo();
myFoo.removeAllEventListeners('', () => {});

export default '123';
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsObject>(script);
                        });
                    }
                }
            }
        }

        [Fact]
        public void AttemptingToRemoveUnRegisteredEventReturnsFalse()
        {
            var script = @"
var count = 0;
var dracula = 0;
var myFoo = new Foo();
var incCount = () => { count++; };
var incDrac = () => { dracula++; };
myFoo.addEventListener('mySuperEvent', incCount);
var result = myFoo.removeEventListener('mySuperEvent', incDrac);

export default result;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        var result = ctx.EvaluateModule<JsBoolean>(script);
                        Assert.False(result);
                    }
                }
            }
        }

        [Fact]
        public void CallingAddEventListenerWithNullThisThrows()
        {
            var script = @"
var myFoo = new Foo();
myFoo.addEventListener.call(null, 'myEvent');
export default result;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsBoolean>(script);
                        });


                    }
                }
            }
        }

        [Fact]
        public void CallingRemoveEventListenerWithNullThisThrows()
        {
            var script = @"
var myFoo = new Foo();
myFoo.removeEventListener.call(null, 'myEvent', () => { });
export default result;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsBoolean>(script);
                        });


                    }
                }
            }
        }

        [Fact]
        public void CallingRemoveAllEventListenerWithNullThisThrows()
        {
            var script = @"
var myFoo = new Foo();
myFoo.removeAllEventListeners.call(null, 'myEvent');
export default result;
";
            using (var rt = BaristaRuntimeFactory.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        ctx.Converter.TryFromObject(ctx, typeof(HasMultipleEvents), out JsValue value);
                        ctx.GlobalObject["Foo"] = value;

                        Assert.Throws<JsScriptException>(() =>
                        {
                            var result = ctx.EvaluateModule<JsBoolean>(script);
                        });


                    }
                }
            }
        }

        #region Test Event Classes
        private class HasEvents
        {
            public static event EventHandler<EventArgs> MyStaticEvent;
            public event EventHandler<EventArgs> MyEvent;

            public void Bonk()
            {
                OnMyEvent();
                OnMyStaticEvent();
            }

            private void OnMyEvent()
            {
                MyEvent?.Invoke(this, new EventArgs());
            }

            private static void OnMyStaticEvent()
            {
                if (MyStaticEvent != null)
                {
                    lock (MyStaticEvent)
                    {
                        MyStaticEvent?.Invoke(null, new EventArgs());
                    }
                }
            }
        }

        private class HasInstanceEvents
        {
            public event EventHandler<EventArgs> MyEvent;

            public void Bonk()
            {
                OnMyEvent();
            }

            private void OnMyEvent()
            {
                MyEvent?.Invoke(this, new EventArgs());
            }
        }

        private class HasIgnoredEvents
        {
            public static event EventHandler<EventArgs> MyStaticEvent;
            [BaristaIgnore]
            public event EventHandler<EventArgs> MyEvent;

            public event EventHandler<EventArgs> MyOtherEvent;

            public void Bonk()
            {
                OnMyEvent();
                OnMyOtherEvent();
                OnMyStaticEvent();
            }

            private void OnMyEvent()
            {
                MyEvent?.Invoke(this, new EventArgs());
            }

            private void OnMyOtherEvent()
            {
                MyOtherEvent?.Invoke(null, new EventArgs());
            }

            private static void OnMyStaticEvent()
            {
                MyStaticEvent?.Invoke(null, new EventArgs());
            }
        }

        private class HasNoExposedEvents
        {
            [BaristaIgnore]
            public static event EventHandler<EventArgs> MyStaticEvent;
            [BaristaIgnore]
            public event EventHandler<EventArgs> MyEvent;

            public void Bonk()
            {
                OnMyEvent();
                OnMyStaticEvent();
            }

            private void OnMyEvent()
            {
                MyEvent?.Invoke(this, new EventArgs());
            }

            private static void OnMyStaticEvent()
            {
                MyStaticEvent?.Invoke(null, new EventArgs());
            }
        }

        private class HasNamedEvents
        {
            [BaristaProperty("mySuperEvent")]
            public event EventHandler<EventArgs> MyEvent;

            public void Bonk()
            {
                OnMyEvent();
            }

            private void OnMyEvent()
            {
                MyEvent?.Invoke(this, new EventArgs());
            }
        }

        private class HasMultipleEvents
        {
            [BaristaProperty("mySuperEvent")]
            public event EventHandler<EventArgs> MyEvent;

            [BaristaProperty("mySuperDuperEvent")]
            public event EventHandler<EventArgs> MyOtherEvent;

            public void Bonk()
            {
                OnMyEvent();
                OnMyOtherEvent();
            }

            private void OnMyEvent()
            {
                MyEvent?.Invoke(this, new EventArgs());
            }

            private void OnMyOtherEvent()
            {
                MyOtherEvent?.Invoke(this, new EventArgs());
            }
        }

        #endregion
    }
}
