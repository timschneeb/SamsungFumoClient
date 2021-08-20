using System.ComponentModel.DataAnnotations;
using System.IO;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Commands
{
    public class Results : Cmd
    {
        [Required] public int? MsgRef { set; get; }
        [Required] public int? CmdRef { set; get; }
        public string? SourceRef { set; get; }
        public string? TargetRef { set; get; }
        public Meta? Meta { set; get; }

        public override void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.RESULTS);
            {
                base.Write(writer);
                writer.WriteElementString(WbxmlElement.MSGREF, MsgRef.ToString()!);
                writer.WriteElementString(WbxmlElement.CMDREF, CmdRef.ToString()!);
                if (SourceRef != null)
                {
                    writer.WriteElementString(WbxmlElement.SOURCEREF, SourceRef!);
                }

                if (TargetRef != null)
                {
                    writer.WriteElementString(WbxmlElement.TARGETREF, TargetRef!);
                }

                Meta?.Write(writer);
            }
            writer.WriteEndElement();
        }

        public override IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var xdmParParseCheckElement = parser.ParseCheckElement(WbxmlElement.RESULTS);
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
                    case WbxmlElement.MSGREF:
                        error = parser.ParseElement(element);
                        MsgRef = int.Parse(parser.CurrentInnerText);
                        continue;
                    case WbxmlElement.SOURCEREF:
                        error = parser.ParseElement(element);
                        SourceRef = parser.CurrentInnerText;
                        continue;
                    case WbxmlElement.TARGETREF:
                        error = parser.ParseElement(element);
                        TargetRef = parser.CurrentInnerText;
                        continue;
                    case WbxmlElement.CMDID:
                        error = parser.ParseElement(element);
                        CmdID = int.Parse(parser.CurrentInnerText);
                        continue;
                    case WbxmlElement.CMDREF:
                        error = parser.ParseElement(element);
                        CmdRef = int.Parse(parser.CurrentInnerText);
                        continue;
                    default:
                        error = WbxmlError.UNKNOWN_ELEMENT;
                        continue;
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}