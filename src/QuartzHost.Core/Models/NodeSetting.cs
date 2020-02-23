using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzHost.Core.Models
{
    public class NodeSetting
    {
        public string IdentityName { get; set; }

        public string Role { get; set; }

        public string Protocol { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public int Priority { get; set; }
        public string ConnStr { get; set; }

        /// <summary>
        /// worker访问秘钥
        /// </summary>
        public string AccessSecret { get; set; }
    }
}