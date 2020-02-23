using DG.Dapper;
using QuartzHost.Core.Common;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Dao
{
    [ServiceMapTo(typeof(TaskDao))]
    public class TaskDao
    {
        /// <summary>
        ///     分页 前面必须有 order by 语句 COUNT(1) OVER() AS Total 获取总条数
        /// </summary>
        public const string PAGESUFFIX = "OFFSET (@PageIndex-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

        private readonly DGContext _context;

        public TaskDao(DGContext dGContext)
        {
            _context = dGContext;
        }

        public Task<IEnumerable<JobTasksEntity>> QueryAllAsync(JobTaskStatus? status)
        {
            if (status.HasValue)
                return _context.QueryAsync<JobTasksEntity>("SELECT * FROM JobTasks WITH(NOLOCK) Where Status=@Status", new { Status = status });
            else
                return _context.QueryAsync<JobTasksEntity>("SELECT * FROM JobTasks WITH(NOLOCK) Where Status<>@Status", new { Status = JobTaskStatus.Deleted });
        }

        public Task<IEnumerable<JobTasksEntity>> QueryPagerAsync(int pageIndex, int pageSize)
        {
            return _context.QueryAsync<JobTasksEntity>($"SELECT COUNT(1) OVER() AS Total,* FROM  JobTasks WITH(NOLOCK) ORDER BY CreateTime {PAGESUFFIX}", new { PageIndex = pageIndex, PageSize = pageSize });
        }
    }
}