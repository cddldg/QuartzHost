using DG.Dapper;
using Microsoft.Extensions.Logging;
using Quartz;
using QuartzHost.Base;
using QuartzHost.Core.Dao;
using QuartzHost.Core.Models;
using QuartzHost.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Common
{
    /// <summary>
    /// 清理那些已经停止但是quartz里面还在运行的任务
    /// </summary>
    public class TaskClearJob : IJob
    {
        private readonly ILogger _logger = DG.Logger.DGLogManager.GetLogger();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                using var _quartzDao = new QuartzDao();

                var stoppedList = await _quartzDao.QueryStopJobTaskIdsAsync(CoreGlobal.NodeSetting.NodeName);

                foreach (var sid in stoppedList)
                {
                    var result = await Stop(sid, context.Scheduler);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台清理失败！{ex.Message}");
            }
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<Result<bool>> Stop(long sid, IScheduler _scheduler)
        {
            var result = new Result<bool> { Data = true, Message = "清理一个任务成功！" };
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                var job = await _scheduler.GetJobDetail(jk);
                if (job != null)
                {
                    //释放资源
                    if (job.JobDataMap["instance"] is TaskBase instance)
                    {
                        instance.Dispose();
                    }
                    //卸载应用程序域
                    var domain = job.JobDataMap["domain"] as TaskLoadContext;
                    AssemblyHelper.UnLoadAssemblyLoadContext(domain);
                    //删除quartz有关设置
                    var trigger = new TriggerKey(sid.ToString());
                    await _scheduler.PauseTrigger(trigger);
                    await _scheduler.UnscheduleJob(trigger);
                    await _scheduler.DeleteJob(jk);
                    _scheduler.ListenerManager.RemoveJobListener(sid.ToString());
                    _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]清理一个任务成功！");
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]清理一个任务失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务停止失败！";
                result.ErrorDetail = exp.Message;
            }
            return result;
        }
    }
}