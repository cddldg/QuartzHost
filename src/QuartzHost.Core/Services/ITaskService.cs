using QuartzHost.Contract.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHost.Core.Services
{
    public interface ITaskService
    {
        /// <summary>
        /// 查询所有Dict
        /// </summary>
        /// <param name="status">默认查询非删除的</param>
        /// <returns></returns>
        Task<Result<List<JobDictEntity>>> QueryDictAllAsync(DictType? type);

        /// <summary>
        /// 查询所有任务
        /// </summary>
        /// <param name="status">默认查询非删除的</param>
        /// <returns></returns>
        Task<Result<List<JobTasksEntity>>> QueryAllAsync(JobTaskStatus? status = null);

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
        Task<Result<JobTasksEntity>> QueryById(long sid);

        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<ResultStatus>> EditAsync(JobTasksInput input);

        /// <summary>
        /// 查询指定状态的任务数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Result<int>> QueryAllCountAsync(JobTaskStatus? status);

        /// <summary>
        /// 查询所有Nodes列表
        /// </summary>
        /// <returns></returns>
        Task<Result<List<JobNodesEntity>>> QueryNodesAll();

        /// <summary>
        /// 查询指定worker状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryWorkerCount(int? status);

        /// <summary>
        /// 查询指定运行状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryTraceCount(int? status);

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        Task<Result<long>> AddAsync(JobTasksInput input);

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<Result<ResultStatus>> DeleteTask(long sid);

        Task<PageResult<List<JobTraceEntity>>> QueryTracesAsync(PageInput pager);
    }
}