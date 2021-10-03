using System.Diagnostics;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class PcData : IXmlElement
    {
        public string? Data { set; get; }
        public PcDataType? Type { set; get; } = PcDataType.STRING;

        public void Write(SyncMlWriter writer)
        {
            // Only type 0 supported
            Debug.Assert(Type == PcDataType.STRING);
            writer.WriteElementString(WbxmlElement.DATA, Data!);
        }

        public IXmlElement Parse(SyncMlParser parser, object? param)
        {
            if (param == null)
            {
                throw new SyncMlParseException(WbxmlError.INVALID_PARAMETER);
            }

            var checkElementResult = parser.ParseCheckElement((WbxmlElement) param);
            if (checkElementResult != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(checkElementResult);
            }

            var zeroBitTagCheck = parser.ZeroBitTagCheck();
            if (zeroBitTagCheck == WbxmlError.ZEROBIT_TAG)
            {
                return this;
            }

            if (zeroBitTagCheck != WbxmlError.ERR_OK)
            {
                Log.E("not WBXML_ERR_OK");
                throw new SyncMlParseException(zeroBitTagCheck);
            }

            var element = (WbxmlElement) parser.ReadByte();
            switch (element)
            {
                case WbxmlElement.STR_I:
                    SetStringPcdata(parser.ParseStrI());
                    break;
                case WbxmlElement.STR_T:
                    SetStringPcdata(parser.ParseStrT());
                    break;
                case WbxmlElement.OPAQUE:
                {
                    var parsed = parser.ParseExtension(element);
                    Type = PcDataType.OPAQUE;
                    SetStringPcdata(parsed);
                    break;
                }
                case WbxmlElement.CODEPAGE:
                {
                    parser.CurrentCodePage = (WbxmlCodepage) parser.ParseReadElement();
                    var subElement = (MetinfTag) parser.CurrentElement();
                    do
                    {
                        switch (parser.CurrentCodePage)
                        {
                            case WbxmlCodepage.METINF when subElement == MetinfTag.ANCHOR:
                                Log.E("Parser: Pcdata.Anchor not implemented");
                                throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                            case WbxmlCodepage.METINF when subElement == MetinfTag.MEM:
                                Log.E("Parser: Pcdata.Mem not implemented");
                                throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                        }

                        if (subElement == 0)
                        {
                            parser.ParseReadElement();
                            parser.ParseReadElement();
                        }

                        subElement = (MetinfTag) parser.CurrentElement();
                    } while ((WbxmlElement) subElement != WbxmlElement.END);

                    break;
                }
                default:
                    parser.Index -= 1;
                    parser.SkipElement();

                    Type = PcDataType.EXTENSION;
                    Data = null;
                    break;
            }

            var checkResult = parser.ParseCheckElement(WbxmlElement.END);
            if (checkResult != 0)
            {
                throw new SyncMlParseException(checkResult);
            }

            return this;
        }

        private void SetStringPcdata(string? str)
        {
            Type = PcDataType.STRING;
            Data = str;
        }
    }
}