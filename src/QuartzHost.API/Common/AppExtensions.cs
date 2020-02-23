using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuartzHost.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuartzHost.API.Common
{
    public static class AppExtensions
    {
        /// <summary>
        /// 自定义控制器激活，并手动注册所有控制器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="obj"></param>
        public static void AddHosControllers(this IServiceCollection services, object obj)
        {
            //services.Replace(ServiceDescriptor.Transient<IControllerActivator, HosControllerActivator>());
            //var assembly = obj.GetType().GetTypeInfo().Assembly;
            //var manager = new ApplicationPartManager();
            //manager.ApplicationParts.Add(new AssemblyPart(assembly));
            //manager.FeatureProviders.Add(new ControllerFeatureProvider());
            //var feature = new ControllerFeature();
            //manager.PopulateFeature(feature);
            //feature.Controllers.Select(ti => ti.AsType()).ToList().ForEach(t =>
            //{
            //    services.AddTransient(t);
            //});
        }

        /// <summary>
        /// 注册应用中的业务service
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppServices(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains("QuartzHost.Core"));
            if (assembly == null) return;
            foreach (var type in assembly.GetTypes())
            {
                var serviceAttribute = type.GetCustomAttribute<ServiceMapToAttribute>();

                if (serviceAttribute != null)
                {
                    switch (serviceAttribute.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(serviceAttribute.ServiceType, type);
                            break;

                        case ServiceLifetime.Scoped:
                            services.AddScoped(serviceAttribute.ServiceType, type);
                            break;

                        case ServiceLifetime.Transient:
                            services.AddTransient(serviceAttribute.ServiceType, type);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否异步请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            bool isAjax = false;
            var xreq = request.Headers.ContainsKey("x-requested-with");
            if (xreq)
            {
                isAjax = request.Headers["x-requested-with"] == "XMLHttpRequest";
            }
            return isAjax;
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(this.DateTimeFormat));
        }
    }
}