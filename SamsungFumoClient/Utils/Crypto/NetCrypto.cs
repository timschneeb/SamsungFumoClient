using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SamsungFumoClient.Utils.Crypto
{
    public class NetCrypto : ICrypto
    {
        public Task<byte[]> Md5ComputeHashAsync(byte[] data)
        {
            return Task.FromResult(MD5.HashData(data));
        }

        public Task<byte[]> HmacSha1ComputeHashAsync(string secret, byte[] data)
        {
            return Task.FromResult(HMACSHA1.HashData(Encoding.UTF8.GetBytes(secret), data));
        }
    }
}