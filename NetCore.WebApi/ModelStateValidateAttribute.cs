using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCore.WebApi
{
    internal class ModelStateValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = (new BadRequestObjectResult(context.ModelState));
            }
        }

        public ModelStateValidateAttribute()
            : base()
        {
        }
    }

}
