using DG.Dapper;

using QuartzHost.Core.Common;
using QuartzHost.Core.Models;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Dao
{
    public class QuartzDao : IDisposable
    {
        private readonly DGContext _context;
        private string NOLOCK = "WITH(NOLOCK)";

        public QuartzDao()
        {
            _context = new DGContext();

            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
            {
                //PAGESUFFIX = "LIMIT @PageSize OFFSET (@PageIndex-1)*@PageSize";
                NOLOCK = "";
                DGContext.Init(SQLiteFactory.Instance, CoreGlobal.NodeSetting.ConnStr);
            }
            else
                DGContext.Init(SqlClientFactory.Instance, CoreGlobal.NodeSetting.ConnStr);
        }

        public async Task<JobNodesEntity> QueryJobNodeByIdAsync(string nodeName)
        {
            //var p = await AddAsync(new JobTasksEntity { Id = Guid.NewGuid(), NodeName = nodeName, Title = "vvip", Remark = "ttvvip", CronExpression = "0/3 * * * * ?", AssemblyName = "VIP", ClassName = "VIP.Test", Status = JobTaskStatus.Running, CreateUserId = 1, CreateUserName = "hhh" });
            return await _context.QuerySingleAsync<JobNodesEntity>($"SELECT  * FROM JobNodes {NOLOCK} WHERE NodeName=@NodeName", new { NodeName = nodeName });
        }

        public async Task<bool> AddAsync(JobTasksEntity entity)
        {
            var ids = entity.Id.ToString().ToUpper();
            var sql = $@"INSERT INTO JobTasks
(Id,NodeName,Title,Remark,CronExpression,AssemblyName,ClassName,CustomParamsJson,Status,CreateTime,CreateUserId,CreateUserName,TotalRunCount)
VALUES
(@Id,@NodeName,@Title,@Remark,@CronExpression,@AssemblyName,@ClassName,@CustomParamsJson,@Status,@CreateTime,@CreateUserId,@CreateUserName,0)";
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
                return (await _context.ExecuteAsync(sql, new { Id = ids, entity.NodeName, entity.Title, entity.Remark, entity.CronExpression, entity.AssemblyName, entity.ClassName, entity.CustomParamsJson, entity.Status, entity.CreateTime, entity.CreateUserId, entity.CreateUserName })) > 0;
            else
                return (await _context.ExecuteAsync(sql, entity)) > 0;
        }

        public Task<JobNodesEntity> QueryJobNodeByTypeAsync(string nodeType)
        {
            return _context.QuerySingleAsync<JobNodesEntity>($"SELECT * FROM JobNodes {NOLOCK} WHERE NodeType=@NodeType", new { NodeType = nodeType });
        }

        public async Task<bool> AddJobNodeAsync(JobNodesEntity node)
        {
            return await _context.ExecuteAsync("INSERT INTO  JobNodes (NodeName,NodeType,MachineName,AccessProtocol,Host,AccessSecret,LastUpdateTime,Status,Priority)VALUES (@NodeName,@NodeType,@MachineName,@AccessProtocol,@Host,@AccessSecret,@LastUpdateTime,@Status,@Priority)", node) == 1;
        }

        public Task<int> UpdateJobNodeStatusAsync(JobNodesEntity node)
        {
            return _context.ExecuteAsync("UPDATE JobNodes SET NodeName=@NodeName,NodeType=@NodeType,MachineName=@MachineName,AccessProtocol=@AccessProtocol,Host=@Host,AccessSecret=@AccessSecret,LastUpdateTime=@LastUpdateTime,Status=@Status,Priority=@Priority  WHERE NodeName=@NodeName", node);
        }

        public Task<IEnumerable<Guid>> QueryRunningJobTaskIdsAsync(string nodeName)
        {
            return _context.QueryAsync<Guid>($"SELECT Id FROM JobTasks {NOLOCK} WHERE NodeName=@NodeName AND Status=@Status", new { NodeName = nodeName, Status = JobTaskStatus.Running });
        }

        public Task<IEnumerable<Guid>> QueryStopJobTaskIdsAsync(string nodeName)
        {
            return _context.QueryAsync<Guid>($"SELECT Id FROM JobTasks {NOLOCK} WHERE NodeName=@NodeName AND Status=@Status", new { NodeName = nodeName, Status = JobTaskStatus.Stop });
        }

        public Task<JobTasksEntity> QueryJobTaskAsync(Guid id)
        {
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
                return _context.QuerySingleAsync<JobTasksEntity>($"SELECT  * FROM JobTasks {NOLOCK} WHERE Id=@Id AND Status<>@Status", new { Id = id.ToString().ToUpper(), Status = JobTaskStatus.Deleted });
            else
                return _context.QuerySingleAsync<JobTasksEntity>($"SELECT  * FROM JobTasks {NOLOCK} WHERE Id=@Id AND Status<>@Status", new { Id = id, Status = JobTaskStatus.Deleted });
        }

        public Task<int> UpdateJobTaskAsync(JobTasksEntity entity)
        {
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
                return _context.ExecuteAsync("UPDATE JobTasks SET LastRunTime=@LastRunTime,NextRunTime=@NextRunTime,TotalRunCount=@TotalRunCount WHERE Id=@Id", new { entity.LastRunTime, entity.NextRunTime, entity.TotalRunCount, Id = entity.Id.ToString().ToUpper() });
            else
                return _context.ExecuteAsync("UPDATE JobTasks SET LastRunTime=@LastRunTime,NextRunTime=@NextRunTime,TotalRunCount=@TotalRunCount WHERE Id=@Id", entity);
        }

        public async Task<Guid> GreateRunTrace(Guid tid, string node)
        {
            JobTraceEntity entity = new JobTraceEntity
            {
                TraceId = Guid.NewGuid(),
                TaskId = tid,
                Node = node,
                StartTime = DateTime.Now,
                Result = TaskRunResult.Null
            };
            var ttid = entity.TraceId.ToString().ToUpper();
            var kid = entity.TaskId.ToString().ToUpper();
            int execint;
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
                execint = await _context.ExecuteAsync("INSERT  INTO JobTrace (TraceId,TaskId,Node,StartTime,ElapsedTime,Result)VALUES(@TraceId,@TaskId,@Node,@StartTime,@ElapsedTime,@Result)", new
                { TraceId = entity.TraceId.ToString().ToUpper(), TaskId = entity.TaskId.ToString().ToUpper(), entity.Node, entity.StartTime, entity.ElapsedTime, entity.Result });
            else
                execint = await _context.ExecuteAsync("INSERT  INTO JobTrace (TraceId,TaskId,Node,StartTime,ElapsedTime,Result)VALUES(@TraceId,@TaskId,@Node,@StartTime,@ElapsedTime,@Result)", entity);
            if (execint > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        public Task<int> UpdateRunTrace(Guid traceId, double elapsed, TaskRunResult result)
        {
            var ttid = traceId.ToString().ToUpper();
            if (traceId == Guid.Empty) return default;
            if (CoreGlobal.NodeSetting.DbType.Equals("Sqlite"))
                return _context.ExecuteAsync("UPDATE JobTrace SET Result=@Result,ElapsedTime=@ElapsedTime,EndTime=@EndTime WHERE TraceId=@TraceId", new { Result = result, ElapsedTime = elapsed, TraceId = traceId.ToString().ToUpper(), EndTime = DateTime.Now });
            else
                return _context.ExecuteAsync("UPDATE JobTrace SET Result=@Result,ElapsedTime=@ElapsedTime,EndTime=@EndTime WHERE TraceId=@TraceId", new { Result = result, ElapsedTime = elapsed, TraceId = traceId, EndTime = DateTime.Now });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}