namespace BaristaLabs.BaristaCore.Http
{
    using JavaScript;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    public class JavaScriptValueResult
    {
        //public JavaScriptValueResult(JavaScriptValue value)
        //{
        //    Value = value;
        //    Formatters = new FormatterCollection<IOutputFormatter>();
        //    ContentTypes = new MediaTypeCollection();
        //}

        //public JavaScriptValue Value
        //{
        //    get;
        //    set;
        //}

        //public FormatterCollection<IOutputFormatter> Formatters
        //{
        //    get;
        //    set;
        //}

        //public MediaTypeCollection ContentTypes
        //{
        //    get;
        //    set;
        //}

        //public Type DeclaredType
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the HTTP status code.
        ///// </summary>
        //public int? StatusCode
        //{
        //    get;
        //    set;
        //}

        //public Task ExecuteResultAsync(HttpContext context)
        //{
        //    var executor = context.RequestServices.GetRequiredService<JavaScriptValueResultExecutor>();
        //    var result = executor.ExecuteAsync(context, this);

        //    return result;
        //}

        ///// <summary>
        ///// This method is called before the formatter writes to the output stream.
        ///// </summary>
        //public virtual void OnFormatting(HttpContext context)
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException(nameof(context));
        //    }

        //    if (StatusCode.HasValue)
        //    {
        //        context.Response.StatusCode = StatusCode.Value;
        //    }
        //}
    }
}
