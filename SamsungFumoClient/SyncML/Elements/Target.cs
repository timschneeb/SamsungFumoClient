using System.ComponentModel.DataAnnotations;
using System.IO;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class Target : IXmlElement
    {
        [Required] public string? LocURI { set; get; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.TARGET);
            {
                writer.WriteElementString(WbxmlElement.LOCURI, LocURI!);
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? param = null)
        {
            var checkElementResult = parser.ParseCheckElement(WbxmlElement.TARGET);
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

            var elementResult = parser.ParseElement(WbxmlElement.LOCURI);
            if (elementResult != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(elementResult);
            }

            string locUri = parser.CurrentInnerText;
            try
            {
                if (parser.CurrentElement() == WbxmlElement.LOCNAME)
                {
                    parser.SkipElement();
                }
            }
            catch (IOException e)
            {
                Log.E(e.ToString());
            }

            var checkEndElement = parser.ParseCheckElement(WbxmlElement.END);
            if (checkEndElement != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(checkEndElement);
            }

            LocURI = !string.IsNullOrEmpty(locUri) ? locUri : null;
            return this;
        }
    }
}