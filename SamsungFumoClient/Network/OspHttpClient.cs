using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SamsungFumoClient.Exceptions;
using SamsungFumoClient.Secure;
using SamsungFumoClient.Utils;
using ThePBone.WbXml2;

namespace SamsungFumoClient.Network
{
    public class OspHttpClient
    {
        private readonly HttpClient _client = new();
        public Device? Device { set; get; }

        public async Task<bool> SendFumoRegisterAsync(Device device)
        {
            Device = device;

            string request =
                "<?xml version='1.0' encoding='UTF-8' standalone='yes' ?>" +
                "<FumoDeviceVO>" +
                "<deviceTypeCode>PHONE DEVICE</deviceTypeCode>" +
                $"<deviceModelID>{device.Model}</deviceModelID>" +
                $"<deviceName>{device.Model}</deviceName>" +
                $"<deviceUniqueID>{device.DeviceId}</deviceUniqueID>" +
                $"<devicePhysicalAddressText>{device.DeviceId}</devicePhysicalAddressText>" +
                $"<customerCode>{device.CustomerCode}</customerCode>" +
                "<uniqueNumber></uniqueNumber>" +
                $"<deviceSerialNumber>{device.SerialNumber}</deviceSerialNumber>" +
                $"<firmwareVersion>{device.FirmwareVersion}</firmwareVersion>" +
                $"<mobileCountryCode>{device.Mcc.ToString()}</mobileCountryCode>" +
                $"<mobileNetworkCode>{device.Mnc.ToString()}</mobileNetworkCode>" +
                $"<mobileCountryCodeByTelephony>{device.Mcc.ToString()}</mobileCountryCodeByTelephony>" +
                $"<mobileNetworkCodeByTelephony>{device.Mnc.ToString()}</mobileNetworkCodeByTelephony>" +
                "<phoneNumberText></phoneNumberText>" +
                "<initialBootingDate></initialBootingDate>" +
                "<terms>Y</terms>" +
                "<termsVersion></termsVersion>" +
                $"<fotaClientVer>{device.FotaClientVersion}</fotaClientVer>" +
                "</FumoDeviceVO>";

            Log.V(">>> OspHttpClient.SendFumoRegisterAsync(device) ==========");
            Log.V(request);

            var httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Post, "https://www.ospserver.net/device/fumo/device/");
            var oauth = OAuthUtils.GenerateOAuthHeader(
                "dz7680f4t7",
                "4BE4F2C346C6F8831A480E14FD4DE276",
                httpRequestMessage.Method.Method.ToUpperInvariant(),
                httpRequestMessage!.RequestUri!.OriginalString,
                request
            );

            Log.D("OspHttpClient.SendFumoRegisterAsync: Using OAuth header: " + oauth);

            httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "SAMSUNG-Android");
            httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", oauth);
            httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "identity");
            httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "*, */*");
            httpRequestMessage.Headers.TryAddWithoutValidation("Connection", "keep-alive");
            httpRequestMessage.Headers.Add("x-osp-version", "1");
            httpRequestMessage.Content = new StringContent(request, Encoding.UTF8, "text/xml");

            var result = await _client.SendAsync(httpRequestMessage);
            var response = await result.Content.ReadAsStringAsync();

            Log.V("<<< Server responded to OspHttpClient.SendFumoRegisterAsync(device) ==========");
            if (!result.IsSuccessStatusCode)
            {
                Log.E(result.ToString());

                if (result.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized)
                {
                    Log.E("OspHttpClient.SendFumoRegisterAsync: FUMO REGISTRATION ERROR: \n" +
                          "Please verify your supplied device properties:\n" +
                          "\t- Is the serial number valid?\n" +
                          "\t- Is the customer code available for your model?\n" +
                          "\t- Does the device id contain one of Samsung's OUI Vendor Prefixes?\n" +
                          "\t- Does the firmware version exist?");
                    return false;
                }

                throw new HttpException((int)result.StatusCode);
            }

            Log.V(response);
            return result.IsSuccessStatusCode;
        }

        public async Task<byte[]> SendWbxmlAsync(string uri, byte[] wbxml)
        {
            if (Device == null)
            {
                Log.W("OspHttpClient.SendWbxmlAsync: Device is unset. Generating invalid user-agent!");
            }

            Log.V(">>> OspHttpClient.SendWbxmlAsync(uri, wbxml) ==========");
#if HasWbXml2
            WbXmlParser? parser = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                parser = new WbXmlParser
                {
                    WbXmlLanguage = WBXMLLanguage.WBXML_LANG_SYNCML_SYNCML12
                };

                Log.V(parser.WriteXmlString(wbxml));
            }
#endif
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent",
                $"Samsung {Device?.Model} SyncML_DM Client");
            httpRequestMessage.Headers.Add("Accept", "application/vnd.syncml.dm+wbxml");
            httpRequestMessage.Content = new ByteArrayContent(wbxml);
            httpRequestMessage.Content.Headers.Add("Content-Type", "application/vnd.syncml.dm+wbxml");

            var result = await _client.SendAsync(httpRequestMessage);

            var response = await result.Content.ReadAsByteArrayAsync();

            Log.V("<<< Server responded to OspHttpClient.SendWbxmlAsync(uri, wbxml) ==========");
            if (!result.IsSuccessStatusCode)
            {
                Log.E(result.ToString());

                throw new HttpException((int)result.StatusCode);
            }
        
#if HasWbXml2
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Log.V(parser!.WriteXmlString(response));
            }
#endif
            return response;
        }
        
        public async Task<string?> GetDownloadDescriptorAsync(string uri)
        {
            Log.V(">>> OspHttpClient.GetDownloadDescriptorAsync(uri) ==========");
            Log.V(uri);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            httpRequestMessage.Headers.Add("Accept", "application/vnd.oma.dd+xml");
            httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent",
                $"Samsung {Device?.Model} SyncML_DM Client");
            
            var result = await _client.SendAsync(httpRequestMessage);
            var response = await result.Content.ReadAsStringAsync();

            Log.V("<<< Server responded to OspHttpClient.GetDownloadDescriptorAsync(uri) ==========");
            if (!result.IsSuccessStatusCode)
            {
                Log.E(result.ToString());

                Log.E("OspHttpClient.GetDownloadDescriptorAsync: Invalid or expired download descriptor URI");
                return null;
            }

            Log.V(response);
            return response;
        }

    }
}