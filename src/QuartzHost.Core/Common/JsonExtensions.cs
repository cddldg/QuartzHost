using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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

    public static class EnumExtensions
    {
        /// <summary>
        ///     从枚举中获取Description
        /// </summary>
        /// <param name="enumName">需要获取枚举描述的枚举</param>
        /// <param name="joinStr">拼接在后面的字符</param>
        /// <returns>描述内容</returns>
        public static string GetDescription(this Enum enumName, string joinStr = "")
        {
            string description;
            var fieldInfo = enumName.GetType().GetField(enumName.ToString());
            var attributes = fieldInfo.GetDescriptAttr();
            if (attributes != null && attributes.Length > 0)
                description = attributes[0].Description;
            else
                description = enumName.ToString();
            return $"{description}{joinStr}";
        }

        /// <summary>
        ///     获取字段Description
        /// </summary>
        /// <param name="fieldInfo">FieldInfo</param>
        /// <returns>DescriptionAttribute[] </returns>
        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return null;
        }

        /// <summary>
        ///     根据Description获取枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举描述</param>
        /// <returns>枚举</returns>
        public static T GetEnumName<T>(string description)
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var curDesc = field.GetDescriptAttr();
                if (curDesc != null && curDesc.Length > 0)
                {
                    if (curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"{description} 未能找到对应的枚举.", nameof(description));
        }

        /// <summary>
        ///     将枚举转换为ArrayList
        ///     若不是枚举类型，则返回NULL
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>ArrayList</returns>
        public static ArrayList ToArrayList(this Type type)
        {
            if (type.IsEnum)
            {
                var array = new ArrayList();
                var enumValues = Enum.GetValues(type);
                foreach (Enum value in enumValues)
                    array.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
                return array;
            }

            return null;
        }
    }
}