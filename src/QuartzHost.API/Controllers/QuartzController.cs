using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using QuartzHost.API.Common;
using QuartzHost.Core.Models;
using QuartzHost.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.API.Controllers
{
    [Route("quartz")]
    public class QuartzController : QuartzHostController
    {
        private IQuartzService _service { get; set; }

        public QuartzController(IQuartzService service)
        {
            _service = service;
        }

        /// <summary>
        /// 启动调度系统
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("start")]
        public async Task<Result<bool>> Start()
        {
            return await _service.InitScheduler();
        }

        /// <summary>
        /// 关闭调度系统
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("shutdown")]
        public async Task<Result<bool>> Shutdown()
        {
            return await _service.Shutdown(false);
        }
    }
}