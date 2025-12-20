using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssetStore.Api.Filters;

/// <summary>
///     Validates whether the model state is valid before executing the action.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        context.Result = new BadRequestObjectResult(context.ModelState);
    }
}