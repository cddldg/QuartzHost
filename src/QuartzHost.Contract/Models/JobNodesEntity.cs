using QuartzHost.Contract.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzHost.Contract.Models
{
    public class JobNodesEntity : IEntity
    {
        /// <summary>
        /// 节点标识
        /// </summary>
        [Key]
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型 master/worker
        /// </summary>
        [Required]
        public string NodeType { get; set; }

        /// <summary>
        /// 所在机器
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 访问协议，http/https
        /// </summary>
        [Required]
        public string AccessProtocol { get; set; }

        /// <summary>
        /// 节点主机(IP+端口)
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// 访问秘钥，每次节点激活时会更新，用来验证访问权限
        /// </summary>
        public string AccessSecret { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 节点状态，0-下线，1-停机，2-运行
        /// </summary>
        public NodeStatus Status { get; set; }

        public string StatusName { get => Status.GetDescription(); }

        /// <summary>
        /// 权重
        /// </summary>
        public int Priority { get; set; }

        public int GenerID { get; set; }
    }

    public enum NodeStatus
    {
        /// <summary>
        /// 下线
        /// </summary>
        [Description("下线")]
        Down = 0,

        /// <summary>
        /// 停机
        /// </summary>
        [Description("停机")]
        Off = 1,

        /// <summary>
        /// 运行
        /// </summary>
        [Description("运行")]
        Run = 2
    }
}