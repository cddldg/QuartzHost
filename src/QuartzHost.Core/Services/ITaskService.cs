using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Services
{
    public interface ITaskService
    {
        /// <summary>
        /// 查询所有任务
        /// </summary>
        /// <param name="status">默认查询非删除的</param>
        /// <returns></returns>
        Task<Result<IEnumerable<JobTasksEntity>>> QueryAllAsync(JobTaskStatus? status = null);

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>

        Task<PageResult<List<JobTasksEntity>>> QueryPagerAsync(PageInput pager);

        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        JobTasksEntity QueryById(long sid);

        /// <summary>
        /// 查询任务详细信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        JobTaskView QueryScheduleView(long sid);

        /// <summary>
        /// 查看指定用户的监护任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="takeSize"></param>
        /// <returns></returns>
        List<JobTasksEntity> QueryUserSchedule(int userId, int takeSize);

        /// <summary>
        /// 查询指定状态的任务数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryScheduleCount(int? status);

        /// <summary>
        /// 查询指定worker状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryWorkerCount(int? status);

        /// <summary>
        /// 查询所有worker列表
        /// </summary>
        /// <returns></returns>
        List<JobNodesEntity> QueryWorkerList();

        /// <summary>
        /// 查询指定运行状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryTraceCount(int? status);

        /// <summary>
        /// 查询运行情况周报表
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        List<KeyValuePair<long, int>> QueryTraceWeeklyReport(int? status);

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        Task<Result<bool>> AddAsync(JobTasksInput input);

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage Edit(JobTasksEntity model);

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage Start(JobTasksEntity task);

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Pause(long sid);

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Resume(long sid);

        /// <summary>
        /// 执行一次任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage RunOnce(long sid);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Stop(long sid);

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Delete(long sid);
    }
}