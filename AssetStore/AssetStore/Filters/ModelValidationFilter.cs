using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace AssetStore.Api.Filters;

/// <summary>
///     Validates whether the model state is valid before executing the action.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        if (actionContext.ModelState.IsValid != false)
            return;
        
        actionContext.Response = actionContext.Request.CreateErrorResponse(
            HttpStatusCode.BadRequest, actionContext.ModelState);
    }
}