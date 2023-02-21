using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class ValidateModelAttribute: ActionFilterAttribute
    {
         public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var state in context.ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        sb.Append(error.ErrorMessage);
                        sb.Append(Environment.NewLine);
                    }
                }
                throw new ModelValidationException("Model validation failed...!", new Exception("Model validation failed for: " + sb.ToString()));
            }
        }
    }
}