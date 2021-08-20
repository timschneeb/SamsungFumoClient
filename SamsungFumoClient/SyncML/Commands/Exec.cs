using System.IO;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Commands
{
    public class Exec : Cmd
    {
        public string? Correlator { set; get; }
        public Meta? Meta { set; get; }

        public override void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.EXEC);
            {
                base.Write(writer);
                if (Correlator != null)
                {
                    writer.WriteElementString(WbxmlElement.CORRELATOR, Correlator!);
                }

                Meta?.Write(writer);
            }
            writer.WriteEndElement();
        }

        public override IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var xdmParParseCheckElement = parser.ParseCheckElement(WbxmlElement.EXEC);
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

            do
            {
                var element = parser.CurrentElement();

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
                    case WbxmlElement.CMDID:
                        error = parser.ParseElement(element);
                        CmdID = int.Parse(parser.CurrentInnerText);
                        continue;
                    case WbxmlElement.ITEM:
                        Item = parser.ParseItemlist(Item);
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
                    case WbxmlElement.NORESP:
                        IsNoResp = (int) parser.ParseBlankElement(element);
                        continue;
                    case WbxmlElement.CORRELATOR:
                        error = parser.ParseElement(element);
                        Correlator = parser.CurrentInnerText;
                        continue;
                    default:
                    {
                        error = WbxmlError.ZEROBIT_TAG;
                        continue;
                    }
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}