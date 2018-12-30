using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.Extensions
{
    public static class StringExtension
    {
        public static string SubstringbyLength(this string str, int length)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (str.Length > length) return str.Substring(0, length);
            return str;
        }
    }
}
