using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace ThePBone.WbXml2
{
    internal static unsafe class WbXml2
    {
        private const string LibraryName = "libwbxml2";

        /**
         * @brief Convert WBXML to XML
         * @param wbxml     [in] WBXML Document to convert
         * @param wbxml_len [in] Length of WBXML Document
         * @param xml       [out] Resulting XML Document
         * @param xml_len   [out] XML Document length
         * @param param    [in] Parameters (if NULL, default values are used)
         * @return WBXML_OK if conversion succeeded, an Error Code otherwise
         */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        [Obsolete]
        internal static extern WBXMLError wbxml_conv_wbxml2xml_withlen(
            byte* wbxml,
            ulong wbxml_len,
            byte** xml,
            ulong* xml_len,
            WBXMLGenXMLParams* param);

        /**
          * @brief Convert XML to WBXML
          * @param xml       [in] XML Document to convert
          * @param xml_len   [in] Length of XML Document
          * @param wbxml     [out] Resulting WBXML Document
          * @param wbxml_len [out] Length of resulting WBXML Document
          * @param params    [in] Parameters (if NULL, default values are used)
          * @return WBXML_OK if conversion succeeded, an Error Code otherwise
          */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        [Obsolete]
        internal static extern WBXMLError wbxml_conv_xml2wbxml_withlen(
            byte* xml,
            ulong xml_len,
            byte** wbxml,
            ulong* wbxml_len,
            WBXMLGenWBXMLParams* param);

        /**
          * @brief Create a new WBXML to XML converter with the default configuration.
          * @param conv [out] a reference to the pointer of the new converter
          * @return WBXML_OK if conversion succeeded, an Error Code otherwise
          */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern WBXMLError wbxml_conv_wbxml2xml_create(WBXMLConvWBXML2XML** conv);

        /**
        * @brief Set the XML generation type (default: WBXML_GEN_XML_INDENT).
        * @param conv     [in] the converter
        * @param gen_type [in] generation type
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_wbxml2xml_set_gen_type(WBXMLConvWBXML2XML* conv,
            WBXMLGenXMLType gen_type);

        /**
        * @brief Set the used WBXML language.
        *        The language is usually detected by the specified public ID in the document.
        *        If the public ID is set then it overrides the language.
        * @param conv [in] the converter
        * @param lang [in] language (e.g. SYNCML12)
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_wbxml2xml_set_language(WBXMLConvWBXML2XML* conv, WBXMLLanguage lang);

        /**
        * @brief Set the used character set.
        *        The default character set is UTF-8.
        *        If the document specifies a character set by it own
        *        then this character set overrides the parameter charset.
        * @param conv    [in] the converter
        * @param charset [in] the character set
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_wbxml2xml_set_charset(WBXMLConvWBXML2XML* conv,
            WBXMLCharsetMIBEnum charset);

        /**
        * @brief Set the indent of the generated XML document (please see EXPAT default).
        * @param conv   [in] the converter
        * @param indent [in] the number of blanks
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_wbxml2xml_set_indent(WBXMLConvWBXML2XML* conv, byte indent);

        /**
        * @brief Enable whitespace preservation (default: FALSE).
        * @param conv     [in] the converter
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_wbxml2xml_enable_preserve_whitespaces(WBXMLConvWBXML2XML* conv);

        /**
        * @brief Convert WBXML to XML
        * @param conv      [in] the converter
        * @param wbxml     [in] WBXML Document to convert
        * @param wbxml_len [in] Length of WBXML Document
        * @param xml       [out] Resulting XML Document
        * @param xml_len   [out] XML Document length
        * @return WBXML_OK if conversion succeeded, an Error Code otherwise
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern WBXMLError wbxml_conv_wbxml2xml_run(WBXMLConvWBXML2XML* conv,
            byte* xml,
            ulong xml_len,
            byte** wbxml,
            ulong* wbxml_len);

        /**
        * @brief Destroy the converter object.
        * @param [in] the converter
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_wbxml2xml_destroy(WBXMLConvWBXML2XML* conv);


        /**
        * @brief Create a new WBXML to XML converter with the default configuration.
        * @param conv [out] a reference to the pointer of the new converter
        * @return WBXML_OK if conversion succeeded, an Error Code otherwise
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern WBXMLError wbxml_conv_xml2wbxml_create(WBXMLConvXML2WBXML** conv);

        /**
        * @brief Set the WBXML version (default: 1.3).
        * @param conv   [in] the converter
        * @param indent [in] the number of blanks
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_xml2wbxml_set_version(WBXMLConvXML2WBXML* conv,
            WBXMLVersion wbxml_version);

        /**
        * @brief Enable whitespace preservation (default: FALSE/DISABLED).
        * @param conv     [in] the converter
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_xml2wbxml_enable_preserve_whitespaces(WBXMLConvXML2WBXML* conv);

        /**
        * @brief Disable string table (default: TRUE/ENABLED).
        * @param conv     [in] the converter
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_xml2wbxml_disable_string_table(WBXMLConvXML2WBXML* conv);

        /**
        * @description: Disable public ID (default: TRUE/ENABLED).
        *              Usually you don't want to produce WBXML documents which are
        *              really anonymous. You want a known public ID or a DTD name
        *              to determine the document type. Some specifications like
        *              Microsoft's ActiveSync explicitly require fully anonymous
        *              WBXML documents. If you need this then you must disable
        *              the public ID mechanism.
        * @param conv     [in] the converter
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_xml2wbxml_disable_public_id(WBXMLConvXML2WBXML* conv);

        /**
        * @brief Convert XML to WBXML
        * @param conv      [in] the converter
        * @param xml       [in] XML Document to convert
        * @param xml_len   [in] Length of XML Document
        * @param wbxml     [out] Resulting WBXML Document
        * @param wbxml_len [out] Length of resulting WBXML Document
        * @return WBXML_OK if conversion succeeded, an Error Code otherwise
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern WBXMLError wbxml_conv_xml2wbxml_run(WBXMLConvXML2WBXML* conv,
            byte* xml,
            ulong xml_len,
            byte** wbxml,
            ulong* wbxml_len);

        /**
        * @brief Destroy the converter object.
        * @param [in] the converter
        */
        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        internal static extern void wbxml_conv_xml2wbxml_destroy(WBXMLConvXML2WBXML* conv);
    }
}