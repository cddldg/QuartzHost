using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzHost.Contract.Models
{
    public class JobUserEntity : IEntity
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string RealName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(500), EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }

        [Required]
        public UserStatus Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? LastLoginTime { get; set; }
    }

    public enum UserStatus
    {
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = -1,

        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Disabled = 0,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Available = 1
    }
}