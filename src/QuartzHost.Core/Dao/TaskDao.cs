using Dapper;
using DG.Dapper;
using QuartzHost.Core.Common;
using QuartzHost.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<JobDictEntity>> QueryDictAllAsync(DictType? type)
        {
            if (type.HasValue)
                return (await _context.QueryAsync<JobDictEntity>($"SELECT * FROM JobDict WHERE Type=@Type ORDER BY Type ASC,Sort ASC", new { Type = type }))?.ToList();
            else
                return (await _context.QueryAsync<JobDictEntity>($"SELECT * FROM JobDict  ORDER BY Type ASC,Sort ASC"))?.ToList();
        }

        public async Task<List<JobTasksEntity>> QueryAllAsync(JobTaskStatus? status)
        {
            if (status.HasValue)
                return (await _context.QueryAsync<JobTasksEntity>($"SELECT * FROM JobTasks {NOLOCK} Where Status=@Status", new { Status = status }))?.ToList();
            else
                return (await _context.QueryAsync<JobTasksEntity>($"SELECT * FROM JobTasks {NOLOCK} Where Status<>@Status", new { Status = JobTaskStatus.Deleted }))?.ToList();
        }

        public async Task<List<JobNodesEntity>> QueryNodesAllAsync()
        {
            return (await _context.QueryAsync<JobNodesEntity>($"SELECT * FROM JobNodes {NOLOCK} ORDER BY Status DESC,LastUpdateTime  "))?.ToList();
        }

        public Task<int> QueryAllCountAsync(JobTaskStatus? status)
        {
            if (status.HasValue)
                return _context.QuerySingleAsync<int>($"SELECT Count(1) FROM JobTasks {NOLOCK} Where Status=@Status", new { Status = status });
            else
                return _context.QuerySingleAsync<int>($"SELECT Count(1) FROM JobTasks {NOLOCK} Where Status<>@Status", new { Status = JobTaskStatus.Deleted });
        }

        public async Task<List<JobTasksEntity>> QueryPagerAsync(PageInput page)
        {
            var orderStr = page.OrderBy ?? "CreateTime";
            var param = new DynamicParameters();
            param.Add("PageIndex", page.PageIndex);
            param.Add("PageSize", page.PageSize);

            var andStr = string.Empty;
            if (page.Extens.Any())
            {
                page.Extens.AsList().ForEach(c => param.Add(c.Key, (c.Key.Equals("Title") ? $"%{c.Value}%" : c.Value)
                ));
                andStr = " AND " + string.Join(" AND ", page.Extens.Select(c => $"{c.Key} {(c.Key.Equals("Title") ? "LIKE" : "=")} @{c.Key}"));
            }
            var total = await _context.ExecuteScalarAsync<int>($"SELECT COUNT(1) FROM  JobTasks Where Status<>-1 {andStr}", param);
            var list = (await _context.QueryAsync<JobTasksEntity>($"SELECT * FROM  JobTasks {NOLOCK} Where Status<>-1 {andStr} ORDER BY {orderStr} {PAGESUFFIX}", param))?.ToList();
            if (list.Any())
                list.ForEach(p => p.Total = total);
            return list;
        }

        public Task<JobTasksEntity> QueryByIdAsync(long sid)
        {
            return _context.QuerySingleAsync<JobTasksEntity>($"SELECT * FROM JobTasks {NOLOCK} Where  Id=@Id", new { Id = sid });
        }

        public async Task<bool> UpdateJobTaskAsync(JobTasksEntity entity)
        {
            return (await _context.ExecuteAsync("UPDATE JobTasks SET AssemblyName=@AssemblyName, ClassName=@ClassName,CronExpression=@CronExpression,CustomParamsJson=@CustomParamsJson,Remark=@Remark,Title=@Title,Children=@Children  WHERE Id=@Id", entity)) > 0;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<bool> DeleteJobTaskAsync(long sid)
        {
            return (await _context.ExecuteAsync("UPDATE JobTasks SET Status=-1, NextRunTime=NULL  WHERE Id=@Id", new { Id = sid })) > 0;
        }

        public async Task<bool> AddAsync(JobTasksEntity entity)
        {
            var sql = $@"INSERT INTO JobTasks
(Id,NodeName,Title,Remark,CronExpression,AssemblyName,ClassName,CustomParamsJson,Status,CreateTime,CreateUserId,CreateUserName,TotalRunCount,Children)
VALUES
(@Id,@NodeName,@Title,@Remark,@CronExpression,@AssemblyName,@ClassName,@CustomParamsJson,@Status,@CreateTime,@CreateUserId,@CreateUserName,0,@Children)";

            return (await _context.ExecuteAsync(sql, entity)) > 0;
        }
    }
}