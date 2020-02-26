using System;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var str = "node1112";
            var dd = str.GetHashCode();
            short.TryParse(str, out short ps);

            Console.WriteLine($"{dd} {ps} {GetSha256(str)}");
            Console.ReadLine();
        }

        public static string GetSha256(string str)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}