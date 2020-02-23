using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using QuartzHost.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.API.Common
{
    /// <summary>
    /// 考虑基本内部使用所以简单验证
    /// </summary>
    public class SimpleCheckAuthorization : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var anonymous = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            if (anonymous.Any())
            {
                return;
            }
            var secret = context.HttpContext.Request.Headers["node_secret"].FirstOrDefault();
            if (!CoreGlobal.NodeSetting.AccessSecret.Equals(secret))
            {
                var Result = new UnauthorizedResult();
                var ret = new QuartzHost.Core.Models.Result<int>
                {
                    Data = Result.StatusCode,
                    Success = false,
                    Message = "Unauthorized"
                };
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = Result.StatusCode;
                context.HttpContext.Response.WriteAsync(ret.ToJson());
            }
        }
    }

    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger = DG.Logger.DGLogManager.GetLogger();

        public GlobalExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.ServiceUnavailable);
            context.ExceptionHandled = true;
        }
    }
}