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
            var result = new Result<IEnumerable<JobTasksEntity>>();

            try
            {
                result.Data = await _taskDao.QueryAllAsync(status);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"QueryAllAsync 异常");
            }
            return result;
        }

        public async Task<PageResult<IEnumerable<JobTasksEntity>>> QueryPagerAsync(PageInput pager)
        {
            var result = new PageResult<IEnumerable<JobTasksEntity>>();

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
                _logger.LogError(ex, $"QueryPagerAsync 异常");
            }
            return result;
        }

        public ServiceResponseMessage Add(JobTasksEntity model, List<int> keepers, List<Guid> nexts, List<string> executors = null)
        {
            throw new NotImplementedException();
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