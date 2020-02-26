using Microsoft.Extensions.Logging;
using QuartzHost.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuartzHost.Core.Common
{
    public class AssemblyHelper
    {
        private static readonly ILogger _logger = DG.Logger.DGLogManager.GetLogger();

        public static Type GetClassType(string assemblyPath, string className)
        {
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
                Type type = assembly.GetType(className, true, true);
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T CreateInstance<T>(Type type) where T : class
        {
            try
            {
                return Activator.CreateInstance(type) as T;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static TaskBase CreateTaskInstance(TaskLoadContext context, long sid, string assemblyName, string className)
        {
            try
            {
                string pluginLocation = GetTaskAssemblyPath(sid, assemblyName);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
                Type type = assembly.GetType(className, true, true);
                if (typeof(TaskBase).IsAssignableFrom(type))
                {
                    TaskBase result = Activator.CreateInstance(type) as TaskBase;
                }
                return Activator.CreateInstance(type) as TaskBase;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetTaskAssemblyPath(long sid, string assemblyName)
        {
            return Path.Combine(Environment.CurrentDirectory, "wwwroot", "tasks", assemblyName, $"{assemblyName}.dll");
        }

        /// <summary>
        /// 加载应用程序域
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static TaskLoadContext LoadAssemblyContext(long sid, string assemblyName)
        {
            try
            {
                string pluginLocation = GetTaskAssemblyPath(sid, assemblyName);
                TaskLoadContext loadContext = new TaskLoadContext(pluginLocation);
                return loadContext;
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"加载应用程序域{assemblyName}失败！");
                throw exp;
            }
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        /// <param name="context"></param>
        public static void UnLoadAssemblyLoadContext(TaskLoadContext context)
        {
            if (context != null)
            {
                context.Unload();
                //for (int i = 0; context.weakReference.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }
}