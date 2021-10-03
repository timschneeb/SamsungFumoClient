using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;

namespace SamsungFumoClient.SyncML
{
    public class SyncMlWriter : WbxmlEncoder
    {
        public byte[] GetBytes()
        {
            return GetBuffer().ToArray();
        }

        public WbxmlError BeginDocument()
        {
            if (!StartDocument(WbxmlCharset.UTF8, "-//SYNCML//DTD SyncML 1.2//EN", 0x1D))
            {
                return WbxmlError.BUFFER_TOO_SMALL;
            }

            WriteStartElement(WbxmlElement.SYNCML);
            return WbxmlError.ERR_OK;
        }

        public WbxmlError EndDocument()
        {
            if (EndElement())
            {
                return WbxmlError.ERR_OK;
            }

            return WbxmlError.BUFFER_TOO_SMALL;
        }

        public void WriteSyncHdr(SyncHdr syncHdr)
        {
            syncHdr.Write(this);
        }

        public void WriteSyncBody(SyncBody syncBody)
        {
            syncBody.Write(this);
        }

        public WbxmlError SwitchCodepage(WbxmlCodepage page)
        {
            return AddSwitchpage((byte) page) ? WbxmlError.ERR_OK : WbxmlError.BUFFER_TOO_SMALL;
        }

        public WbxmlError WriteOpaqueString(string str)
        {
            if (!AddOpaque(str.ToCharArray(), str.ToCharArray().Length))
            {
                return WbxmlError.BUFFER_TOO_SMALL;
            }

            return WbxmlError.ERR_OK;
        }

        public void WriteElementString(byte tag, string value)
        {
            WriteStartElement(tag);
            WriteString(value);
            WriteEndElement();
        }

        public void WriteElementString(WbxmlElement tag, string value)
        {
            WriteElementString((byte) tag, value);
        }

        public void WriteElementString(MetinfTag tag, string value)
        {
            WriteElementString((byte) tag, value);
        }

        public WbxmlError WriteStartElement(WbxmlElement tag)
        {
            return WriteStartElement((byte) tag);
        }

        public WbxmlError WriteStartElement(MetinfTag tag)
        {
            return WriteStartElement((byte) tag);
        }

        public WbxmlError WriteStartElement(byte tag)
        {
            return !StartElement(tag, true) ? WbxmlError.BUFFER_TOO_SMALL : WbxmlError.ERR_OK;
        }

        public WbxmlError WriteString(string str)
        {
            return !AddContent(str) ? WbxmlError.BUFFER_TOO_SMALL : WbxmlError.ERR_OK;
        }

        public WbxmlError WriteEndElement()
        {
            return !EndElement() ? WbxmlError.BUFFER_TOO_SMALL : WbxmlError.ERR_OK;
        }

        public WbxmlError WriteSelfClosingElement(WbxmlElement tag)
        {
            return !StartElement((byte) tag, false) ? WbxmlError.BUFFER_TOO_SMALL : WbxmlError.ERR_OK;
        }

        public WbxmlError WriteSelfClosingElement(MetinfTag tag)
        {
            return !StartElement((byte) tag, false) ? WbxmlError.BUFFER_TOO_SMALL : WbxmlError.ERR_OK;
        }
    }
}