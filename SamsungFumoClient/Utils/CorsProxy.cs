using System;
using System.Web;

namespace SamsungFumoClient.Utils
{
    public static class CorsProxy
    {
        private const string CorsApi = "https://cors.timschneeberger.workers.dev/?apiurl=";
        
        public static string Handle(string url)
        {
            if (OperatingSystem.IsBrowser())
            {
                return CorsApi + HttpUtility.UrlEncode(url);
            }

            return url;
        }
    }
}