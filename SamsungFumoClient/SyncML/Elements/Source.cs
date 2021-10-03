using System.ComponentModel.DataAnnotations;
using System.IO;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class Source : IXmlElement
    {
        [Required] public string? LocURI { get; set; }
        public string? LocName { get; set; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.SOURCE);
            {
                writer.WriteElementString(WbxmlElement.LOCURI, LocURI!);
                if (LocName != null)
                {
                    writer.WriteElementString(WbxmlElement.LOCNAME, LocName);
                }
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? param = null)
        {
            var _xdmParParseCheckElement = parser.ParseCheckElement(WbxmlElement.SOURCE);
            if (_xdmParParseCheckElement != 0)
            {
                throw new SyncMlParseException(_xdmParParseCheckElement);
            }

            var _xdmParParseZeroBitTagCheck = parser.ZeroBitTagCheck();
            if (_xdmParParseZeroBitTagCheck == WbxmlError.ZEROBIT_TAG)
            {
                return this;
            }

            if (_xdmParParseZeroBitTagCheck != 0)
            {
                Log.E("not WBXML_ERR_OK");
                throw new SyncMlParseException(_xdmParParseZeroBitTagCheck);
            }

            var _xdmParParseElement = parser.ParseElement(WbxmlElement.LOCURI);
            if (_xdmParParseElement != 0)
            {
                throw new SyncMlParseException(_xdmParParseElement);
            }

            string str = parser.CurrentInnerText;
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

            var xdmParParseCheckElement2 = parser.ParseCheckElement(WbxmlElement.END);
            if (xdmParParseCheckElement2 != 0)
            {
                throw new SyncMlParseException(xdmParParseCheckElement2);
            }

            LocURI = !string.IsNullOrEmpty(str) ? str : null;
            return this;
        }
    }
}