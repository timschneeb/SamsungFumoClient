using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.Network
{
    public static class PollingHttpClient
    {
        private static readonly HttpClient _client = new();

        public static async Task<string[]> FindOldVersionsAsync(string model, string customerCode)
        {
            Log.V($">>> PollingHttpClient.FindOldVersionAsync(\"{model}\",\"{customerCode}\") ==========");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
                $"https://fota-cloud-dn.ospserver.net/firmware/{customerCode.ToUpperInvariant()}/{model.ToUpperInvariant()}/version.xml");
            httpRequestMessage.Headers.Add("Accept", "text/xml");
            var result = await _client.SendAsync(httpRequestMessage);

            var response = await result.Content.ReadAsStringAsync();

            Log.V(
                "<<< Server responded to PollingHttpClient.FindOldVersionAsync(\"{model}\",\"{customerCode}\") ==========");
            if (!result.IsSuccessStatusCode)
            {
                Log.E(result.ToString());
                throw new NetworkInformationException((int) result.StatusCode);
            }

            var doc = new XmlDocument();
            doc.LoadXml(response);

            /* Handle potential server error */
            var errorNode = doc.SelectSingleNode("/Error");
            if (errorNode != null)
            {
                foreach (XmlNode child in errorNode.ChildNodes)
                {
                    if (child.Name == "Message")
                    {
                        throw new XmlException("Unable to resolve valid firmware build string. " +
                                               "Please check the device model and customer code. " +
                                               "Server responded with: " + child.InnerText);
                    }
                }
            }

            var oldFirmwareList = new List<string>();
            var version = doc.SelectNodes("/versioninfo/firmware/version/upgrade/value");
            if (version != null)
            {
                foreach (XmlNode versionChild in version)
                {
                    oldFirmwareList.Add(versionChild.InnerText);
                }
            }

            Log.V(response);
            return oldFirmwareList.ToArray();
        }
    }
}