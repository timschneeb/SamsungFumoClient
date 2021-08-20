using System.IO;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class Meta : IXmlElement
    {
        public string? Format { set; get; }
        public string? Type { set; get; }
        public int? Size { set; get; }
        public string? NextNonce { set; get; }
        public int? MaxMsgSize { set; get; }
        public int? MaxObjSize { set; get; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.META);
            writer.SwitchCodepage(WbxmlCodepage.METINF);
            {
                if (Format != null)
                {
                    writer.WriteElementString(MetinfTag.FORMAT, Format);
                }

                if (Type != null)
                {
                    writer.WriteElementString(MetinfTag.TYPE, Type);
                }

                if (Size != null)
                {
                    writer.WriteElementString(MetinfTag.SIZE, Size.ToString()!);
                }

                if (NextNonce != null)
                {
                    writer.WriteElementString(MetinfTag.NEXTNONCE, NextNonce);
                }

                if (MaxMsgSize != null)
                {
                    writer.WriteElementString(MetinfTag.MAXMSGSIZE, MaxMsgSize.ToString()!);
                }

                if (MaxObjSize != null)
                {
                    writer.WriteElementString(MetinfTag.MAXOBJSIZE, MaxObjSize.ToString()!);
                }
            }
            writer.SwitchCodepage(WbxmlCodepage.SYNCML);
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var checkElementResult = parser.ParseCheckElement(WbxmlElement.META);
            if (checkElementResult != 0)
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
                throw new SyncMlParseException(zeroBitTagCheck);
            }

            WbxmlElement element;
            try
            {
                element = parser.CurrentElement();
            }
            catch (IOException e)
            {
                Log.E(e.ToString());
                element = WbxmlElement.UNSET;
            }

            if (element == WbxmlElement.END)
            {
                parser.ParseReadElement();
                return this;
            }

            var checkCodepage = parser.ParseCheckElement(WbxmlElement.CODEPAGE);
            if (checkCodepage != 0)
            {
                throw new SyncMlParseException(checkCodepage);
            }

            var error = parser.ParseCheckElement(WbxmlElement.END);
            if (error != 0)
            {
                throw new SyncMlParseException(error);
            }

            parser.CurrentCodePage = WbxmlCodepage.METINF;
            do
            {
                try
                {
                    element = parser.CurrentElement();
                }
                catch (IOException e2)
                {
                    Log.E(e2.ToString());
                }

                switch (element)
                {
                    case WbxmlElement.END:
                        parser.ParseReadElement();
                        parser.Meta = this;
                        return this;
                    case WbxmlElement.CODEPAGE:
                        parser.ParseReadElement();
                        element = parser.ParseReadElement();
                        parser.CurrentCodePage = (WbxmlCodepage) element;
                        break;
                    default:
                        switch ((MetinfTag) element)
                        {
                            case MetinfTag.NEXTNONCE:
                                error = parser.ParseElement(element);
                                NextNonce = parser.CurrentInnerText;
                                continue;
                            case MetinfTag.ANCHOR:
                                Log.E("Parser: Metinf.Anchor not implemented");
                                continue;
                            case MetinfTag.EMI:
                                error = parser.ParseElement(element);
                                Log.E("Parser: Metinf.Emi not implemented");
                                continue;
                            case MetinfTag.MARK:
                                error = parser.ParseElement(element);
                                Log.E("Parser: Metinf.Mark not implemented");
                                continue;
                            case MetinfTag.MAXMSGSIZE:
                                error = parser.ParseElement(element);
                                MaxMsgSize = int.Parse(parser.CurrentInnerText);
                                continue;
                            case MetinfTag.MEM:
                                Log.E("Parser: Metinf.Mem not implemented");
                                continue;
                            case MetinfTag.SIZE:
                                error = parser.ParseElement(element);
                                Size = int.Parse(parser.CurrentInnerText);
                                continue;
                            case MetinfTag.TYPE:
                                error = parser.ParseElement(element);
                                Type = parser.CurrentInnerText;
                                continue;
                            case MetinfTag.VERSION:
                                error = parser.ParseElement(element);
                                Log.E("Parser: Meta.Version not implemented");
                                continue;
                            case MetinfTag.MAXOBJSIZE:
                                error = parser.ParseElement(element);
                                MaxObjSize = int.Parse(parser.CurrentInnerText);
                                continue;
                            case MetinfTag.FORMAT:
                                error = parser.ParseElement(element);
                                Format = parser.CurrentInnerText;
                                continue;
                            default:
                                error = WbxmlError.UNKNOWN_ELEMENT;
                                continue;
                        }
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}