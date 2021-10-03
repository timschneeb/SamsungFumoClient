using System.IO;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML
{
    public class SyncMlParser : WbxmlDecoder
    {
        public SyncMlParser(byte[] wbxml) : base(wbxml)
        {
        }

        public SyncDocument Parse()
        {
            var document = new SyncDocument();

            Index = 0;
            ParseStartdoc(this);
            var element = CurrentElement();

            if (element != WbxmlElement.SYNCML)
            {
                throw new SyncMlParseException(WbxmlError.UNKNOWN_ELEMENT);
            }

            var checkSyncMlElement = ParseCheckElement(WbxmlElement.SYNCML);
            if (checkSyncMlElement != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(checkSyncMlElement);
            }

            var error = ZeroBitTagCheck();
            if (error == WbxmlError.ZEROBIT_TAG)
            {
                return document;
            }

            if (error != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(error);
            }

            do
            {
                element = CurrentElement();

                switch (element)
                {
                    case WbxmlElement.END:
                        return document;
                    case WbxmlElement.CODEPAGE:
                        ParseReadElement();
                        ParseReadElement();
                        break;
                    case WbxmlElement.SYNCBODY:
                        try
                        {
                            document.SyncBody = (SyncBody) new SyncBody().Parse(this);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        break;
                    case WbxmlElement.SYNCHDR:
                        try
                        {
                            document.SyncHdr = (SyncHdr) new SyncHdr().Parse(this);
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        break;
                    default:
                        error = WbxmlError.UNKNOWN_ELEMENT;
                        break;
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}