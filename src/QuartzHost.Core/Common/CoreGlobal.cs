using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UniqueIdGenerator.Net;

namespace QuartzHost.Core.Common
{
    public static class CoreGlobal
    {
        public static NodeSetting NodeSetting;
        public static Generator Generator;

        /// <summary>
        ///     Snowflake获取唯一数字序列
        /// </summary>
        /// <returns></returns>
        public static long SnowflakeUniqueId()
        {
            return (long)Generator.NextLong();
        }
    }
}