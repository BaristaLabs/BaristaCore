namespace BaristaLabs.BaristaCore.Tests
{
    using Serilog;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class LoggingFixture : IDisposable
    {
        static int s_unhandledExceptionCount = 0;
        static object s_initLock = new object();
        static bool s_initialized = false;

        public LoggingFixture()
        {
            // Do this one time per AppDomain.
            if (!s_initialized)
            {
                lock (s_initLock)
                {
                    if (!s_initialized)
                    {
                        //var configuration = new ConfigurationBuilder()
                        //    .AddJsonFile("appsettings.json")
                        //    .Build();

                        Log.Logger = new LoggerConfiguration()
                           .WriteTo.Console()
                           .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Hour)
                           .CreateLogger();

                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

                        s_initialized = true;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        private void OnUnhandledException(object o, UnhandledExceptionEventArgs e)
        {
            
            // Let this occur one time for each AppDomain.
            if (Interlocked.Exchange(ref s_unhandledExceptionCount, 1) != 0)
                return;

            Exception currentException = null;
            for (currentException = (Exception)e.ExceptionObject; currentException != null; currentException = currentException.InnerException)
            {
                Log.Fatal(currentException, "Unhandled Exception Occurred.");
            }
        }
    }

    [CollectionDefinition("BaristaCore Tests")]
    public class BaristaCoreCollection : ICollectionFixture<LoggingFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
