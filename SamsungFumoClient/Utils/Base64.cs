using System;
using System.Text;

namespace SamsungFumoClient.Utils
{
    public static class Base64
    {
        public static string Encode(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string Encode(byte[] bArr)
        {
            return Convert.ToBase64String(bArr);
        }

        public static byte[] Decode(string str)
        {
            return Convert.FromBase64String(str);
        }

        public static byte[] Decode(byte[] bArr)
        {
            return Convert.FromBase64String(Encoding.UTF8.GetString(bArr));
        }
    }
}