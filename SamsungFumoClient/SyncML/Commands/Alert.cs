using System.IO;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Commands
{
    public class Alert : Cmd
    {
        public Cred? Cred { set; get; }
        public string? Correlator { set; get; } = null!;

        public override void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.ALERT);
            {
                base.Write(writer);

                Cred?.Write(writer);
                if (Correlator != null)
                {
                    writer.WriteElementString(WbxmlElement.CORRELATOR, Correlator!);
                }
            }
            writer.WriteEndElement();
        }

        public override IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var checkAlert = parser.ParseCheckElement(WbxmlElement.ALERT);
            if (checkAlert != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(checkAlert);
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
                    case WbxmlElement.CMDID:
                        error = parser.ParseElement(element);
                        CmdID = int.Parse(parser.CurrentInnerText);
                        continue;
                    case WbxmlElement.ITEM:
                        Item = parser.ParseItemlist(Item);
                        continue;
                    case WbxmlElement.NORESP:
                        IsNoResp = (int) parser.ParseBlankElement(element);
                        continue;
                    case WbxmlElement.CRED:
                        try
                        {
                            Cred = (Cred) new Cred().Parse(parser);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.DATA:
                        error = parser.ParseElement(element);
                        Data = parser.CurrentInnerText;
                        continue;
                    default:
                    {
                        error = WbxmlError.UNKNOWN_ELEMENT;
                        continue;
                    }
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}