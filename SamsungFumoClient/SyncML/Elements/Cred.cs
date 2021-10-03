using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class Cred : IXmlElement
    {
        public Meta? Meta { set; get; }
        public string? Data { set; get; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.CRED);
            {
                Meta?.Write(writer);
                if (Data != null)
                {
                    writer.WriteElementString(WbxmlElement.DATA, Data);
                }
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var error = parser.ParseCheckElement(WbxmlElement.CRED);
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
                    case 0:
                        parser.ParseReadElement();
                        element = parser.ParseReadElement();
                        parser.CurrentCodePage = (WbxmlCodepage) element;
                        continue;
                    case WbxmlElement.DATA:
                        error = parser.ParseElement(element);
                        Data = parser.CurrentInnerText;
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

                        Meta = parser.Meta;
                        continue;
                    default:
                        error = WbxmlError.UNKNOWN_ELEMENT;
                        continue;
                }
            } while (error == 0);

            return this;
        }
    }
}