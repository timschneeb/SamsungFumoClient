using System;
using System.Security.Cryptography;
using System.Text;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.Secure
{
    public static class CryptUtils
    {
        private static readonly byte[] Dict =
        {
            1, 15, 5, 11, 19, 28, 23, 47, 35, 44, 2, 14, 6, 10, 18, 13, 22, 26, 32, 47, 3, 13, 7, 9, 17, 30, 21, 25, 33,
            45, 4, 12, 8, 63, 16, 31, 20, 24, 34, 46
        };

        private static readonly char[] HexTable =
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};

        private static readonly MD5 Md5 = new MD5CryptoServiceProvider();

        private static string AdpShuffle(string var0)
        {
            var var1 = var0.Length;
            var var2 = var1 % 2;
            var var3 = var1 / 2;
            if (var2 != 0)
            {
                ++var3;
            }

            while (var3 < var1)
            {
                var var4 = var0[var3];
                var0 = var0.Remove(var3, 1);
                var var5 = var1 - var3;
                if (var2 == 0)
                {
                    var5 += -1;
                }

                var0 = var0.Insert(var5, var4.ToString());
                ++var3;
            }

            return var0;
        }

        private static char[] AdpEncodeHex(byte[] bArr)
        {
            char[] cArr = new char[(bArr.Length * 2)];
            var i = 0;
            foreach (var b in bArr)
            {
                var i2 = i + 1;
                char[] cArr2 = HexTable;
                cArr[i] = cArr2[b & 15];
                i = i2 + 1;
                cArr[i2] = cArr2[(b >> 4) & 15];
            }

            return cArr;
        }

        private static string? CharToString(char[] cArr)
        {
            if (cArr.Length <= 0)
            {
                return null;
            }

            var i = 0;
            while (cArr[i] != 0 && cArr.Length > i)
            {
                i++;
            }

            char[] cArr2 = new char[i];
            for (var i2 = 0; i2 < i; i2++)
            {
                cArr2[i2] = cArr[i2];
            }

            return new string(cArr2);
        }

        public static string? GenerateClientPassword(string str, string str2)
        {
            char[] cArr = new char[64];
            string substring = str.Substring(str.IndexOf((char) 58) + 1);
            var subLength = substring.Length;
            if (subLength == 0)
            {
                return null;
            }

            var i = 0;
            for (var i2 = 0; i2 < subLength; i2++)
            {
                if (char.IsLetterOrDigit(substring[i2]))
                {
                    cArr[i] = substring[i2];
                    i++;
                }
            }

            long j = 0;
            long j2 = 0;
            for (var i3 = 0; i3 < i - 1; i3++)
            {
                long j3 = cArr[i3];
                byte[] bArr = Dict;
                j += j3 * bArr[i3];
                j2 += cArr[i3] * (long) cArr[(i - i3) - 1] * bArr[i3];
            }

            string devPwdKey = $"{j}{j2}";
            if (string.IsNullOrEmpty(devPwdKey))
            {
                return null;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(str2 + devPwdKey + str);
            try
            {
                char[] hex = AdpEncodeHex(Md5.ComputeHash(bytes));
                var length = str.Length;
                bytes[0] = Encoding.UTF8.GetBytes(str)[length - 2];
                bytes[1] = Encoding.UTF8.GetBytes(str)[length - 1];
                string concat = new string(new[] {hex[1], hex[4], hex[5], hex[7]})
                                + (CharToString(new DesCrypt().Generate(str, bytes).ToCharArray()));
                string stringBuffer = string.Empty;
                stringBuffer += concat;
                for (var k = 0; k < 3; k++)
                {
                    stringBuffer = AdpShuffle(stringBuffer);
                }

                return stringBuffer;
            }
            catch (Exception e)
            {
                Log.E(e.ToString());
                return "";
            }
        }

        private static string ComputeMd5Credentials(string str, string str2, byte[] bArr)
        {
            string concat = $"{str}:{str2}";

            var hash = Md5.ComputeHash(Encoding.UTF8.GetBytes(concat));
            var concat2 = $"{Base64.Encode(hash)}:";
            var concat2Bytes = Encoding.UTF8.GetBytes(concat2);

            var length = concat2Bytes.Length;
            var bArr2 = new byte[(bArr.Length + length)];
            Array.Copy(concat2Bytes, 0, bArr2, 0, length);
            Array.Copy(bArr, 0, bArr2, length, bArr.Length);
            return Base64.Encode(Md5.ComputeHash(bArr2));
        }

        public static string? MakeDigest(AuthTypes authType, string clientId, string clientPassword, byte[]? nextNonce,
            byte[]? bArr2)
        {
            switch (authType)
            {
                case AuthTypes.Basic:
                {
                    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientPassword))
                    {
                        Log.E("CryptUtils.MakeDigest: Missing parameters for CRED_TYPE_BASIC");
                        return null;
                    }

                    string concat = $"{clientId}:{clientPassword}";
                    string xdmBase64Encode = Base64.Encode(concat);
                    Log.D("CryptUtils.MakeDigest: Generated credentials using CRED_TYPE_BASIC: " + xdmBase64Encode);
                    return xdmBase64Encode;
                }
                case AuthTypes.Md5:
                {
                    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientPassword) || nextNonce == null)
                    {
                        Log.E("CryptUtils.MakeDigest: Missing parameters for CRED_TYPE_MD5");
                        return null;
                    }

                    string cred = ComputeMd5Credentials(clientId, clientPassword, nextNonce);
                    Log.D(
                        $"CryptUtils.MakeDigest: Generated credentials using CRED_TYPE_MD5: {cred} (Nonce='{nextNonce}')");
                    return cred;
                }
                case AuthTypes.Hmac:
                case AuthTypes.HmacAsString:
                {
                    MD5 md5 = new MD5CryptoServiceProvider();

                    string concat2 = $"{clientId}:{clientPassword}";
                    string concat3 = Base64.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(concat2))) +
                                     ":" + Encoding.UTF8.GetString(nextNonce ?? Array.Empty<byte>()) + ":" +
                                     Base64.Encode(md5.ComputeHash(bArr2 ?? Array.Empty<byte>()));

                    return authType == AuthTypes.Hmac
                        ? Base64.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(concat3)))
                        : Encoding.UTF8.GetString(md5.ComputeHash(Encoding.UTF8.GetBytes(concat3)));
                }
                default:
                {
                    Log.E("CryptUtils.MakeDigest: Unknown auth type");
                    return null;
                }
            }
        }

        public static string GenerateRandomToken(int i)
        {
            var byteArray = new byte[(int) Math.Ceiling(i / 2.0)];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(byteArray);
            }

            return string.Concat(Array.ConvertAll(byteArray, x => x.ToString("X2")));
        }
    }
}