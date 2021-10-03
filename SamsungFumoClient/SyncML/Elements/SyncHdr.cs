using System.ComponentModel.DataAnnotations;
using System.IO;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class SyncHdr : IXmlElement
    {
        [Required] public string? VerDTD { set; get; } = "1.2";
        [Required] public string? VerProto { set; get; } = "DM/1.2";

        [Required] public string? SessionID { set; get; }
        [Required] public int? MsgID { set; get; }

        public Target? Target { set; get; }
        public Source? Source { set; get; }

        /**
         * Server only
         */
        public string? RespURI { set; get; }

        public int? IsNoResp { set; get; }

        public Cred? Cred { set; get; }
        public Meta? Meta { set; get; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.SYNCHDR);
            {
                writer.WriteElementString(WbxmlElement.VERDTD, VerDTD!);
                writer.WriteElementString(WbxmlElement.VERPROTO, VerProto!);
                writer.WriteElementString(WbxmlElement.SESSIONID, SessionID!);
                writer.WriteElementString(WbxmlElement.MSGID, MsgID.ToString()!);

                if (RespURI != null)
                {
                    writer.WriteElementString(WbxmlElement.RESPURI, RespURI);
                }

                if (IsNoResp is > 0)
                {
                    writer.WriteSelfClosingElement(WbxmlElement.NORESP);
                }

                Target?.Write(writer);
                Source?.Write(writer);

                Cred?.Write(writer);
                Meta?.Write(writer);
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var checkElementResult = parser.ParseCheckElement(WbxmlElement.SYNCHDR);
            if (checkElementResult != 0)
            {
                throw new SyncMlParseException(checkElementResult);
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

            var currentElement = WbxmlElement.UNSET;
            do
            {
                try
                {
                    currentElement = parser.CurrentElement();
                }
                catch (IOException e)
                {
                    Log.E("xdmParParseCurrentElement error = " + e);
                }

                switch (currentElement)
                {
                    case WbxmlElement.END:
                        parser.ParseReadElement();
                        return this;
                    case WbxmlElement.CODEPAGE:
                        parser.ParseReadElement();
                        currentElement = parser.ParseReadElement();
                        parser.CurrentCodePage = (WbxmlCodepage) currentElement;
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
                    case WbxmlElement.NORESP:
                        IsNoResp = (int) parser.ParseBlankElement(currentElement);
                        continue;
                    case WbxmlElement.RESPURI:
                        error = parser.ParseElement(currentElement);
                        RespURI = parser.CurrentInnerText;
                        continue;
                    case WbxmlElement.SESSIONID:
                        error = parser.ParseElement(currentElement);
                        SessionID = parser.CurrentInnerText;
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
                    case WbxmlElement.MSGID:
                        error = parser.ParseElement(currentElement);
                        MsgID = int.Parse(parser.CurrentInnerText);
                        continue;
                    case WbxmlElement.VERDTD:
                        error = parser.ParseElement(currentElement);
                        VerDTD = parser.CurrentInnerText;
                        continue;
                    default:
                    {
                        if (currentElement == WbxmlElement.VERPROTO)
                        {
                            error = parser.ParseElement(currentElement);
                            VerProto = parser.CurrentInnerText;
                            continue;
                        }

                        error = WbxmlError.UNKNOWN_ELEMENT;
                        continue;
                    }
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}