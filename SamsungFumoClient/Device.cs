using System.Threading.Tasks;
using SamsungFumoClient.Exceptions;
using SamsungFumoClient.Network;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient
{
    public class Device
    {
        private string? _firmwareVersion;

        public Device(string model, string deviceId, string customerCode, string serialNumber, string firmwareVersion,
            string languageCode, int mcc, int mnc, string fotaClientVersion)
        {
            Model = model;
            DeviceId = deviceId;
            CustomerCode = customerCode;
            SerialNumber = serialNumber;
            FirmwareVersion = firmwareVersion;
            LanguageCode = languageCode;
            Mcc = mcc;
            Mnc = mnc;
            FotaClientVersion = fotaClientVersion;
        }

        public Device()
        {
            Model = "UNKNOWN";
            CustomerCode = "XAA";
        }

        public string Model { init; get; }
        public string CustomerCode { init; get; }

        public string? FirmwareVersion
        {
            init => _firmwareVersion = value;
            get => _firmwareVersion;
        }

        public bool IsFirmwareVersionSet => _firmwareVersion is { Length: > 0 };

        public string DeviceId { init; get; } = DeviceUtils.GenerateDeviceId();
        public string SerialNumber { init; get; } = DeviceUtils.GenerateSerialNumber();
        public string LanguageCode { init; get; } = DeviceUtils.DefaultLanguageCode;
        public int Mcc { init; get; } = DeviceUtils.DefaultMcc;
        public int Mnc { init; get; } = DeviceUtils.DefaultMnc;
        public string FotaClientVersion { init; get; } = DeviceUtils.DefaultFotaVersion;

        public (string, string)[] AsDevInfNodes()
        {
            return new[]
            {
                ("./DevInfo/Lang", LanguageCode),
                ("./DevInfo/DmV", "1.2"),
                ("./DevInfo/Mod", Model),
                ("./DevInfo/Man", "Samsung"),
                ("./DevInfo/DevId", DeviceId),
                ("./DevInfo/Ext/TelephonyMcc", Mcc.ToString()),
                ("./DevInfo/Ext/TelephonyMnc", Mnc.ToString()),
                ("./DevInfo/Ext/SIMCardMcc", Mcc.ToString()),
                ("./DevInfo/Ext/SIMCardMnc", Mnc.ToString()),
                ("./DevInfo/Ext/FotaClientVer", FotaClientVersion),
                ("./DevInfo/Ext/DMClientVer", FotaClientVersion),
                ("./DevInfo/Ext/OmcCode", CustomerCode),
                ("./DevInfo/Ext/DevNetworkConnType", "WIFI"),
            };
        }

        public (string, string)[] AsDevDetailNodes(string? firmwareVersion = null)
        {
            firmwareVersion ??= FirmwareVersion;
            
            if (_firmwareVersion is not { Length: > 0 })
            {
                Log.I(
                    "Device.AsDevDetailNodes: firmwareVersion is null or empty. Automatically determining firmware version by checking online...");
                RandomizeFirmwareVersion();
            }

            return new[]
            {
                ("./DevDetail/FwV", firmwareVersion),
            };
        }

        public async Task RandomizeFirmwareVersion()
        {
            try
            {
                var oldFws = await PollingHttpClient.FindOldVersionsAsync(Model, CustomerCode);
                _firmwareVersion = oldFws.RandomElement();
            }
            catch (HttpException ex)
            {
                Log.E($"Device.RandomizeFirmwareVersion: PollingHttpClient.FindOldVersionsAsync failed due to HTTP error {ex.ErrorCode}. " +
                      $"Cannot determine current firmware build version, please supply this parameter on your own or try again.");
                throw;
            }
        }
    }
}