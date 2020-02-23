using Microsoft.Extensions.Logging;
using QuartzHost.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniqueIdGenerator.Net;

namespace VIP
{
    public class Test : TaskBase
    {
        private static readonly ILogger _logger = DG.Logger.DGLogManager.GetLogger();
        public static readonly short GenerID = (short)new Random().Next(0, 512);
        private static readonly Generator Generator = new Generator(GenerID, DateTime.Today);

        public override async Task Run(TaskContext context)
        {
            _logger.LogInformation($"--------vip test {(long)Generator.NextLong()} {context.ToJson()} {DateTime.Now}");
            await Task.FromResult(0);
        }
    }

    public static class JsonExtensions
    {
        public static string ToJson<T>(this T obj)
        {
            if (obj == null)
                return null;
            try
            {
                return JsonSerializer.Serialize(obj, obj.GetType());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T ToObj<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}