namespace SamsungFumoClient.Utils.Crypto
{
    public static class CryptoProvider
    {
        private static ICrypto? _instance;
        public static ICrypto Instance => _instance ??= new NetCrypto();

        public static void Use(ICrypto obj)
        {
            _instance = obj;
        }
    }
}