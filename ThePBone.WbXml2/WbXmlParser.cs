using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace ThePBone.WbXml2
{
    /**
    * <summary>Parse WAP binary XML content and convert it into XML</summary>
    */
    public unsafe class WbXmlParser
    {
        private WBXMLConvWBXML2XML* _self;
        private WBXMLGenXMLType _xmlGenerationType = WBXMLGenXMLType.WBXML_GEN_XML_INDENT;
        private WBXMLLanguage _wbXmlLanguage = WBXMLLanguage.WBXML_LANG_UNKNOWN;
        private WBXMLCharsetMIBEnum _charset = WBXMLCharsetMIBEnum.WBXML_CHARSET_UTF_8;
        private byte _indentSpaces = 3;

        /**
        * <exception cref="WbXmlParseException">Unable to allocate native conversion object</exception>
        */
        public WbXmlParser()
        {
            WBXMLError error;
            fixed (WBXMLConvWBXML2XML** s = &_self)
            {
                error = WbXml2.wbxml_conv_wbxml2xml_create(s);
            }

            if (error != WBXMLError.WBXML_OK)
            {
                throw new WbXmlParseException(error);
            }

            // Set custom defaults
            WbXml2.wbxml_conv_wbxml2xml_set_indent(_self, _indentSpaces);
        }

        /**
        * <summary>Enable whitespace preservation (default: FALSE).</summary>
        */
        public void EnablePreserveWhitespaces()
        {
            WbXml2.wbxml_conv_wbxml2xml_enable_preserve_whitespaces(_self);
        }

        /**
        * <summary>Set the XML generation type (default: WBXML_GEN_XML_INDENT).</summary>
        */
        public WBXMLGenXMLType XmlGenerationType
        {
            set
            {
                _xmlGenerationType = value;
                WbXml2.wbxml_conv_wbxml2xml_set_gen_type(_self, value);
            }
            get => _xmlGenerationType;
        }

        /**
        * <summary>Set the used WBXML language.<br/>
        *          The language is usually detected by the specified public ID in the document.
        *          If the public ID is set then it overrides the language.</summary>
        */
        public WBXMLLanguage WbXmlLanguage
        {
            set
            {
                _wbXmlLanguage = value;
                WbXml2.wbxml_conv_wbxml2xml_set_language(_self, value);
            }
            get => _wbXmlLanguage;
        }

        /**
        * <summary>Set the used character set.<br/>
        *          The default character set is UTF-8.
        *          If the document specifies a character set by it own
        *          then this character set overrides the parameter charset.</summary>
        */
        public WBXMLCharsetMIBEnum Charset
        {
            set
            {
                _charset = value;
                WbXml2.wbxml_conv_wbxml2xml_set_charset(_self, value);
            }
            get => _charset;
        }

        /**
        * <summary>Set the indent of the generated XML document (please see EXPAT default).</summary>
        */
        public byte IndentSpaces
        {
            set
            {
                _indentSpaces = value;
                WbXml2.wbxml_conv_wbxml2xml_set_indent(_self, value);
            }
            get => _indentSpaces;
        }

        /**
        * <summary>Convert WBXML to XML as string</summary>
        * <exception cref="WbXmlParseException">Unable to parse WbXml content</exception>
        */
        public string WriteXmlString(byte[] wbxml)
        {
            byte* xmlBuffer;
            ulong xmlLength;
            fixed (byte* wbxmlPtr = wbxml)
            {
                var error = WbXml2.wbxml_conv_wbxml2xml_run(_self,
                    wbxmlPtr,
                    (ulong) wbxml.Length,
                    &xmlBuffer,
                    &xmlLength);

                if (error != WBXMLError.WBXML_OK)
                {
                    throw new WbXmlParseException(error);
                }
            }

            var xml = new byte[xmlLength];
            Marshal.Copy((IntPtr) xmlBuffer, xml, 0, (int) xmlLength);
            return Encoding.UTF8.GetString(xml);
        }

        /**
        * <summary>Convert WBXML to XML document</summary>
        * <exception cref="WbXmlParseException">Unable to parse WbXml content</exception>
        * <exception cref="XmlException">There is a load or parse error in the XML. In this case, the document remains empty.</exception>
        */
        public XmlDocument WriteXml(byte[] wbxml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(WriteXmlString(wbxml));
            return doc;
        }

        ~WbXmlParser()
        {
            WbXml2.wbxml_conv_wbxml2xml_destroy(_self);
        }
    }
}