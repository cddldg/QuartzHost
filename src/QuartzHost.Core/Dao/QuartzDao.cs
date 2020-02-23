using DG.Dapper;
using QuartzHost.Core.Common;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Dao
{
    public class QuartzDao : IDisposable
    {
        private readonly DGContext _context;

        public QuartzDao()
        {
            _context = new DGContext();
            DGContext.Init(SqlClientFactory.Instance, CoreGlobal.NodeSetting.ConnStr);
        }

        public Task<JobNodesEntity> QueryJobNodeByIdAsync(string nodeName)
        {
            return _context.QuerySingleAsync<JobNodesEntity>("SELECT TOP 1 * FROM dbo.JobNodes WITH(NOLOCK) WHERE NodeName=@NodeName", new { NodeName = nodeName });
        }

        public Task<JobNodesEntity> QueryJobNodeByTypeAsync(string nodeType)
        {
            return _context.QuerySingleAsync<JobNodesEntity>("SELECT * FROM dbo.JobNodes WITH(NOLOCK) WHERE NodeType=@NodeType", new { NodeType = nodeType });
        }

        public async Task<bool> AddJobNodeAsync(JobNodesEntity node)
        {
            return await _context.ExecuteAsync("INSERT dbo.JobNodes (NodeName,NodeType,MachineName,AccessProtocol,Host,AccessSecret,LastUpdateTime,Status,Priority)VALUES (@NodeName,@NodeType,@MachineName,@AccessProtocol,@Host,@AccessSecret,@LastUpdateTime,@Status,@Priority)", node) == 1;
        }

        public Task<int> UpdateJobNodeStatusAsync(JobNodesEntity node)
        {
            return _context.ExecuteAsync("UPDATE dbo.JobNodes SET NodeName=@NodeName,NodeType=@NodeType,MachineName=@MachineName,AccessProtocol=@AccessProtocol,Host=@Host,AccessSecret=@AccessSecret,LastUpdateTime=SYSDATETIME(),Status=@Status,Priority=@Priority  WHERE NodeName=@NodeName", node);
        }

        public Task<IEnumerable<Guid>> QueryRunningJobTaskIdsAsync(string nodeName)
        {
            return _context.QueryAsync<Guid>("SELECT Id FROM dbo.JobTasks WITH(NOLOCK) WHERE NodeName=@NodeName AND Status=@Status", new { NodeName = nodeName, Status = JobTaskStatus.Running });
        }

        public Task<IEnumerable<Guid>> QueryStopJobTaskIdsAsync(string nodeName)
        {
            return _context.QueryAsync<Guid>("SELECT Id FROM dbo.JobTasks WITH(NOLOCK) WHERE NodeName=@NodeName AND Status=@Status", new { NodeName = nodeName, Status = JobTaskStatus.Stop });
        }

        public Task<JobTasksEntity> QueryJobTaskAsync(Guid id)
        {
            return _context.QuerySingleAsync<JobTasksEntity>("SELECT TOP 1 * FROM dbo.JobTasks WITH(NOLOCK) WHERE Id=@Id AND Status<>@Status", new { Id = id, Status = JobTaskStatus.Deleted });
        }

        public Task<int> UpdateJobTaskAsync(JobTasksEntity entity)
        {
            return _context.ExecuteAsync("UPDATE dbo.JobTasks SET LastRunTime=@LastRunTime,NextRunTime=@NextRunTime,TotalRunCount=@TotalRunCount WHERE Id=@Id", entity);
        }

        public async Task<Guid> GreateRunTrace(Guid tid, string node)
        {
            JobTraceEntity entity = new JobTraceEntity();
            entity.TraceId = Guid.NewGuid();
            entity.TaskId = tid;
            entity.Node = node;
            entity.StartTime = DateTime.Now;
            entity.Result = TaskRunResult.Null;
            ;
            if ((await _context.ExecuteAsync("INSERT dbo.JobTrace (TraceId,TaskId,Node,StartTime,ElapsedTime,Result)VALUES(@TraceId,@TaskId,@Node,SYSDATETIME(),@ElapsedTime,@Result)", entity)) > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        public Task<int> UpdateRunTrace(Guid traceId, double elapsed, TaskRunResult result)
        {
            if (traceId == Guid.Empty) return default;
            return _context.ExecuteAsync("UPDATE dbo.JobTrace SET Result=@Result,ElapsedTime=@ElapsedTime,EndTime=SYSDATETIME() WHERE TraceId=@TraceId", new { Result = result, ElapsedTime = elapsed, TraceId = traceId });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}