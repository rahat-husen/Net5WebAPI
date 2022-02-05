using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.ActionFilters
{
    public class ValidationFilterAttribute:IActionFilter
    {
        private ILoggerManager _logger;
        public ValidationFilterAttribute(ILoggerManager logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];
            var param = context.ActionArguments
                        .SingleOrDefault(x => x.Value.ToString().Contains("Dto")).Value;
            if (param is null)
            {
                _logger.LogError($"Object send is null Controller:{controller} action:{action}");
                context.Result = new BadRequestObjectResult($"Object send is null Controller:{controller} action:{action}");
                return;
            }
            if (!context.ModelState.IsValid)
            {
                _logger.LogError($"Invalid modelstate for the object Controller:{controller} action:{action}");
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }
    }
}
