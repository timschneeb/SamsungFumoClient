using System.IO;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class Item : IXmlElement
    {
        public Source? Source { set; get; }
        public Target? Target { set; get; }

        public PcData? Data { set; get; }
        public Meta? Meta { set; get; }

        /**
         * Parser only
         */
        public int? MoreData { set; get; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.ITEM);
            {
                Source?.Write(writer);
                Target?.Write(writer);
                Meta?.Write(writer);
                Data?.Write(writer);
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var xdmParParseCheckElement = parser.ParseCheckElement(WbxmlElement.ITEM);
            if (xdmParParseCheckElement != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(xdmParParseCheckElement);
            }

            var error = parser.ZeroBitTagCheck();
            if (error == WbxmlError.ZEROBIT_TAG)
            {
                return this;
            }

            if (error != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(error);
            }

            var element = WbxmlElement.UNSET;
            do
            {
                try
                {
                    element = parser.CurrentElement();
                }
                catch (IOException e)
                {
                    Log.E(e.ToString());
                }

                switch (element)
                {
                    case WbxmlElement.END:
                        parser.ParseReadElement();
                        return this;
                    case WbxmlElement.CODEPAGE:
                        parser.ParseReadElement();
                        element = parser.ParseReadElement();
                        parser.CurrentCodePage = (WbxmlCodepage) element;
                        continue;
                    case WbxmlElement.DATA:
                        try
                        {
                            Data = (PcData) new PcData().Parse(parser, element);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.META:
                        try
                        {
                            Meta = (Meta) new Meta().Parse(parser);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.SOURCE:
                        try
                        {
                            Source = (Source) new Source().Parse(parser);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.TARGET:
                        try
                        {
                            Target = (Target) new Target().Parse(parser);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.MOREDATA:
                        MoreData = (int) parser.ParseBlankElement(element);
                        continue;
                    default:
                    {
                        error = WbxmlError.UNKNOWN_ELEMENT;
                        continue;
                    }
                }
            } while (error == WbxmlError.ERR_OK);

            throw new SyncMlParseException(error);
        }
    }
}