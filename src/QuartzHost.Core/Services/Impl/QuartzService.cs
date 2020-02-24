using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using QuartzHost.Base;
using QuartzHost.Core.Common;
using QuartzHost.Core.Dao;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuartzHost.Core.Services.Impl
{
    [ServiceMapTo(typeof(IQuartzService), ServiceLifetime.Singleton)]
    public class QuartzService : IQuartzService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// worker访问秘钥
        /// </summary>
        public static string AccessSecret { get; private set; }

        /// <summary>
        /// 调度器实例
        /// </summary>
        private static IScheduler _scheduler = null;

        public QuartzService(NodeSetting nodeSetting, ILogger logger)
        {
            CoreGlobal.NodeSetting = nodeSetting;
            _logger = logger;
        }

        /// <summary>
        /// 初始化调度系统
        /// </summary>
        public async Task<Result<bool>> InitScheduler()
        {
            var result = new Result<bool> { Data = true, Message = "任务调度平台初始化成功！" };

            try
            {
                if (_scheduler == null)
                {
                    NameValueCollection properties = new NameValueCollection();
                    properties["quartz.scheduler.instanceName"] = "QuartzHost.Core";
                    properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                    properties["quartz.threadPool.threadCount"] = "50";
                    properties["quartz.threadPool.threadPriority"] = "Normal";
                    ISchedulerFactory factory = new StdSchedulerFactory(properties);
                    _scheduler = await factory.GetScheduler();
                }
                await _scheduler.Start();
                await _scheduler.Clear();
                await MarkNodeAsync(true);
                _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台初始化成功！");
                await RunningRecoveryAsync();
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Success = false;
                result.Message = $"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台初始化失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台初始化失败！");
            }
            return result;
        }

        /// <summary>
        /// 关闭调度系统
        /// </summary>
        public async Task<Result<bool>> Shutdown(bool isOnStop = false)
        {
            var result = new Result<bool> { Data = true, Message = "任务调度平台已经停止成功！" };
            try
            {
                if (_scheduler == null)
                    result.Message = "任务调度平台未启动无需停止！";
                //判断调度是否已经关闭
                if (_scheduler != null && !_scheduler.IsShutdown)
                {
                    //等待任务运行完成再关闭调度
                    await _scheduler.Shutdown(true);
                    await MarkNodeAsync(false, isOnStop);
                    _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台已经停止！");
                    _scheduler = null;
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Success = false;
                result.Message = $"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台停止失败！";
                result.ErrorDetail = ex.Message;
                _logger.LogError(ex, $"节点[{CoreGlobal.NodeSetting.NodeName}]任务调度平台停止失败！");
            }
            return result;
        }

        /// <summary>
        /// 启动一个任务，带重试机制
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<Result<bool>> StartWithRetry(Guid sid)
        {
            var result = new Result<bool> { Data = true };
            var jk = new JobKey(sid.ToString().ToLower());
            if (await _scheduler.CheckExists(jk))
            {
                result.Message = "任务已存在Scheduler中";
                return result;
            }
            JobTaskView view = await GetJobTaskViewAsync(sid);
            TaskLoadContext lc = null;
            try
            {
                lc = AssemblyHelper.LoadAssemblyContext(view.JobTask.Id, view.JobTask.AssemblyName);
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        await Start(view, lc);
                        result.Message = "任务启动成功！";
                        return result;
                    }
                    catch (SchedulerException sexp)
                    {
                        _logger.LogError(sexp, $"节点[{CoreGlobal.NodeSetting.NodeName}][{view.JobTask.Title}({view.JobTask.Id})]任务启动失败！开始第{i + 1}次重试...");
                    }
                }
                //最后一次尝试
                await Start(view, lc);
            }
            catch (SchedulerException sexp)
            {
                AssemblyHelper.UnLoadAssemblyLoadContext(lc);
                _logger.LogError(sexp, $"节点[{CoreGlobal.NodeSetting.NodeName}][{view.JobTask.Title}({view.JobTask.Id})]任务所有重试都失败了，已放弃启动！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务所有重试都失败了，已放弃启动！";
                result.ErrorDetail = sexp.Message;
            }
            catch (Exception exp)
            {
                AssemblyHelper.UnLoadAssemblyLoadContext(lc);
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][{view.JobTask.Title}({view.JobTask.Id})]任务启动失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务启动失败！";
                result.ErrorDetail = exp.Message;
            }
            return result;
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<Result<bool>> Pause(Guid sid)
        {
            var result = new Result<bool> { Data = true, Message = "任务已经暂停运行！" };
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                if (await _scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    await _scheduler.PauseJob(jk);
                    var jobDetail = await _scheduler.GetJobDetail(jk);
                    if (jobDetail.JobType.GetInterface("IInterruptableJob") != null)
                    {
                        await _scheduler.Interrupt(jk);
                    }
                    _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务已经暂停运行！");
                }
                else
                {
                    result.Data = false;
                    result.Success = false;
                    result.Message = "任务不存在Scheduler中";
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务暂停运行失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务暂停运行失败！";
                result.ErrorDetail = exp.Message;
            }
            return result;
        }

        /// <summary>
        /// 恢复运行
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<Result<bool>> Resume(Guid sid)
        {
            var result = new Result<bool> { Data = true, Message = "任务已经恢复运行！" };
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                if (await _scheduler.CheckExists(jk))
                {
                    //恢复任务继续执行
                    await _scheduler.ResumeJob(jk);
                    _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务已经恢复运行！");
                }
                else
                {
                    result.Data = false;
                    result.Success = false;
                    result.Message = "任务不存在Scheduler中";
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务恢复运行失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务恢复运行失败！";
                result.ErrorDetail = exp.Message;
            }
            return result;
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<Result<bool>> Stop(Guid sid)
        {
            var result = new Result<bool> { Data = true, Message = "任务已经停止运行！" };
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
                }
                _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务已经停止运行！");
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务停止失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务停止失败！";
                result.ErrorDetail = exp.Message;
            }
            return result;
        }

        /// <summary>
        ///立即运行一次任务
        /// </summary>
        /// <param name="sid"></param>
        public async Task<Result<bool>> RunOnce(Guid sid)
        {
            var result = new Result<bool> { Data = true, Message = "任务立即运行成功！" };
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                if (await _scheduler.CheckExists(jk))
                {
                    await _scheduler.TriggerJob(jk);
                    _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务立即运行成功！");
                }
                else
                {
                    result.Data = false;
                    result.Success = false;
                    result.Message = "任务不存在Scheduler中";
                    _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务立即运行失败！任务不存在Scheduler中");
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][({sid})]任务立即运行失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "任务立即运行失败！";
                result.ErrorDetail = exp.Message;
            }

            return result;
        }

        /// <summary>
        /// 执行自定义任务类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <param name="cronExp"></param>
        /// <returns></returns>
        public async Task<Result<bool>> Start<T>(string identity, string cronExp) where T : IJob
        {
            var result = new Result<bool> { Data = true, Message = "执行自定义任务类成功！" };
            try
            {
                IJobDetail job = JobBuilder.Create<T>().WithIdentity(identity).Build();
                CronTriggerImpl trigger = new CronTriggerImpl
                {
                    CronExpressionString = cronExp,
                    Name = identity,
                    Key = new TriggerKey(identity)
                };
                trigger.StartTimeUtc = DateTimeOffset.Now;
                await _scheduler.ScheduleJob(job, trigger);
                _logger.LogInformation($"节点[{CoreGlobal.NodeSetting.NodeName}][({identity})]执行自定义任务类成功！");
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"节点[{CoreGlobal.NodeSetting.NodeName}][({identity})]执行自定义任务类失败！");
                result.Data = false;
                result.Success = false;
                result.Message = "执行自定义任务类失败！";
                result.ErrorDetail = exp.Message;
            }
            return result;
        }

        #region 私有

        /// <summary>
        /// 更新节点状态
        /// </summary>
        /// <param name="isStarted"></param>
        /// <param name="isOnStop"></param>
        private async Task MarkNodeAsync(bool isStarted, bool isOnStop = false)
        {
            using var _quartzDao = new QuartzDao();
            bool isCreate = false;
            bool isSave = false;
            var node = await _quartzDao.QueryJobNodeByIdAsync(CoreGlobal.NodeSetting.NodeName);
            if (isStarted)
            {
                if (node == null)
                {
                    isCreate = true;
                    node = new JobNodesEntity();
                }
                node.NodeName = CoreGlobal.NodeSetting.NodeName;
                node.NodeType = CoreGlobal.NodeSetting.NodeType;
                node.MachineName = Environment.MachineName;
                node.AccessProtocol = CoreGlobal.NodeSetting.Protocol;
                node.Host = $"{CoreGlobal.NodeSetting.IP}:{CoreGlobal.NodeSetting.Port}";
                node.Priority = CoreGlobal.NodeSetting.Priority;
                node.Status = 2;
                node.AccessSecret = Guid.NewGuid().ToString("n");
                isSave = await _quartzDao.UpdateJobNodeStatusAsync(node) > 0;
                if (isCreate)
                    await _quartzDao.AddJobNodeAsync(node);
            }
            else
            {
                if (node != null)
                {
                    node.Status = isOnStop ? 0 : 1;
                    isSave = await _quartzDao.UpdateJobNodeStatusAsync(node) > 0;
                    if (isOnStop) node.AccessSecret = null;
                }
            }
            if (isSave)
            {
                AccessSecret = node.AccessSecret;
            }
            CoreGlobal.NodeSetting.AccessSecret = AccessSecret;
        }

        private async Task RunningRecoveryAsync()
        {
            //任务恢复：查找那些绑定了本节点并且在状态是运行中的任务执行启动操作
            using var _quartzDao = new QuartzDao();
            var list = await _quartzDao.QueryRunningJobTaskIdsAsync(CoreGlobal.NodeSetting.NodeName);
            _logger.LogInformation($"[{CoreGlobal.NodeSetting.NodeName}]任务恢复 {list.ToJson()}");
            list?.AsParallel().ForAll(async sid => await StartWithRetry(sid));
        }

        private async Task<JobTaskView> GetJobTaskViewAsync(Guid sid)
        {
            using var _quartzDao = new QuartzDao();
            var model = await _quartzDao.QueryJobTaskAsync(sid);
            if (model != null)
            {
                JobTaskView view = new JobTaskView() { JobTask = model };

                //await LoadPluginFile(_quartzDao, model);
                return view;
            }
            throw new InvalidOperationException($"不存在的任务id：{sid}");
        }

        private async Task LoadPluginFile(QuartzDao _quartzDao, JobTasksEntity model)
        {
            var master = await _quartzDao.QueryJobNodeByTypeAsync("master");
            if (master == null)
            {
                throw new InvalidOperationException("cannot find master.");
            }
            //var sourcePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "tasks", model.AssemblyName);
        }

        private async Task Start(JobTaskView view, TaskLoadContext lc)
        {
            //throw

            //在应用程序域中创建实例返回并保存在job中，这是最终调用任务执行的实例
            TaskBase instance = AssemblyHelper.CreateTaskInstance(lc, view.JobTask.Id, view.JobTask.AssemblyName, view.JobTask.ClassName);
            if (instance == null)
            {
                throw new InvalidCastException($"任务实例创建失败，请确认目标任务是否派生自TaskBase类型。程序集：{view.JobTask.AssemblyName}，类型：{view.JobTask.ClassName}");
            }
            // instance.logger = new LogWriter(); ;
            JobDataMap map = new JobDataMap
            {
                new KeyValuePair<string, object> ("domain",lc),
                new KeyValuePair<string, object> ("instance",instance),
                new KeyValuePair<string, object> ("name",view.JobTask.Title),
                new KeyValuePair<string, object> ("params",ConvertParamsJson(view.JobTask.CustomParamsJson)),
                new KeyValuePair<string, object> ("keepers",view.Keepers),
                new KeyValuePair<string, object> ("children",view.Children)
            };

            try
            {
                IJobDetail job = JobBuilder.Create<RootJob>()
                    .WithIdentity(view.JobTask.Id.ToString())
                    .UsingJobData(map)
                    .Build();

                //添加触发器
                var listener = new JobRunListener(view.JobTask.Id.ToString());
                listener.OnSuccess += StartedEventAsync;
                _scheduler.ListenerManager.AddJobListener(listener, KeyMatcher<JobKey>.KeyEquals(new JobKey(view.JobTask.Id.ToString())));

                if (!CronExpression.IsValidExpression(view.JobTask.CronExpression))
                {
                    throw new Exception("cron表达式验证失败");
                }
                CronTriggerImpl trigger = new CronTriggerImpl
                {
                    CronExpressionString = view.JobTask.CronExpression,
                    Name = view.JobTask.Title,
                    Key = new TriggerKey(view.JobTask.Id.ToString()),
                    Description = view.JobTask.Remark
                };

                await _scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                throw new SchedulerException(ex);
            }
            _logger.LogInformation($"任务[{view.JobTask.Title}]启动成功！", view.JobTask.Id);
        }

        private async Task StartedEventAsync(Guid sid, DateTime? nextRunTime)
        {
            using var _quartzDao = new QuartzDao();
            //每次运行成功后更新任务的运行情况
            var task = await _quartzDao.QueryJobTaskAsync(sid);
            if (task == null) return;
            task.LastRunTime = DateTime.Now;
            task.NextRunTime = nextRunTime;
            task.TotalRunCount += 1;
            await _quartzDao.UpdateJobTaskAsync(task);
        }

        private Dictionary<string, object> ConvertParamsJson(string source)
        {
            var list = source.ToObj<List<TaskParam>>();
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (list == null || list.Any() == false)
                return result;
            foreach (var item in list)
            {
                result[item.ParamKey] = item.ParamValue;
            }
            return result;
        }

        #endregion 私有
    }

    /// <summary>
    /// 任务运行状态监听器
    /// </summary>
    internal class JobRunListener : IJobListener
    {
        public delegate Task SuccessEventHandler(Guid sid, DateTime? nextTime);

        public string Name { get; set; }

        public event SuccessEventHandler OnSuccess;

        public JobRunListener()
        {
        }

        public JobRunListener(string name)
        {
            Name = name;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken)
        {
            IJobDetail job = context.JobDetail;

            if (jobException == null)
            {
                var utcDate = context.Trigger.GetNextFireTimeUtc();
                DateTime? nextTime = utcDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value.DateTime, TimeZoneInfo.Local) : new DateTime?();
                OnSuccess(Guid.Parse(job.Key.Name), nextTime);

                //子任务触发
                Task.Run(async () =>
                {
                    var children = job.JobDataMap["children"] as Dictionary<Guid, string>;
                    foreach (var item in children)
                    {
                        var jobkey = new JobKey(item.Key.ToString());
                        if (await context.Scheduler.CheckExists(jobkey))
                        {
                            JobDataMap map = new JobDataMap{
                                 new KeyValuePair<string, object>("PreviousResult", context.Result)
                             };
                            await context.Scheduler.TriggerJob(jobkey, map);
                        }
                    }
                });
            }
            else if (jobException is BusinessRunException)
            {
                Task.Run(() =>
                {
                    var name = job.JobDataMap["name"] as string;
                    var users = job.JobDataMap["keepers"] as List<KeyValuePair<string, string>>;
                    if (users != null && users.Any())
                    {
                        //MailKitHelper.SendMail(users, $"任务运行异常 — {name}", QuartzManager.GetErrorEmailContent(name, (jobException as BusinessRunException).Detail));
                    }
                });
            }
            return Task.FromResult(0);
        }
    }
}