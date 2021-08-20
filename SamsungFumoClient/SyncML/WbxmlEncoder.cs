using System.IO;
using System.Text;
using SamsungFumoClient.SyncML.Enum;

namespace SamsungFumoClient.SyncML
{
    public class WbxmlEncoder
    {
        private readonly MemoryStream _memoryStream = new();

        public bool StartDocument(WbxmlCharset charset, string stringTable, int i3)
        {
            if (!AppendByte(2) || !AppendUInt32(0))
            {
                return false;
            }

            return AppendUInt32(0) && AppendUInt32((int) charset) && AppendUInt32(i3) &&
                   AppendToBuffer(stringTable);
        }

        public bool StartElement(byte tag, bool z)
        {
            var i = tag;
            if (z)
            {
                i |= 64;
            }

            return AppendByte(i);
        }

        public bool EndElement()
        {
            return AppendByte(1);
        }

        public bool AddSwitchpage(byte i)
        {
            if (AppendByte(0) && AppendByte(i))
            {
                return true;
            }

            return false;
        }

        public bool AddContent(string str)
        {
            if (!AppendByte(3) || !AppendToBuffer(str))
            {
                return false;
            }

            _memoryStream.WriteByte(0);
            return true;
        }

        public bool AddOpaque(char[] cArr, int i)
        {
            if (!(AppendByte((byte) WbxmlElement.OPAQUE) && AppendUInt32(i)))
            {
                return false;
            }

            _memoryStream.Write(Encoding.UTF8.GetBytes(cArr));

            return true;
        }

        public MemoryStream GetBuffer()
        {
            return _memoryStream;
        }

        public bool AppendToBuffer(string str)
        {
            _memoryStream.Write(Encoding.UTF8.GetBytes(str));
            return true;
        }

        public bool AppendByte(byte i)
        {
            _memoryStream.WriteByte(i);
            return true;
        }

        public bool AppendUInt32(int i)
        {
            int i2;
            byte[] bArr = new byte[5];
            var i3 = 0;
            while (true)
            {
                i2 = i3 + 1;
                bArr[i3] = (byte) (i & 127);
                i >>= 7;
                if (i == 0)
                {
                    break;
                }

                i3 = i2;
            }

            while (i2 > 1)
            {
                i2--;
                _memoryStream.WriteByte((byte) (bArr[i2] | -128));
            }

            _memoryStream.WriteByte(bArr[0]);
            return true;
        }
    }
}