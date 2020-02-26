using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Ts();
            Console.ReadLine();
        }

        private static void Ts()
        {
            var list = new Dictionary<string, object>();

            list.Add("NodeName", "node1");
            list.Add("CreateUserId", 1);
            PageInput ps = new PageInput
            {
                PageIndex = 1,
                PageSize = 1,
                Extens = list,
                OrderBy = "TotalRunCount DESC",
            };
            var ss = ps.ToJson();
            Console.WriteLine(ss);
        }
    }

    public class PageInput
    {
        /// <summary>
        ///     每页大小默认20
        /// </summary>

        public int PageSize { get; set; } = 20;

        /// <summary>
        ///     页码
        /// </summary>

        public int PageIndex { get; set; } = 1;

        public Dictionary<string, object> Extens { get; set; } = new Dictionary<string, object>();
        public string OrderBy { get; set; }
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