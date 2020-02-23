using Quartz;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Services
{
    public interface IQuartzService
    {
        Task<Result<bool>> InitScheduler();

        /// <summary>
        /// 关闭调度系统
        /// </summary>
        Task<Result<bool>> Shutdown(bool isOnStop = false);

        /// <summary>
        /// 启动一个任务，带重试3机制
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<Result<bool>> StartWithRetry(Guid sid);

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<Result<bool>> Pause(Guid sid);

        /// <summary>
        /// 恢复运行
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<Result<bool>> Resume(Guid sid);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<Result<bool>> Stop(Guid sid);

        /// <summary>
        ///立即运行一次任务
        /// </summary>
        /// <param name="sid"></param>
        Task<Result<bool>> RunOnce(Guid sid);

        /// <summary>
        /// 执行自定义任务类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <param name="cronExp"></param>
        /// <returns></returns>
        Task<Result<bool>> Start<T>(string identity, string cronExp) where T : IJob;
    }
}