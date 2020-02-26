using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzHost.Base
{
    /// <summary>
    /// 任务运行时的上下文
    /// </summary>
    public class TaskContext
    {
        private TaskBase _instance;

        public TaskContext(TaskBase instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// 所在节点
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 运行轨迹
        /// </summary>
        public long TraceId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public Dictionary<string, object> ParamsDict { get; set; }

        /// <summary>
        /// 前置任务的运行结果
        /// </summary>
        public object PreviousResult { get; set; }

        /// <summary>
        /// 本次运行的返回结果
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 获取自定义参数字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetArgument<T>(string name)
        {
            if (ParamsDict == null)
            {
                return default;
            }
            try
            {
                object value;
                ParamsDict.TryGetValue(name, out value);
                //dynamic parseObj = JsonConvert.DeserializeObject<dynamic>(ParamsDict);
                //var value = parseObj.GetType().GetField(name).GetValue(parseObj);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}