using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SamsungFumoClient.Utils.Crypto
{
    public class NetCrypto : ICrypto
    {
        private static readonly MD5 Md5 = new MD5CryptoServiceProvider();
        
        public async Task<byte[]> Md5ComputeHashAsync(byte[] data)
        {
            return Md5.ComputeHash(data);
        }

        public async Task<byte[]> HmacSha1ComputeHashAsync(string secret, byte[] data)
        {
            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
            var stream = new MemoryStream(data);
            var hash = await hmac.ComputeHashAsync(stream);
            stream.Close();

            return hash;
        }
    }
}