using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCore.WebApi
{
    internal class VoidApiResultAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is EmptyResult)
            {
                context.Result = new NoContentResult();
            }
        }

        public VoidApiResultAttribute()
            : base()
        {
        }
    }
}
