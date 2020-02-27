using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuartzHost.API.Common;
using QuartzHost.Core.Common;
using QuartzHost.Core.Models;
using QuartzHost.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.API.Controllers
{
    /// <summary>
    /// job
    /// </summary>
    [Route("job")]
    public class JobController : QuartzHostController
    {
        private readonly ITaskService _taskService;
        private readonly IQuartzService _quartzService;

        public JobController(ITaskService service, IQuartzService quartzService)
        {
            _taskService = service;
            _quartzService = quartzService;
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("get/all")]
        public async Task<Result<IEnumerable<JobTasksEntity>>> QueryAllAsync()
        {
            return await _taskService.QueryAllAsync();
        }

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("get/pager")]
        public async Task<PageResult<List<JobTasksEntity>>> QueryPagerAsync(PageInput pager)
        {
            return await _taskService.QueryPagerAsync(pager);
        }

        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="sid">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get/one")]
        public async Task<Result<JobTasksEntity>> QueryById(long sid)
        {
            return await _taskService.QueryById(sid);
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<Result<long>> AddAsync(JobTasksInput input)
        {
            var result = await _taskService.AddAsync(input);
            if (result.Success && input.RunNow)
            {
                var start = await _quartzService.StartJobTask(result.Data);
                result.Message = $"任务创建成功!任务状态:[{start.Data.GetDescription()}]";
            }
            return result;
        }
    }
}