using Microsoft.AspNetCore.Mvc;
using QuartzHost.Core.Common;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.API.Common
{
    [ApiController]
    public class QuartzHostController : ControllerBase
    {
        /// <summary>
        /// 验证请求参数
        /// </summary>
        [NonAction]
        protected void ValidRequest()
        {
            if (!ModelState.IsValid)
                throw new BusinessException(ResultStatus.ValidateError, System.Net.HttpStatusCode.BadRequest);
        }
    }
}