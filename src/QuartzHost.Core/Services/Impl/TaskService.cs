using QuartzHost.Core.Common;
using QuartzHost.Contract.Common;
using QuartzHost.Core.Dao;
using QuartzHost.Contract.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace QuartzHost.Core.Services.Impl
{
    [ServiceMapTo(typeof(ITaskService))]
    public class TaskService : BaseService, ITaskService
    {
        private readonly TaskDao _taskDao;
        private readonly ILogger _logger;

        public TaskService(TaskDao taskDao, ILogger logger)
        {
            _taskDao = taskDao;
            _logger = logger;
        }

        public async Task<Result<JobUserEntity>> QueryUserAsync(Input input)
        {
            var result = new Result<JobUserEntity> { Message = "查询用户成功！" };
            try
            {
                input.Extens.TryGetValue("UserName", out string name);
                input.Extens.TryGetValue("Password", out string pwd);
                result.Data = await _taskDao.QueryUserAsync(name.Trim(), Secret.GetMd5(pwd.Trim()));
                if (result.Data == null || result.Data.Id <= 0)
                    throw new Exception("查无此人！");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"查无此人！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"input:{input.ToJson()} 查询用户失败！");
            }
            return result;
        }

        public async Task<Result<JobUserEntity>> LoginAsync(Input input)
        {
            var result = new Result<JobUserEntity> { Message = "查询用户成功！" };
            try
            {
                input.Extens.TryGetValue("UserName", out string name);
                input.Extens.TryGetValue("Password", out string pwd);
                pwd = Secret.DesDecrypt(pwd);
                result.Data = await _taskDao.QueryUserAsync(name.Trim(), Secret.GetMd5(pwd.Trim()));
                result.Message = $"{result.Data.RealName ?? name} 欢迎你！";
                if (result.Data == null || result.Data.Id <= 0)
                    throw new Exception("查无此人！");
                await _taskDao.LastLoginTimeAsync(result.Data.Id);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"查无此人！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"input:{input.ToJson()} 查询用户失败！");
            }
            return result;
        }

        public async Task<Result<List<JobDictEntity>>> QueryDictAllAsync(DictType? type)
        {
            var result = new Result<List<JobDictEntity>> { Message = "查询Dict成功!" };

            try
            {
                result.Data = await _taskDao.QueryDictAllAsync(type);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "查询Dict失败!";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"查询Dict 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<Result<List<JobTasksEntity>>> QueryAllAsync(JobTaskStatus? status = null)
        {
            var result = new Result<List<JobTasksEntity>> { Message = "查询任务成功!" };

            try
            {
                result.Data = await _taskDao.QueryAllAsync(status);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "查询任务失败!";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"查询任务 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<PageResult<List<JobTasksEntity>>> QueryPagerAsync(PageInput pager)
        {
            var result = new PageResult<List<JobTasksEntity>> { Message = "查询任务成功!" };

            try
            {
                var list = await _taskDao.QueryPagerAsync(pager);
                result.Data = list;
                result.Total = list?.FirstOrDefault()?.Total ?? 0;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"分页查询任务 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<PageResult<List<JobTraceEntity>>> QueryTracesAsync(PageInput pager)
        {
            var result = new PageResult<List<JobTraceEntity>> { Message = "查询日志成功!" };

            try
            {
                var list = await _taskDao.QueryTracesAsync(pager);
                result.Data = list;
                result.Total = list?.FirstOrDefault()?.Total ?? 0;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"分页查询任务日志 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<Result<JobTasksEntity>> QueryById(long sid)
        {
            var result = new Result<JobTasksEntity>();

            try
            {
                result.Data = await _taskDao.QueryByIdAsync(sid);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "获取任务失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"获取任务 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<Result<long>> AddAsync(JobTasksInput input)
        {
            var result = new Result<long> { Message = $"任务创建成功!任务状态:[{JobTaskStatus.Stop.GetDescription()}]" };

            try
            {
                input.JobTasks.Id = QuartzHostExtensions.GetTimeStamp();// CoreGlobal.SnowflakeUniqueId();
                input.JobTasks.Status = JobTaskStatus.Stop;
                result.Data = input.JobTasks.Id;
                var isOk = await _taskDao.AddAsync(input.JobTasks);
                if (isOk)
                {
                }
                else
                {
                    result.Success = false;
                    result.Message = "任务创建失败！";
                    result.ErrorDetail = "添加任务失败";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "任务创建失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"任务创建 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<Result<ResultStatus>> EditAsync(JobTasksInput input)
        {
            var result = new Result<ResultStatus> { Data = ResultStatus.Success, Message = $"编辑任务成功!" };

            try
            {
                var task = await _taskDao.QueryByIdAsync(input.JobTasks.Id);
                if (task == null)
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "任务不存在!";
                    return result;
                }
                if (task.Status != JobTaskStatus.Stop)
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "在停止状态下才能编辑任务信息!";
                    return result;
                }
                if (!await _taskDao.UpdateJobTaskAsync(task))
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "编辑任务失败!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Data = ResultStatus.Failed;
                result.Success = false;
                result.Message = "编辑任务失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"编辑任务 异常:{ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<Result<ResultStatus>> DeleteTask(long sid)
        {
            var result = new Result<ResultStatus> { Data = ResultStatus.Success, Message = $"删除任务成功!" };

            try
            {
                var task = await _taskDao.QueryByIdAsync(sid);
                if (task == null)
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "任务不存在!";
                    return result;
                }
                if (task.Status == JobTaskStatus.Deleted)
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "任务已经是删除状态!";
                    return result;
                }
                if (task.Status != JobTaskStatus.Stop)
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "在停止状态下才能删除任务!";
                    return result;
                }

                if (!await _taskDao.DeleteJobTaskAsync(sid))
                {
                    result.Data = ResultStatus.Failed;
                    result.Success = false;
                    result.Message = "删除任务失败!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Data = ResultStatus.Failed;
                result.Success = false;
                result.Message = "删除任务失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"删除任务 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<Result<int>> QueryAllCountAsync(JobTaskStatus? status)
        {
            var result = new Result<int> { Message = $"获取数量成功!" };

            try
            {
                result.Data = await _taskDao.QueryAllCountAsync(status);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "获取数量失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"获取数量 异常:{ex.Message}");
            }
            return result;
        }

        public int QueryTraceCount(int? status)
        {
            throw new NotImplementedException();
        }

        public int QueryWorkerCount(int? status)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<JobNodesEntity>>> QueryNodesAll()
        {
            var result = new Result<List<JobNodesEntity>> { Message = "查询所有工作节点成功!" };

            try
            {
                result.Data = await _taskDao.QueryNodesAllAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "查询所有工作节点失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"查询所有工作节点 异常:{ex.Message}");
            }
            return result;
        }
    }
}