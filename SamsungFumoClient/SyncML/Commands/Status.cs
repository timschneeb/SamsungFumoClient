using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Commands
{
    public class Status : Cmd
    {
        [Required] public int? MsgRef { set; get; }
        [Required] public int? CmdRef { set; get; }
        [Required] public string? Cmd { set; get; }

        public string? TargetRef { set; get; } //TODO this is a list
        public string? SourceRef { set; get; } //TODO this is a list
        public Chal? Chal { set; get; }
        public Cred? Cred { set; get; }

        public override void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.STATUS);
            {
                base.Write(writer);
                writer.WriteElementString(WbxmlElement.MSGREF, MsgRef.ToString()!);
                writer.WriteElementString(WbxmlElement.CMDREF, CmdRef.ToString()!);
                writer.WriteElementString(WbxmlElement.CMD, Cmd!);
                if (TargetRef != null)
                {
                    writer.WriteElementString(WbxmlElement.TARGETREF, TargetRef); // TODO Write Elelist instead
                }

                if (SourceRef != null)
                {
                    writer.WriteElementString(WbxmlElement.SOURCEREF, SourceRef); // TODO Write Elelist instead
                }

                Chal?.Write(writer);
                Cred?.Write(writer);
            }
            writer.WriteEndElement();
        }

        public override IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var checkStatus = parser.ParseCheckElement(WbxmlElement.STATUS);
            if (checkStatus != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(checkStatus);
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
                    case WbxmlElement.STATUS:
                        parser.ParseReadElement();
                        continue;
                    case WbxmlElement.CODEPAGE:
                        parser.ParseReadElement();
                        element = parser.ParseReadElement();
                        parser.CurrentCodePage = (WbxmlCodepage) element;
                        continue;
                    case WbxmlElement.ITEM:
                        Item = parser.ParseItemlist(Item);
                        continue;
                    case WbxmlElement.MSGREF:
                        error = parser.ParseElement(element);
                        MsgRef = int.Parse(parser.CurrentInnerText);
                        continue;
                    case WbxmlElement.SOURCEREF:
                    {
                        var xDmList = new List<string>();
                        SourceRef = parser.ParseElelist(element, xDmList)![0];
                        // TODO only first list element checked
                        continue;
                    }
                    case WbxmlElement.TARGETREF:
                    {
                        var xDMList2 = new List<string>();
                        TargetRef = parser.ParseElelist(element, xDMList2)![0];
                        // TODO only first list element checked
                        continue;
                    }
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
                    case WbxmlElement.CHAL:
                        try
                        {
                            Chal = (Chal) new Chal().Parse(parser);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.CMD:
                        error = parser.ParseElement(element);
                        Cmd = parser.CurrentInnerText;
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