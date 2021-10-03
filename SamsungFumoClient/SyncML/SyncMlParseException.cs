using System;
using SamsungFumoClient.SyncML.Enum;

namespace SamsungFumoClient.SyncML
{
    public class SyncMlParseException : Exception
    {
        public SyncMlParseException(WbxmlError wbxmlError) : base(wbxmlError.ToString())
        {
            WbXmlError = wbxmlError;
        }

        public WbxmlError WbXmlError { get; }
    }
}