using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuartzHost.Contract.Models
{
    /// <summary>
    /// 基础结果返回
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// 创建指定泛型类型参数指定的类型的实例(使用这种方法可以不对泛型类型T做限制，即不用添加：where T:new())
        /// </summary>
        //private T t = System.Activator.CreateInstance<T>();
        public T Data { get; set; }

        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public string ErrorDetail { get; set; }
        public DateTime ServiceTime { get; set; } = DateTime.Now;
    }

    public class PageResult<T> : Result<T>
    {
        /// <summary>
        /// 总数据量
        /// </summary>
        public int Total { get; set; }
    }

    public class Input
    {
        public Dictionary<string, string> Extens { get; set; } = new Dictionary<string, string>();
    }

    public class PageInput : Input
    {
        /// <summary>
        ///     每页大小默认20
        /// </summary>

        [Range(1, 100, ErrorMessage = "每页最多显示1-100条数据")]
        public int PageSize { get; set; } = 20;

        /// <summary>
        ///     页码
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        public string OrderBy { get; set; }
    }
}