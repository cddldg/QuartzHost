﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        [Route("init")]
        public async Task<Result<bool>> InitScheduler()
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

        /// <summary>
        /// 启动一个任务，带重试3机制
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("start")]
        public async Task<Result<bool>> Start(long sid)
        {
            return await _service.StartJobTask(sid);
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("pause")]
        public async Task<Result<bool>> Pause(long sid)
        {
            return await _service.Pause(sid);
        }

        /// <summary>
        /// 恢复运行一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resume")]
        public async Task<Result<bool>> Resume(long sid)
        {
            return await _service.Resume(sid);
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("stop")]
        public async Task<Result<bool>> Stop(long sid)
        {
            return await _service.Stop(sid);
        }

        /// <summary>
        ///立即运行一次任务
        /// </summary>
        /// <param name="sid"></param>
        [HttpPost]
        [Route("runonce")]
        public async Task<Result<bool>> RunOnce(long sid)
        {
            return await _service.RunOnce(sid);
        }
    }
}