using System;

namespace ThePBone.WbXml2
{
    public class WbXmlParseException : Exception
    {
        public WBXMLError WbXmlError { get; }

        public WbXmlParseException(WBXMLError wbxmlError) : base(wbxmlError.ToString())
        {
            WbXmlError = wbxmlError;
        }
    }
}