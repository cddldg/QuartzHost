using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzHost.Contract.Models
{
    public class JobDictEntity : IEntity
    {
        /// <summary>
        /// 节点标识
        /// </summary>
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
        public DictType Type { get; set; }
        public int Sort { get; set; }
        public string Extension { get; set; }
        public string Remark { get; set; }
    }

    public enum DictType
    {
        [Description("未知类型")]
        Unknown = -1,

        /// <summary>
        /// 系统设置
        /// </summary>
        [Description("系统设置")]
        System = 0,

        /// <summary>
        /// 邮箱设置
        /// </summary>
        [Description("API设置")]
        API = 1,

        /// <summary>
        /// 运行中
        /// </summary>
        [Description("网站设置")]
        Web = 2,

        /// <summary>
        /// 已暂停
        /// </summary>
        [Description("邮箱设置")]
        Email = 3
    }
}