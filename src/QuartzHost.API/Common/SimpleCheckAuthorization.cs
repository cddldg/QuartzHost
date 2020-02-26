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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using QuartzHost.Core.Models;
using System.Net;

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

            var token = context.HttpContext.Request.Headers.GetAuthorization();
            if (string.IsNullOrEmpty(token) || !CoreGlobal.NodeSetting.AccessSecret.Equals(token))
            {
                throw new BusinessException(ResultStatus.Illegal, HttpStatusCode.Unauthorized, "Unauthorized");
            }

            //var authorizationService = context.HttpContext.RequestServices.GetService<IAuthenticationService>();
            //var res = await authorizationService.AuthenticateAsync(context.HttpContext, "Bearer");
            //if (!res.Succeeded)
            //{
            //    context.HttpContext.Response.ContentType = "application/json";
            //    context.HttpContext.Response.StatusCode = Result.StatusCode;
            //    await context.HttpContext.Response.WriteAsync(ret.ToJson());
            //}
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
            var ex = context.Exception;
            _logger.LogError(ex, ex.Message);
            var ret = new QuartzHost.Core.Models.Result<int>
            {
                Data = (int)System.Net.HttpStatusCode.ServiceUnavailable,
                Success = false,
                Message = ResultStatus.ServiceError.GetDescription(),
                ErrorDetail = ex.Message
            };
            if (ex is BusinessException bsEx)
            {
                ret.Data = (int)bsEx.HttpStatusCode;
                ret.Message = bsEx.ResultStatus.GetDescription();
                ret.ErrorDetail = bsEx.Message;
            }
            context.Result = new StatusCodeResult(ret.Data);
            context.ExceptionHandled = true;

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = ret.Data;
            context.HttpContext.Response.WriteAsync(ret.ToJson());
        }
    }
}