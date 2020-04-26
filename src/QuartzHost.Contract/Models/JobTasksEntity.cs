using QuartzHost.Contract.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzHost.Contract.Models
{
    public class JobTasksEntity : BaseEntity, IEntity
    {
        /// <summary>
        /// 任务id
        /// </summary>

        public long Id { get; set; }

        /// <summary>
        /// 节点标识
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// cron表达式
        /// </summary>
        [MaxLength(50)]
        public string CronExpression { get; set; }

        /// <summary>
        /// 任务所在程序集
        /// </summary>
        [Required, MaxLength(200)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务的类型（完整命名空间.ClassName）
        /// </summary>
        [Required, MaxLength(200)]
        public string ClassName { get; set; }

        /// <summary>
        /// 自定义参数（json格式）
        /// </summary>
        [MaxLength(2000)]
        public string CustomParamsJson { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        [Required]
        public JobTaskStatus Status { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public string StatusName { get => Status.GetDescription(); }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreateUserId { get; set; }

        /// <summary>
        /// 创建人账号
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>
        public DateTime? NextRunTime { get; set; }

        /// <summary>
        /// 总运行成功次数
        /// </summary>
        public int TotalRunCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Children { get; set; }

        /// <summary>
        /// 子任务Id
        /// </summary>
        public long[] Nexts { get; set; }
    }

    /// <summary>
    /// 添加任务接收参数
    /// </summary>
    public class JobTasksInput
    {
        public JobTasksEntity JobTasks { get; set; }

        /// <summary>
        /// Todo 监护人 提醒
        /// </summary>
        public List<int> Keepers { get; set; }

        /// <summary>
        /// 子任务
        /// </summary>
        public List<long> Nexts { get; set; }

        /// <summary>
        /// Todo 多节点运行
        /// </summary>
        public List<string> Executors { get; set; }

        /// <summary>
        /// 创建并运行
        /// </summary>
        public bool RunNow { get; set; }
    }

    public class JobTaskView
    {
        public JobTasksEntity JobTask { get; set; }

        public List<TaskParam> Params
        {
            get
            {
                return JobTask.CustomParamsJson.ToObj<List<TaskParam>>();
            }
        }

        public List<KeyValuePair<string, string>> Keepers { get; set; }

        public Dictionary<long, string> Children { get; set; }

        public List<string> Executors { get; set; }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum JobTaskStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = -1,

        /// <summary>
        /// 已停止
        /// </summary>
        [Description("已停止")]
        Stop = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Running = 1,

        /// <summary>
        /// 已暂停
        /// </summary>
        [Description("已暂停")]
        Paused = 2
    }
}