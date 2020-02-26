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
        private string PAGESUFFIX = "OFFSET (@PageIndex-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

        private string NOLOCK = "WITH(NOLOCK)";
        private readonly DGContext _context;

        public TaskDao(DGContext dGContext)
        {
            _context = dGContext;
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
            {
                PAGESUFFIX = "LIMIT @PageSize OFFSET (@PageIndex-1)*@PageSize";
                NOLOCK = "";
            }
        }

        public Task<IEnumerable<JobTasksEntity>> QueryAllAsync(JobTaskStatus? status)
        {
            if (status.HasValue)
                return _context.QueryAsync<JobTasksEntity>($"SELECT * FROM JobTasks {NOLOCK} Where Status=@Status", new { Status = status });
            else
                return _context.QueryAsync<JobTasksEntity>($"SELECT * FROM JobTasks {NOLOCK} Where Status<>@Status", new { Status = JobTaskStatus.Deleted });
        }

        public Task<IEnumerable<JobTasksEntity>> QueryPagerAsync(int pageIndex, int pageSize)
        {
            return _context.QueryAsync<JobTasksEntity>($"SELECT COUNT(1) OVER() AS Total,* FROM  JobTasks {NOLOCK} ORDER BY CreateTime {PAGESUFFIX}", new { PageIndex = pageIndex, PageSize = pageSize });
        }

        public async Task<bool> AddAsync(JobTasksEntity entity)
        {
            var sql = $@"INSERT INTO JobTasks
(Id,NodeName,Title,Remark,CronExpression,AssemblyName,ClassName,CustomParamsJson,Status,CreateTime,CreateUserId,CreateUserName,TotalRunCount)
VALUES
(@Id,@NodeName,@Title,@Remark,@CronExpression,@AssemblyName,@ClassName,@CustomParamsJson,@Status,@CreateTime,@CreateUserId,@CreateUserName,0)";
            int exeint = 0;
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
                exeint = await _context.ExecuteAsync(sql, new { Id = entity.Id.ToString().ToUpper(), entity.NodeName, entity.Title, entity.Remark, entity.CronExpression, entity.AssemblyName, entity.ClassName, entity.CustomParamsJson, entity.Status, entity.CreateTime, entity.CreateUserId, entity.CreateUserName });
            else
                exeint = await _context.ExecuteAsync(sql, entity);
            return exeint > 0;
        }
    }
}