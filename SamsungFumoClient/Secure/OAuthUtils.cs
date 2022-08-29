using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SamsungFumoClient.Utils.Crypto;

namespace SamsungFumoClient.Secure
{
    public static class OAuthUtils
    {
        public static async Task<string> GenerateOAuthHeader(string appId, string appSecret, string requestMethod,
            string requestUri, string requestBody, long? timestamp = null)
        {
            if (timestamp == null)
            {
                timestamp = MakeTimestamp();
            }

            var oauth = new Dictionary<string, string>();
            oauth.Add("oauth_consumer_key", appId);
            oauth.Add("oauth_nonce", CryptUtils.GenerateRandomToken(10));
            oauth.Add("oauth_signature_method", "HmacSHA1");
            oauth.Add("oauth_timestamp", timestamp.ToString() ?? string.Empty);
            oauth.Add("oauth_version", "1.0");
            oauth.Add("oauth_signature", await GenerateSignature(appSecret, requestMethod, requestUri, requestBody, oauth));

            var sb = new StringBuilder();
            foreach (var (key, value) in oauth)
            {
                sb.Append(key + "=" + value + ",");
            }

            return sb.ToString().TrimEnd(',');
        }

        private static long MakeTimestamp()
        {
            return (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private static async Task<string> GenerateSignature(string appSecret, string requestMethod, string requestUri,
            string requestBody, Dictionary<string, string> oauth)
        {
            var stringBuffer = new StringBuilder();
            stringBuffer.Append(requestMethod.ToUpperInvariant());
            stringBuffer.Append('&');
            stringBuffer.Append(NormalizeUrlWithOAuthSpec(requestUri));
            stringBuffer.Append('&');
            stringBuffer.Append(NormalizeParameters(oauth));
            if (requestBody is {Length: > 0})
            {
                stringBuffer.Append('&');
                stringBuffer.Append(requestBody);
            }

            return Convert.ToBase64String(await ComputeSignature(appSecret, stringBuffer.ToString()));
        }

        private static async Task<byte[]> ComputeSignature(string appSecret, string str2)
        {
            return await CryptoProvider.Instance.HmacSha1ComputeHashAsync(appSecret, Encoding.ASCII.GetBytes(str2));
        }

        private static string NormalizeUrlWithOAuthSpec(string str)
        {
            int lastIndexOf;
            var uri = new Uri(str);
            var lowerCase = uri.Scheme.ToLowerInvariant();
            var lowerCase2 = uri.Authority.ToLowerInvariant();
            if (((lowerCase.Equals("http") && uri.Port == 80) || (lowerCase.Equals("https") && uri.Port == 443)) &&
                (lastIndexOf = lowerCase2.LastIndexOf((char) 58)) >= 0)
            {
                lowerCase2 = lowerCase2.Substring(0, lastIndexOf);
            }

            var rawPath = uri.PathAndQuery;
            if (rawPath is not {Length: > 0})
            {
                rawPath = "/";
            }

            var val = UrlEncodeWithOAuthSpec(lowerCase + "://" + lowerCase2 + rawPath);
            return val;
        }

        private static string NormalizeParameters(Dictionary<string, string> map)
        {
            var stringBuffer = new StringBuilder();
            foreach (var (key, value) in map)
            {
                stringBuffer.Append(key + "=" + value.Replace("\"", "").Replace("&quot;", ""));
                stringBuffer.Append('&');
            }

            var val = UrlEncodeWithOAuthSpec(stringBuffer.ToString().TrimEnd('&'));
            return val;
        }

        private static string UrlEncodeWithOAuthSpec(string? str)
        {
            return str == null
                ? ""
                : Uri.EscapeDataString(str)
                    .Replace("+", "%20")
                    .Replace("*", "%2A")
                    .Replace("%7E", "~")
                    .Replace("%25", "%");
        }
    }
}