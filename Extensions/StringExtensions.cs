using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ServerlessDataPipeline.Extensions
{
    public static class StringExtensions
    {
        public static string GetMd5Id(this string[] parts)
        {
            var md5 = MD5.Create();

            var contentInBytes = Encoding.UTF8.GetBytes(string.Concat(parts));

            var sb = new StringBuilder();
            var hash = md5.ComputeHash(contentInBytes);
            foreach (var byteElement in hash)
            {
                sb.Append(byteElement.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
