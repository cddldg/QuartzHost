using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Common
{
    public static class JSRuntimeExtensions
    {
        private const string ClassName = "QHUI";

        /// <summary>
        /// 执行js
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="funName"></param>
        /// <param name="args"></param>
        public static void DoVoid(this IJSRuntime jsRuntime, string funName, object[] args = null)
        {
            if (jsRuntime != null) jsRuntime.InvokeVoidAsync($"{ClassName}.{funName}", args);
        }

        /// <summary>
        /// 执行js
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="funName"></param>
        /// <param name="args"></param>
        public static ValueTask DoVoidAsync(this IJSRuntime jsRuntime, string funName, object[] args = null)
        {
            if (jsRuntime != null)
                return jsRuntime.InvokeVoidAsync($"{ClassName}.{funName}", args);
            else
                return default;
        }

        /// <summary>
        /// 执行js有返回
        /// </summary>
        /// <param name="jSRuntime"></param>
        /// <returns></returns>
        public static async ValueTask<string> InvokeString(this IJSRuntime jSRuntime, string funName, object[] args = null) => jSRuntime == null ? "" : await jSRuntime.InvokeAsync<string>($"{ClassName}.{funName}", args);
    }
}