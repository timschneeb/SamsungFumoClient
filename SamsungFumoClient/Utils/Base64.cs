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

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static byte[] Decode(string str)
        {
            return Convert.FromBase64String(ClearUnusedBits(str.Trim()));
        }

        public static byte[] Decode(byte[] bArr)
        {
            return Decode(Encoding.UTF8.GetString(bArr));
        }

        /// <summary>
        /// Samsung's DM server sends nonces such as "LLLLLL==" whose unused bits before the padding are not zero.
        /// .NET 8 silently ignored those bits, but .NET 11 rejects such input with a FormatException.
        /// Clearing them yields exactly the bytes the lenient decoder produced.
        /// </summary>
        private static string ClearUnusedBits(string str)
        {
            var padding = str.EndsWith("==", StringComparison.Ordinal) ? 2 : str.EndsWith('=') ? 1 : 0;
            var index = str.Length - padding - 1;
            if (padding == 0 || index < 0)
            {
                return str;
            }

            var value = Alphabet.IndexOf(str[index]);
            if (value < 0)
            {
                // Let Convert.FromBase64String report the malformed input
                return str;
            }

            var canonical = Alphabet[value & (padding == 2 ? 0b110000 : 0b111100)];
            if (canonical == str[index])
            {
                return str;
            }

            var chars = str.ToCharArray();
            chars[index] = canonical;
            return new string(chars);
        }
    }
}