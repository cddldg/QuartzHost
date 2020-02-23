using Microsoft.Extensions.Logging;
using Quartz;
using QuartzHost.Base;
using QuartzHost.Core.Dao;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Common
{
    /// <summary>
    /// 这个是quartz直接调用的公共job
    /// </summary>
    //禁止多实例并发执行
    [DisallowConcurrentExecution]
    public class RootJob : IJob
    {
        private Guid _sid;
        private readonly ILogger _logger = DG.Logger.DGLogManager.GetLogger();
        private string node = CoreGlobal.NodeSetting.IdentityName;

        public async Task Execute(IJobExecutionContext context)
        {
            _sid = Guid.Parse(context.JobDetail.Key.Name);
            using (var _quartzDao = new QuartzDao())
            {
                var getLocked = true;
                if (getLocked)
                {
                    IJobDetail job = context.JobDetail;
                    _logger.LogInformation($"节点[{node}] 准备执行任务[{job.JobDataMap["name"]}({_sid})]....");
                    try
                    {
                        if (job.JobDataMap["instance"] is TaskBase instance)
                        {
                            Guid traceId = await _quartzDao.GreateRunTrace(_sid, node);
                            Stopwatch stopwatch = new Stopwatch();
                            TaskContext tctx = new TaskContext(instance)
                            {
                                NodeName = node,
                                Title = $"{job.JobDataMap["name"]}",
                                TaskId = _sid,
                                TraceId = traceId,
                                ParamsDict = job.JobDataMap["params"] as Dictionary<string, object>
                            };
                            if (context.MergedJobDataMap["PreviousResult"] is object prev)
                            {
                                tctx.PreviousResult = prev;
                            }
                            try
                            {
                                stopwatch.Restart();
                                await instance.InnerRun(tctx);
                                stopwatch.Stop();
                                await _quartzDao.UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), Models.TaskRunResult.Success);
                                _logger.LogInformation($"任务[{job.JobDataMap["name"]}({_sid})]运行成功！{traceId} 用时{stopwatch.Elapsed.TotalMilliseconds.ToString()}ms");
                                //保存运行结果用于子任务触发
                                context.Result = tctx.Result;
                            }
                            catch (RunConflictException conflict)
                            {
                                stopwatch.Stop();
                                await _quartzDao.UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), Models.TaskRunResult.Conflict);
                                throw conflict;
                            }
                            catch (Exception e)
                            {
                                stopwatch.Stop();
                                await _quartzDao.UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), Models.TaskRunResult.Failed);
                                _logger.LogError(e, $"任务[{job.JobDataMap["name"]}({_sid})]运行失败！{traceId} ");
                                //这里抛出的异常会在JobListener的JobWasExecuted事件中接住
                                //如果吃掉异常会导致程序误以为本次任务执行成功
                                throw new BusinessRunException(e);
                            }
                        }
                    }
                    finally
                    {
                        //为了避免各节点之间的时间差，延迟1秒释放锁
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                else
                {
                    //LogHelper.Info($"节点{node}抢锁失败！", _sid);
                    //throw new JobExecutionException("lock_failed");
                }
            }
            //return Task.FromResult(0);
        }
    }

    public class BusinessRunException : JobExecutionException
    {
        public Exception Detail;

        public BusinessRunException(Exception exp)
        {
            Detail = exp;
        }
    }
}