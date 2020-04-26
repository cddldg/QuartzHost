using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuartzHost.API.Common;
using QuartzHost.Contract.Common;
using QuartzHost.Contract.Models;
using QuartzHost.Core.Services;
using System.Collections.Generic;
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
        /// 查询所有Dict
        /// </summary>
        /// <param name="type">类型 可选</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("dict/all")]
        public async Task<Result<List<JobDictEntity>>> QueryDictAllAsync(DictType? type)
        {
            return await _taskService.QueryDictAllAsync(type);
        }

        /// <summary>
        /// 查询所有node列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("node/all")]
        public async Task<Result<List<JobNodesEntity>>> QueryNodesAll()
        {
            return await _taskService.QueryNodesAll();
        }

        /// <summary>
        /// 查询全部任务
        /// </summary>
        /// <param name="status">状态 可选</param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/all")]
        public async Task<Result<List<JobTasksEntity>>> QueryAllAsync(JobTaskStatus? status)
        {
            return await _taskService.QueryAllAsync(status);
        }

        /// <summary>
        /// 查询指定状态的任务数量
        /// </summary>
        /// <param name="status">状态 可选</param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/allcount")]
        public async Task<Result<int>> QueryAllCountAsync(JobTaskStatus? status)
        {
            return await _taskService.QueryAllCountAsync(status);
        }

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/pager")]
        public async Task<PageResult<List<JobTasksEntity>>> QueryPagerAsync(PageInput pager)
        {
            return await _taskService.QueryPagerAsync(pager);
        }

        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/one/{id}")]
        public async Task<Result<JobTasksEntity>> QueryById(long id)
        {
            return await _taskService.QueryById(id);
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/create")]
        public async Task<Result<long>> AddAsync(JobTasksInput input)
        {
            var result = await _taskService.AddAsync(input);
            if (result.Success && input.RunNow)
            {
                var start = await _quartzService.StartJobTask(result.Data);

                result.Message = $"任务创建成功!启动详情:{start.Message} 任务状态:[{start.Data.GetDescription()}]";
                result.ErrorDetail += start.ErrorDetail;
            }
            return result;
        }

        /// <summary>
        /// 启动一个任务，带重试3机制
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/start/{id}")]
        public async Task<Result<JobTaskStatus>> Start(long id)
        {
            return await _quartzService.StartJobTask(id);
        }

        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/edit")]
        public async Task<Result<ResultStatus>> EditAsync(JobTasksInput input)
        {
            return await _taskService.EditAsync(input);
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/pause/{id}")]
        public async Task<Result<JobTaskStatus>> PauseAsync(long id)
        {
            return await _quartzService.PauseTask(id);
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/runonce/{id}")]
        public async Task<Result<ResultStatus>> RunOnce(long id)
        {
            return await _quartzService.RunOnceTask(id);
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/resume/{id}")]
        public async Task<Result<JobTaskStatus>> Resume(long id)
        {
            return await _quartzService.ResumeTask(id);
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/stop/{id}")]
        public async Task<Result<JobTaskStatus>> Stop(long id)
        {
            return await _quartzService.StopTask(id);
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [HttpDelete]
        [Route("task/delete/{id}")]
        public async Task<Result<ResultStatus>> Delete(long id)
        {
            return await _taskService.DeleteTask(id);
        }
    }
}