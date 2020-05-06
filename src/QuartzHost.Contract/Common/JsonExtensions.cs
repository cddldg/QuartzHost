using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuartzHost.Contract.Common
{
    public static class QuartzHostExtensions
    {
        #region Json

        public static JsonSerializerOptions JsonOptions()
        {
            var op = new JsonSerializerOptions { PropertyNamingPolicy = null };
            op.Converters.Add(new DateTimeConverter());
            return op;
        }

        public static string ToJson<T>(this T obj)
        {
            if (obj == null)
                return null;
            try
            {
                return JsonSerializer.Serialize(obj, obj.GetType(), JsonOptions());
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

            return JsonSerializer.Deserialize<T>(json, JsonOptions());
        }

        #endregion Json

        #region Function TimeStamp

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="bflag">true=10位时间戳 false=13 毫秒</param>
        /// <returns></returns>
        public static long GetTimeStamp(bool bflag = false)
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);//ToUniversalTime()转换为标准时区的时间,去掉的话直接就用北京时间
            var ret = 0L;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="bflag">true=10位时间戳 false=13 毫秒</param>
        /// <returns></returns>
        public static DateTime TimeStampToTime(long timeStamp, bool bflag = false)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime;
            if (bflag)//判断是10位
            {
                lTime = Int64.Parse($"{timeStamp}0000000");
            }
            else
            {
                lTime = Int64.Parse($"{timeStamp}0000");//13位
            }
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime daTime = dtStart.Add(toNow);
            return daTime;
        }

        #endregion Function TimeStamp
    }

    public static class Secret
    {
        public static string GetMd5(string str, string salt = null)
        {
            salt ??= "QuartzHost";
            using (var md5 = MD5.Create())
            {
                var input = str;
                if (!string.IsNullOrEmpty(salt))
                    input = $"{input}:{salt}";
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString().ToUpperInvariant();
            }
        }

        private const string SKey = "NH4Av@X2";

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="pToEncrypt">待加密的字符串</param>
        /// <param name="sKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DesEncrypt(string pToEncrypt, string sKey = null)
        {
            sKey ??= SKey;
            StringBuilder ret = new StringBuilder();
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch
            {
                return pToEncrypt;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="pToDecrypt">待解密的字符串</param>
        /// <param name="sKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DesDecrypt(string pToDecrypt, string sKey = null)
        {
            sKey ??= SKey;
            MemoryStream ms = new MemoryStream();
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return pToDecrypt;
            }
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