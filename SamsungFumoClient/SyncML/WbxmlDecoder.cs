using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML
{
    public class WbxmlDecoder
    {
        public string CurrentInnerText;
        public WbxmlCodepage CurrentCodePage = WbxmlCodepage.SYNCML;
        public int Index;
        public Meta? Meta;
        
        private string _stringT;
        private string? _stringTable;
        private int _publicUid;
        private readonly byte[] _buffer;
        
        protected WbxmlDecoder(byte[] bArr)
        {
            _buffer = bArr;
            Index = 0;
            CurrentInnerText = string.Empty;
            _stringT = string.Empty;
        }

        public WbxmlElement ParseReadElement()
        {
            try
            {
                return (WbxmlElement) (ReadByte() & 63 & 127);
            }
            catch (IOException e)
            {
                Log.E("WbxmlDecoder.ParseReadElement: Failed to read byte: " + e.ToString());
                return WbxmlElement.UNSET;
            }
        }

        public Item[] ParseItemlist(Item[]? xDmList)
        {
            var list = new List<Item>(xDmList ?? Array.Empty<Item>());
            while (true)
            {
                var i = CurrentElement();

                if (i != WbxmlElement.ITEM)
                {
                    return list.ToArray();
                }

                var xDmParserItem = new Item();
                xDmParserItem.Parse((SyncMlParser) this);

                list.Add(xDmParserItem);
            }
        }

        public string? ParseContent()
        {
            var nextByte = (WbxmlElement) ReadByte();
            if (nextByte == WbxmlElement.STR_I)
            {
                return ParseStrI();
            }

            if (nextByte == WbxmlElement.STR_T)
            {
                return ParseStrT();
            }

            if (nextByte == WbxmlElement.OPAQUE)
            {
                return ParseExtension(nextByte);
            }

            Index--;
            SkipElement();
            return null;
        }

        public List<string>? ParseElelist(WbxmlElement listElementType, List<string> xDmList)
        {
            while (true)
            {
                var currentElement = CurrentElement();

                if (currentElement != listElementType)
                {
                    return xDmList;
                }

                if (ParseElement(listElementType) != 0)
                {
                    return null;
                }

                xDmList.Add(CurrentInnerText);
            }
        }

        public WbxmlError ParseElement(WbxmlElement i)
        {
            const string str = "";
            CurrentInnerText = "";
            var checkResult = ParseCheckElement(i);
            if (checkResult != WbxmlError.ERR_OK)
            {
                return checkResult;
            }

            var zeroBitResult = ZeroBitTagCheck();
            if (zeroBitResult == WbxmlError.ZEROBIT_TAG)
            {
                return WbxmlError.ERR_OK;
            }

            if (zeroBitResult != WbxmlError.ERR_OK)
            {
                return zeroBitResult;
            }

            var skipLiteralResult = SkipLiteralElement();
            if (skipLiteralResult != WbxmlError.ERR_OK)
            {
                return skipLiteralResult;
            }

            while (true)
            {
                CurrentInnerText = str + ParseContent();

                if (ReadByte() != 131)
                {
                    Index--;
                    break;
                }

                CurrentInnerText = str + ParseStrT();
                if (ReadByte() == 1)
                {
                    Index--;
                    break;
                }

                Index--;
            }

            return ParseCheckElement(WbxmlElement.END);
        }

        public WbxmlError ParseBlankElement(WbxmlElement element)
        {
            var z = ((byte) CurrentElement() & 64) != 0;
            var checkResult = ParseCheckElement(element);
            if (checkResult != 0)
            {
                return checkResult;
            }

            WbxmlError checkEndResult;
            if (!z || (checkEndResult = ParseCheckElement(WbxmlElement.END)) == WbxmlError.ERR_OK)
            {
                return WbxmlError.FAIL;
            }

            return checkEndResult;
        }

        public WbxmlElement CurrentElement()
        {
            var i = _buffer[Index] & 255;
            if (i >= 0)
            {
                return (WbxmlElement) (i & 63 & 127);
            }

            throw new IOException("Unexpected EOF");
        }

        public WbxmlError ParseCheckElement(WbxmlElement element)
        {
            var next = ParseReadElement();
            if (element == next)
            {
                return WbxmlError.ERR_OK;
            }

            Log.E($"WbxmlDecoder.ParseCheckElement: Unknown element; {element} does not match with {next}!");
            return WbxmlError.UNKNOWN_ELEMENT;
        }

        public WbxmlError ZeroBitTagCheck()
        {
            WbxmlError result;
            byte[] bArr = _buffer;
            var i2 = Index - 1;
            Index = i2;
            var i3 = bArr[i2] & 255;
            if (i3 < 0)
            {
                return WbxmlError.FAIL;
            }

            var i4 = i3 & 63 & 127;
            if (i4 < 5 || i4 > 60 || (i3 & 64) != 0)
            {
                result = WbxmlError.ERR_OK;
            }
            else
            {
                Log.E("WbxmlDecoder.ZeroBitTagCheck: Encountered a zero bit tag");
                result = WbxmlError.ZEROBIT_TAG;
            }

            Index++;
            return result;
        }

        public void SkipElement()
        {
            var i = 0;
            while (true)
            {
                var currentElement = CurrentElement();
                if (currentElement == WbxmlElement.CODEPAGE)
                {
                    ReadByte();
                    ReadByte();
                }
                else if (currentElement == WbxmlElement.END)
                {
                    ReadByte();
                    i--;
                    if (i == 0)
                    {
                        break;
                    }
                }
                else
                {
                    if (currentElement is not (WbxmlElement.STR_I or WbxmlElement.STR_T))
                    {
                        if (currentElement != WbxmlElement.OPAQUE)
                        {
                            ReadByte();
                            i++;
                        }
                    }

                    ParseContent();
                }
            }

            while (CurrentElement() == WbxmlElement.CODEPAGE)
            {
                ReadByte();
                ReadByte();
            }
        }

        public WbxmlError SkipLiteralElement()
        {
            if (CurrentElement() != WbxmlElement.LITERAL)
            {
                return WbxmlError.ERR_OK;
            }

            do
            {
            } while (ReadByte() != (byte) WbxmlElement.END);

            return WbxmlError.ERR_OK;
        }

        public void ParseStartdoc(WbxmlDecoder xDmParser)
        {
            try
            {
                ReadByte();
                xDmParser._publicUid = ReadUInt32();
                if (xDmParser._publicUid == 0)
                {
                    ReadUInt32();
                }

                ReadUInt32();
                xDmParser._stringTable = ParseStringTable();
                _stringT = new string(xDmParser._stringTable);
            }
            catch (IOException e)
            {
                Log.E("WbxmlDecoder.ParseStartDoc: " + e.ToString());
            }
        }

        public string ParseStrT()
        {
            var byteArrayOutputStream = new MemoryStream();
            try
            {
                for (var uint1 = ReadUInt32();
                    _stringT[uint1] != 0;
                    uint1++)
                {
                    byteArrayOutputStream.WriteByte((byte) _stringT[uint1]);
                }
            }
            catch (IOException e)
            {
                Log.E("WbxmlDecoder.ParseStrT" + e.ToString());
            }

            return Encoding.UTF8.GetString(byteArrayOutputStream.ToArray());
        }

        public string ParseStrI()
        {
            var byteArrayOutputStream = new MemoryStream();
            while (true)
            {
                var byte1 = ReadByte();
                if (byte1 == 0)
                {
                    string str = Encoding.UTF8.GetString(byteArrayOutputStream.ToArray());
                    byteArrayOutputStream.Close();
                    return str;
                }

                if (byte1 != 255)
                {
                    byteArrayOutputStream.WriteByte(byte1);
                }
                else
                {
                    throw new IOException("Unexpected EOF ParseStrI");
                }
            }
        }

        public string? ParseExtension(WbxmlElement element)
        {
            var byteArrayOutputStream = new MemoryStream();
            if (element == WbxmlElement.OPAQUE)
            {
                try
                {
                    var nextUint32 = ReadUInt32();
                    for (var i2 = 0; i2 < nextUint32; i2++)
                    {
                        byteArrayOutputStream.WriteByte(ReadByte());
                    }

                    return Encoding.UTF8.GetString(byteArrayOutputStream.ToArray());
                }
                catch (IOException e)
                {
                    Log.E("WbxmlDecoder.ParseStartDoc" + e.ToString());
                }
            }

            return null;
        }

        public string? ParseStringTable()
        {
            var byteArrayOutputStream = new MemoryStream();
            try
            {
                var uint1 = ReadUInt32();
                for (var i = 0; i < uint1; i++)
                {
                    byteArrayOutputStream.WriteByte(ReadByte());
                }

                return Encoding.UTF8.GetString(byteArrayOutputStream.ToArray());
            }
            catch (IOException e)
            {
                Log.E("WbxmlDecoder.ParseStringTable: " + e.ToString());
                return null;
            }
        }

        public int ReadUInt32()
        {
            int byte1;
            var i = 0;
            for (var i2 = 0; i2 < 5 && (byte1 = ReadByte()) >= 0; i2++)
            {
                i = (i << 7) | (byte1 & 127);
                if ((byte1 & 128) == 0)
                {
                    return i;
                }
            }

            return 0;
        }

        public byte ReadByte()
        {
            byte[] bArr = _buffer;
            var i = Index;
            Index = i + 1;
            var i2 = (byte) (bArr[i] & 255);
            if (i2 != 255)
            {
                return i2;
            }

            throw new IOException("Unexpected EOF ReadByte");
        }
    }
}