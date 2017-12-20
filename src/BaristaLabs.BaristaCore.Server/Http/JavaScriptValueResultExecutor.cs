namespace BaristaLabs.BaristaCore.Http
{
    using JavaScript;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Buffers;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Executes and formats a JavaScript Object to write to the response.
    /// </summary>
    /// <remarks>
    /// This class is similar to the ObjectResultExecutor from MVC but uses the result object to determine
    /// how the http response is formatted.
    /// 
    /// See https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/Internal/ObjectResultExecutor.cs
    /// </remarks>
    public class JavaScriptValueResultExecutor
    {
        public JavaScriptValueResultExecutor(
        IOptions<BaristaOptions> options,
        IHttpResponseStreamWriterFactory writerFactory,
        ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Logger = loggerFactory.CreateLogger<ObjectResultExecutor>();
            WriterFactory = writerFactory.CreateWriter;
        }

        /// <summary>
        /// Gets the <see cref="ILogger"/>.
        /// </summary>
        protected ILogger Logger
        {
            get;
        }

        /// <summary>
        /// Gets the writer factory delegate.
        /// </summary>
        protected Func<Stream, Encoding, TextWriter> WriterFactory
        {
            get;
        }

        /// <summary>
        /// Using the http context and value, obtain an appropriate value that is then formatted and returned in the http response.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual Task ExecuteAsync(HttpContext httpContext, JavaScriptValueResult result)
        {
            //var value = result.Value;
            //object resultValue;

            //var context = value.GetContext();

            //using (var jsContext = context.AcquireExecutionContext())
            //{
            //    if (context.HasException)
            //    {
            //        dynamic exception = context.GetAndClearException();
            //        var jsException = new JavaScriptScriptException(JavaScriptErrorCode.NoCurrentContext, )
            //        //{
            //        //    Message = (string)exception.message,
            //        //    LineNumber = (int)exception.line,
            //        //    ColumnNumber = (int)exception.column,
            //        //    Length = (int)exception.length,
            //        //    Source = (string)exception.source,
            //        //    Stack = exception.stack == context.UndefinedValue ? string.Empty : (string)exception.stack
            //        //};

            //        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //        httpContext.Response.ContentType = "text/html";
            //        resultValue = jsException;
            //    }
            //    else
            //    {
            //        dynamic glob = context.GlobalObject;
            //        //TODO: Use a stringify that doesn't die on recursive referenes
            //        var val = glob.JSON.stringify(result.Value);
            //        if ((string)val == "{}")
            //            val = context.Converter.ToString(result.Value);

            //        //TODO: Support other result types, buffer, etc

            //        //TODO: Allow require("http").response have first go at the actual result.

            //        result.OnFormatting(httpContext);
            //        resultValue = (string)val;
            //    }

            //    var objectType = resultValue.GetType();

            //    var formatterContext = new OutputFormatterWriteContext(
            //        httpContext,
            //        WriterFactory,
            //        objectType,
            //        resultValue);

            //    var formatter = SelectFormatter(formatterContext);
            //    return formatter.WriteAsync(formatterContext);
            //}

            return Task.FromResult(42);
        }

        protected virtual IOutputFormatter SelectFormatter(OutputFormatterWriteContext formatterContext)
        {
            if (formatterContext == null)
            {
                throw new ArgumentNullException(nameof(formatterContext));
            }

            //If the result is a JavaScriptException, serialize exception details.
            if (formatterContext.ObjectType.IsAssignableFrom(typeof(JsException)))
            {
                var formatterSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
                return new JsonOutputFormatter(formatterSettings, ArrayPool<char>.Create());
            }

            //TODO: Additional Formatters....
            return new StringOutputFormatter();
        }
    }
}
