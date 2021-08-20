using System;

namespace ThePBone.WbXml2
{
    public class WbXmlWriteException : Exception
    {
        public WBXMLError WbXmlError { get; }

        public WbXmlWriteException(WBXMLError wbxmlError) : base(wbxmlError.ToString())
        {
            WbXmlError = wbxmlError;
        }
    }
}