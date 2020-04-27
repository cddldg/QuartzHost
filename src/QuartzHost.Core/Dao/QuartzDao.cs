using DG.Dapper;

using QuartzHost.Core.Common;
using QuartzHost.Contract.Models;

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

        public Task<JobNodesEntity> QueryJobNodeByIdAsync(string nodeName)
        {
            return _context.QuerySingleAsync<JobNodesEntity>($"SELECT  * FROM JobNodes {NOLOCK} WHERE NodeName=@NodeName", new { NodeName = nodeName });
        }

        public Task<JobNodesEntity> QueryJobNodeByTypeAsync(string nodeType)
        {
            return _context.QuerySingleAsync<JobNodesEntity>($"SELECT * FROM JobNodes {NOLOCK} WHERE NodeType=@NodeType", new { NodeType = nodeType });
        }

        public Task<int> QueryJobNodeMaxGId()
        {
            return _context.ExecuteScalarAsync<int>("SELECT max(GenerID) FROM JobNodes;");
        }

        public async Task<bool> AddJobNodeAsync(JobNodesEntity node)
        {
            return await _context.ExecuteAsync("INSERT INTO  JobNodes (NodeName,NodeType,MachineName,AccessProtocol,Host,AccessSecret,LastUpdateTime,Status,Priority,GenerID)VALUES (@NodeName,@NodeType,@MachineName,@AccessProtocol,@Host,@AccessSecret,@LastUpdateTime,@Status,@Priority,@GenerID)", node) == 1;
        }

        public Task<int> UpdateJobNodeStatusAsync(JobNodesEntity node)
        {
            return _context.ExecuteAsync("UPDATE JobNodes SET NodeName=@NodeName,NodeType=@NodeType,MachineName=@MachineName,AccessProtocol=@AccessProtocol,Host=@Host,AccessSecret=@AccessSecret,LastUpdateTime=@LastUpdateTime,Status=@Status,Priority=@Priority  WHERE NodeName=@NodeName", node);
        }

        public Task<IEnumerable<long>> QueryRunningJobTaskIdsAsync(string nodeName)
        {
            return _context.QueryAsync<long>($"SELECT Id FROM JobTasks {NOLOCK} WHERE NodeName=@NodeName AND Status=@Status", new { NodeName = nodeName, Status = JobTaskStatus.Running });
        }

        public Task<IEnumerable<long>> QueryStopJobTaskIdsAsync(string nodeName)
        {
            return _context.QueryAsync<long>($"SELECT Id FROM JobTasks {NOLOCK} WHERE NodeName=@NodeName AND Status=@Status", new { NodeName = nodeName, Status = JobTaskStatus.Stop });
        }

        public Task<JobTasksEntity> QueryJobTaskAsync(long id)
        {
            return _context.QuerySingleAsync<JobTasksEntity>($"SELECT  * FROM JobTasks {NOLOCK} WHERE Id=@Id AND Status<>@Status", new { Id = id, Status = JobTaskStatus.Deleted });
        }

        public Task<int> UpdateJobTaskAsync(JobTasksEntity entity)
        {
            return _context.ExecuteAsync("UPDATE JobTasks SET LastRunTime=@LastRunTime,NextRunTime=@NextRunTime,TotalRunCount=@TotalRunCount,Status=@Status WHERE Id=@Id", entity);
        }

        public async Task<bool> UpdateJobTaskStatusAsync(long sid, JobTaskStatus status, bool clearNextRunTime = false, int count = 0)
        {
            return (await _context.ExecuteAsync($"UPDATE JobTasks SET Status=@Status {(clearNextRunTime ? ",NextRunTime=NULL" : "")} {(count > 1 ? ",TotalRunCount=TotalRunCount+1,LastRunTime=@LastRunTime" : "")} WHERE Id=@Id", new { Status = status, Id = sid, LastRunTime = DateTime.Now })) > 0;
        }

        public async Task<long> GreateRunTrace(long tid, string node)
        {
            JobTraceEntity entity = new JobTraceEntity
            {
                TraceId = CoreGlobal.SnowflakeUniqueId(),
                TaskId = tid,
                Node = node,
                StartTime = DateTime.Now,
                Result = TaskRunResult.Null
            };
            var sql = "INSERT  INTO JobTrace (TraceId,TaskId,Node,StartTime,ElapsedTime,Result)VALUES(@TraceId,@TaskId,@Node,@StartTime,@ElapsedTime,@Result)";
            if ((await _context.ExecuteAsync(sql, entity)) > 0)
            {
                return entity.TraceId;
            }
            return 0;
        }

        public Task<int> UpdateRunTrace(long traceId, double elapsed, TaskRunResult result)
        {
            var de = decimal.Parse(elapsed.ToString("f4"));

            if (traceId == 0) return default;

            return _context.ExecuteAsync("UPDATE JobTrace SET Result=@Result,ElapsedTime=@ElapsedTime,EndTime=@EndTime WHERE TraceId=@TraceId", new { Result = result, ElapsedTime = de, TraceId = traceId, EndTime = DateTime.Now });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}