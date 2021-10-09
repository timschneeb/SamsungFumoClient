using System.Threading.Tasks;

namespace SamsungFumoClient.Utils.Crypto
{
    public interface ICrypto
    {
        Task<byte[]> Md5ComputeHashAsync(byte[] data);
        Task<byte[]> HmacSha1ComputeHashAsync(string secret, byte[] data);
    }
}