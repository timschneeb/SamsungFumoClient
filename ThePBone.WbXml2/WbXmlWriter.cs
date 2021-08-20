// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace ThePBone.WbXml2
{
    /**
    * <summary>Convert XML and write as WAP binary XML content into byte array</summary>
    */
    public unsafe class WbXmlWriter
    {
        private WBXMLConvXML2WBXML* _self;
        private WBXMLVersion _wbxmlVersion = WBXMLVersion.WBXML_VERSION_13;

        /**
        * <exception cref="WbXmlWriteException">Unable to allocate native conversion object</exception>
        */
        public WbXmlWriter()
        {
            WBXMLError error;
            fixed (WBXMLConvXML2WBXML** s = &_self)
            {
                error = WbXml2.wbxml_conv_xml2wbxml_create(s);
            }

            if (error != WBXMLError.WBXML_OK)
            {
                throw new WbXmlWriteException(error);
            }
        }

        /** 
        *  <summary>Disable public ID (default: TRUE/ENABLED).</summary>
        *  <remarks>Usually you don't want to produce WBXML documents which are
        *           really anonymous. You want a known public ID or a DTD name
        *           to determine the document type. Some specifications like
        *           Microsoft's ActiveSync explicitly require fully anonymous
        *           WBXML documents. If you need this then you must disable
        *           the public ID mechanism.</remarks>
        */
        public void DisablePublicId()
        {
            WbXml2.wbxml_conv_xml2wbxml_disable_public_id(_self);
        }

        /**
        *  <summary>Disable string table (default: TRUE/ENABLED)</summary>
        */
        public void DisableStringTable()
        {
            WbXml2.wbxml_conv_xml2wbxml_disable_string_table(_self);
        }

        /**
        * <summary>Enable whitespace preservation (default: FALSE).</summary>
        */
        public void EnablePreserveWhitespaces()
        {
            WbXml2.wbxml_conv_xml2wbxml_enable_preserve_whitespaces(_self);
        }

        /**
        * <summary>Set/get the WBXML version (default: 1.3).</summary>
        */
        public WBXMLVersion WbXmlVersion
        {
            get => _wbxmlVersion;
            set
            {
                _wbxmlVersion = value;
                WbXml2.wbxml_conv_xml2wbxml_set_version(_self, value);
            }
        }

        /**
        * <summary>Convert XML to WBXML</summary>
        * <exception cref="WbXmlWriteException">Unable to parse XML content</exception>
        */
        public byte[] WriteBytes(string xml)
        {
            var xmlAsBytes = Encoding.UTF8.GetBytes(xml);
            byte* wbxmlBuffer;
            ulong wbxmlLength;
            fixed (byte* xmlPtr = xmlAsBytes)
            {
                var error = WbXml2.wbxml_conv_xml2wbxml_run(_self,
                    xmlPtr,
                    (ulong) xmlAsBytes.Length,
                    &wbxmlBuffer,
                    &wbxmlLength);

                if (error != WBXMLError.WBXML_OK)
                {
                    throw new WbXmlWriteException(error);
                }
            }

            var wbxml = new byte[wbxmlLength];
            Marshal.Copy((IntPtr) wbxmlBuffer, wbxml, 0, (int) wbxmlLength);
            return wbxml;
        }

        ~WbXmlWriter()
        {
            WbXml2.wbxml_conv_xml2wbxml_destroy(_self);
        }
    }
}