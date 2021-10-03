using SamsungFumoClient.Secure;

namespace SamsungFumoClient.Utils
{
    public static class DeviceUtils
    {
        public const int DefaultMcc = 262; /* Germany */
        public const int DefaultMnc = 03; /* o2 */

        public const string DefaultLanguageCode = "en-US";
        public const string DefaultFotaVersion = "3.0.21072951";

        private static readonly string[] KnownGeneralBudsCustomerCodes =
        {
            "ZTO",
            "XAC",
            "CHC",
            "MEA",
            "XAR",
            "KSA",
            "BRI",
            "ARO",
            "EUB",
            "TUR",
            "XFA",
            "EUE",
            "KOO",
            "ASA",
            "EUA",
            "MXO",
            "SEK",
            "XXV",
            "LTA",
            "ILO",
            "EUG",
            "XSP",
            "XSE",
            "INU",
            "XME",
            "XJP",
        };

        private static readonly string[] KnownBudsLiveCustomerCodes =
        {
            "CAC",
            "KTC",
            "SER"
        };

        private static readonly string[] KnownBudsProCustomerCodes =
        {
            "EUD",
            "CIS"
        };

        private static readonly string[] KnownBudsVendorPrefixes =
        {
            "64:03:7F",
            "6C:DD:BC",
            "70:CE:8C",
            "F8:8F:07",
            "08:BF:A0",
            "80:7B:3E",
            "74:9E:F5",
            "58:A6:39",
            "B4:CE:40",
            "24:5A:B5",
            "18:4E:16",
            "1C:E6:1D",
            "80:9F:F5",
            "34:82:C5",
            "B4:1A:1D",
            "78:46:D4",
            "18:54:CF",
            "A0:AC:69",
            "C4:5D:83",
            "E8:7F:6B",
            "5C:CB:99",
        };

        private static readonly string[] KnownBudsLastChar =
        {
            "V",
            "H",
            "J",
            "B",
            "L",
            "F",
            "M",
            "T",
            "A",
            "Z",
            "K",
            "Y",
            "W",
            "X",
            "N",
            "R",
            "E",
            "P",
            "D",
        };

        private static readonly string[] KnownBudsCharPool =
        {
            "V",
            "H",
            "J",
            "B",
            "4",
            "F",
            "L",
            "0",
            "M",
            "T",
            "1",
            "A",
            "7",
            "Z",
            "Q",
            "3",
            "K",
            "C",
            "Y",
            "W",
            "X",
            "6",
            "G",
            "N",
            "R",
            "8",
            "5",
            "E",
            "P",
            "D",
            "S",
            "9",
            "2",
        };

        public static string GenerateDeviceId()
        {
            return $"TWID:{GenerateMacAddress().Replace(":", "")}";
        }

        public static string GenerateMacAddress()
        {
            return KnownBudsVendorPrefixes.RandomElement()
                   + ":" + CryptUtils.GenerateRandomToken(2)
                   + ":" + CryptUtils.GenerateRandomToken(2)
                   + ":" + CryptUtils.GenerateRandomToken(2);
        }

        public static string GenerateSerialNumber()
        {
            var monthList = new[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C'};
            var yearList = new[] {'N', 'R'};
            var manufacturerList = new[] {"FA", "XA"};

            var actualSerial = new[] {'0', '1', '2', '3', '4'}.RandomElement().ToString();
            for (var i = 0; i < 4; i++)
            {
                actualSerial += KnownBudsCharPool.RandomElement();
            }

            actualSerial += KnownBudsLastChar.RandomElement();

            return "R" +
                   $"{manufacturerList.RandomElement()}" +
                   $"{yearList.RandomElement().ToString()}{monthList.RandomElement().ToString()}" +
                   $"{actualSerial}";
        }

        public static string GenerateBudsCustomerCode(string? model = null)
        {
            var codes = KnownGeneralBudsCustomerCodes;
            codes = model switch
            {
                "SM-R180" => ArrayUtils.ConcatArray(codes, KnownBudsLiveCustomerCodes),
                "SM-R190" => ArrayUtils.ConcatArray(codes, KnownBudsProCustomerCodes),
                _ => codes
            };

            return codes.RandomElement();
        }
    }
}