using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class Chal : IXmlElement
    {
        public Meta? Meta { set; get; }

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.CHAL);
            {
                Meta?.Write(writer);
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? _ = null)
        {
            var checkElementResult = parser.ParseCheckElement(WbxmlElement.CHAL);
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
                throw new SyncMlParseException(zeroBitTagCheck);
            }

            Meta = (Meta) new Meta().Parse(parser);
            var checkEnd = parser.ParseCheckElement(WbxmlElement.END);
            if (checkEnd != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(checkEnd);
            }

            return this;
        }
    }
}