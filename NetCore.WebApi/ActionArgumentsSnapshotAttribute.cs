using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCore.WebApi
{
    internal class ActionArgumentsSnapshotAttribute : ActionFilterAttribute
    {
        public const string ArgumentsSnapshotKey = "!ActionArguments:Snapshot";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["!ActionArguments:Snapshot"] = context.ActionArguments;
        }

        public ActionArgumentsSnapshotAttribute()
            : base()
        {
        }
    }
}
