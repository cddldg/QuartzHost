﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuartzHost.Core.Models;
using QuartzHost.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.API.Controllers
{
    [ApiController]
    [Route("job")]
    public class JobController : ControllerBase
    {
        private ITaskService _service { get; set; }

        public JobController(ITaskService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("list")]
        public async Task<Result<IEnumerable<JobTasksEntity>>> QueryAllAsync()
        {
            return await _service.QueryAllAsync();
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<Result<bool>> AddAsync(JobTasksInput input)
        {
            return await _service.AddAsync(input.JobTasks, input.Keepers, input.Nexts, input.Executors);
        }
    }
}