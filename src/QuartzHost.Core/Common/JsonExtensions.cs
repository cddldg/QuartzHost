using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace QuartzHost.Core.Common
{
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