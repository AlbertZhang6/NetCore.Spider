using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace NetCore.WebApi
{
    internal class ApiErrorReplyAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ILogger<ApiErrorReplyAttribute> service = ServiceProviderServiceExtensions.GetService<ILogger<ApiErrorReplyAttribute>>(context.HttpContext.RequestServices);
            string text = NewErrorID();
            object obj = context.HttpContext.Items["!ActionArguments:Snapshot"];
            LoggerExtensions.LogError(service, new EventId(500, text), context.Exception, JsonConvert.SerializeObject(obj), Array.Empty<object>());
            if (context.Result == null)
            {
                ObjectResult val = new ObjectResult((object)text);
                val.StatusCode = ((int?)500);
                context.Result = (val);
            }
        }

        private string NewErrorID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
    }
}
