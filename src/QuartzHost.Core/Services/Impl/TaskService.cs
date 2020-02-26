using QuartzHost.Core.Common;
using QuartzHost.Core.Dao;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<Result<IEnumerable<JobTasksEntity>>> QueryAllAsync(JobTaskStatus? status = null)
        {
            var result = new Result<IEnumerable<JobTasksEntity>> { Message = "查询任务成功!" };

            try
            {
                result.Data = await _taskDao.QueryAllAsync(status);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"查询任务 异常:{ex.Message}");
            }
            return result;
        }

        public async Task<PageResult<IEnumerable<JobTasksEntity>>> QueryPagerAsync(PageInput pager)
        {
            var result = new PageResult<IEnumerable<JobTasksEntity>> { Message = "查询任务成功!" };

            try
            {
                var list = await _taskDao.QueryPagerAsync(pager.PageIndex, pager.PageSize);
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

        public async Task<Result<bool>> AddAsync(JobTasksEntity model, List<int> keepers, List<Guid> nexts, List<string> executors = null)
        {
            var result = new Result<bool> { Data = true, Message = "任务创建成功!" };

            try
            {
                if ((await _taskDao.AddAsync(model)) == false)
                {
                    result.Data = false;
                    result.Success = false;
                    result.Message = "任务创建失败！";
                    result.ErrorDetail = "添加任务失败";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"任务创建 异常:{ex.Message}");
            }
            return result;
        }

        public ServiceResponseMessage Delete(Guid sid)
        {
            throw new NotImplementedException();
        }

        public ServiceResponseMessage Edit(JobTasksEntity model)
        {
            throw new NotImplementedException();
        }

        public ServiceResponseMessage Pause(Guid sid)
        {
            throw new NotImplementedException();
        }

        public JobTasksEntity QueryById(Guid sid)
        {
            throw new NotImplementedException();
        }

        public int QueryScheduleCount(int? status)
        {
            throw new NotImplementedException();
        }

        public JobTaskView QueryScheduleView(Guid sid)
        {
            throw new NotImplementedException();
        }

        public int QueryTraceCount(int? status)
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<long, int>> QueryTraceWeeklyReport(int? status)
        {
            throw new NotImplementedException();
        }

        public List<JobTasksEntity> QueryUserSchedule(int userId, int takeSize)
        {
            throw new NotImplementedException();
        }

        public int QueryWorkerCount(int? status)
        {
            throw new NotImplementedException();
        }

        public List<JobNodesEntity> QueryWorkerList()
        {
            throw new NotImplementedException();
        }

        public ServiceResponseMessage Resume(Guid sid)
        {
            throw new NotImplementedException();
        }

        public ServiceResponseMessage RunOnce(Guid sid)
        {
            throw new NotImplementedException();
        }

        public ServiceResponseMessage Start(JobTasksEntity task)
        {
            throw new NotImplementedException();
        }

        public ServiceResponseMessage Stop(Guid sid)
        {
            throw new NotImplementedException();
        }
    }
}