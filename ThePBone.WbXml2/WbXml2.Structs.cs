using System;

#pragma warning disable 169

namespace ThePBone.WbXml2
{
    /** WBXML Versions (WBXML tokens) */
    public enum WBXMLVersion
    {
        /** Unknown WBXML Version */
        WBXML_VERSION_UNKNOWN = -1,

        /** WBXML 1.0 Token */
        WBXML_VERSION_10 = 0x00,

        /** WBXML 1.1 Token */
        WBXML_VERSION_11 = 0x01,

        /** WBXML 1.2 Token */
        WBXML_VERSION_12 = 0x02,

        /** WBXML 1.3 Token */
        WBXML_VERSION_13 = 0x03
    };

    /** Supported WBXML Languages */
    public enum WBXMLLanguage
    {
        /** Unknown / Not Specified */
        WBXML_LANG_UNKNOWN = 0,

        /* ---------- WAP */
        /** WML 1.0 */
        WBXML_LANG_WML10 = 1101,

        /** WML 1.1 */
        WBXML_LANG_WML11 = 1102,

        /** WML 1.2 */
        WBXML_LANG_WML12 = 1103,

        /** WML 1.3 */
        WBXML_LANG_WML13 = 1104,

        /** WTA 1.0 */
        WBXML_LANG_WTA10 = 1201,

        /** WTAWML 1.2 */
        WBXML_LANG_WTAWML12 = 1202,

        /** CHANNEL 1.1 */
        WBXML_LANG_CHANNEL11 = 1203,

        /** CHANNEL 1.2 */
        WBXML_LANG_CHANNEL12 = 1204,

        /** SI 1.0 */
        WBXML_LANG_SI10 = 1301,

        /** SL 1.0 */
        WBXML_LANG_SL10 = 1401,

        /** CO 1.0 */
        WBXML_LANG_CO10 = 1501,

        /** PROV 1.0 */
        WBXML_LANG_PROV10 = 1601,

        /** EMN 1.0 */
        WBXML_LANG_EMN10 = 1701,

        /** DRMREL 1.0 */
        WBXML_LANG_DRMREL10 = 1801,

        /* ---------- Ericsson / Nokia OTA Settings v7.0 */
        /** OTA Settings */
        WBXML_LANG_OTA_SETTINGS = 1901,

        /* ---------- SyncML */
        /** SYNCML 1.0 */
        WBXML_LANG_SYNCML_SYNCML10 = 2001,

        /** DEVINF 1.0 */
        WBXML_LANG_SYNCML_DEVINF10 = 2002,

        /** METINF 1.0 */
        WBXML_LANG_SYNCML_METINF10 = 2003,

        /** SYNCML 1.1 */
        WBXML_LANG_SYNCML_SYNCML11 = 2101,

        /** DEVINF 1.1 */
        WBXML_LANG_SYNCML_DEVINF11 = 2102,

        /** METINF 1.1 */
        WBXML_LANG_SYNCML_METINF11 = 2103,

        /** SYNCML 1.2 */
        WBXML_LANG_SYNCML_SYNCML12 = 2201,

        /** DEVINF 1.2 */
        WBXML_LANG_SYNCML_DEVINF12 = 2202,

        /** METINF 1.2 */
        WBXML_LANG_SYNCML_METINF12 = 2203,

        /** DMDDF  1.2 */
        WBXML_LANG_SYNCML_DMDDF12 = 2204,

        /* ---------- Wireless-Village */
        /** WV CSP 1.1 */
        WBXML_LANG_WV_CSP11 = 2301,

        /** WV CSP 1.2 */
        WBXML_LANG_WV_CSP12 = 2302,

        /* ---------- Microsoft AirSync */
        /** AirSync */
        WBXML_LANG_AIRSYNC = 2401,

        /** ActiveSync */
        WBXML_LANG_ACTIVESYNC = 2402,

        /* ---------- Nokia ConML */
        /** ConML */
        WBXML_LANG_CONML = 2501
    };

    /** Supported WBXML Charsets MIBEnum */
    public enum WBXMLCharsetMIBEnum
    {
        /** Unknown Charset */
        WBXML_CHARSET_UNKNOWN = 0,

        /** US-ASCII */
        WBXML_CHARSET_US_ASCII = 3,

        /** ISO-8859-1 */
        WBXML_CHARSET_ISO_8859_1 = 4,

        /** ISO-8859-2 */
        WBXML_CHARSET_ISO_8859_2 = 5,

        /** ISO-8859-3 */
        WBXML_CHARSET_ISO_8859_3 = 6,

        /** ISO-8859-4 */
        WBXML_CHARSET_ISO_8859_4 = 7,

        /** ISO-8859-5 */
        WBXML_CHARSET_ISO_8859_5 = 8,

        /** ISO-8859-6 */
        WBXML_CHARSET_ISO_8859_6 = 9,

        /** ISO-8859-7 */
        WBXML_CHARSET_ISO_8859_7 = 10,

        /** ISO-8859-8 */
        WBXML_CHARSET_ISO_8859_8 = 11,

        /** ISO-8859-8 */
        WBXML_CHARSET_ISO_8859_9 = 12,

        /** Shift_JIS */
        WBXML_CHARSET_SHIFT_JIS = 17,

        /** UTF-8 */
        WBXML_CHARSET_UTF_8 = 106,

        /** ISO-10646-UCS-2 */
        WBXML_CHARSET_ISO_10646_UCS_2 = 1000,

        /** UTF-16 */
        WBXML_CHARSET_UTF_16 = 1015,

        /** Big5 */
        WBXML_CHARSET_BIG5 = 2026
    };

    /**
     * @brief Type of XML Generation
     * @note Canonical Form is defined here: http://www.jclark.com/xml/canonxml.html
     */
    public enum WBXMLGenXMLType
    {
        /** Compact XML generation */
        WBXML_GEN_XML_COMPACT = 0,

        /** Indented XML generation */
        WBXML_GEN_XML_INDENT = 1,

        /** Canonical XML generation */
        WBXML_GEN_XML_CANONICAL = 2
    };

    public enum WBXMLError
    {
        /* ---------- Generic Errors */
        /** No Error */
        WBXML_OK = 0,

        /** Not an error; just a special internal return code */
        WBXML_NOT_ENCODED = 1,
        WBXML_ERROR_ATTR_TABLE_UNDEFINED = 10,
        WBXML_ERROR_BAD_DATETIME = 11,
        WBXML_ERROR_BAD_PARAMETER = 12,
        WBXML_ERROR_INTERNAL = 13,
        WBXML_ERROR_LANG_TABLE_UNDEFINED = 14,
        WBXML_ERROR_NOT_ENOUGH_MEMORY = 15,
        WBXML_ERROR_NOT_IMPLEMENTED = 16,
        WBXML_ERROR_TAG_TABLE_UNDEFINED = 17,
        WBXML_ERROR_B64_ENC = 18,
        WBXML_ERROR_B64_DEC = 19,
        WBXML_ERROR_WV_DATETIME_FORMAT = 20,
        WBXML_ERROR_NO_CHARSET_CONV = 30,
        WBXML_ERROR_CHARSET_STR_LEN = 31,
        WBXML_ERROR_CHARSET_UNKNOWN = 32,
        WBXML_ERROR_CHARSET_CONV_INIT = 33,
        WBXML_ERROR_CHARSET_CONV = 34,
        WBXML_ERROR_CHARSET_NOT_FOUND = 35,

        /* ---------- WBXML Parser Errors */
        WBXML_ERROR_ATTR_VALUE_TABLE_UNDEFINED = 40,
        WBXML_ERROR_BAD_LITERAL_INDEX = 41,
        WBXML_ERROR_BAD_NULL_TERMINATED_STRING_IN_STRING_TABLE = 42,
        WBXML_ERROR_BAD_OPAQUE_LENGTH = 43,
        WBXML_ERROR_EMPTY_WBXML = 44,
        WBXML_ERROR_END_OF_BUFFER = 45,
        WBXML_ERROR_ENTITY_CODE_OVERFLOW = 46,
        WBXML_ERROR_EXT_VALUE_TABLE_UNDEFINED = 47,
        WBXML_ERROR_INVALID_STRTBL_INDEX = 48,
        WBXML_ERROR_LITERAL_NOT_NULL_TERMINATED_IN_STRING_TABLE = 49,
        WBXML_ERROR_NOT_NULL_TERMINATED_INLINE_STRING = 50,
        WBXML_ERROR_NULL_PARSER = 51,
        WBXML_ERROR_NULL_STRING_TABLE = 52,
        WBXML_ERROR_STRING_EXPECTED = 53,
        WBXML_ERROR_STRTBL_LENGTH = 54,
        WBXML_ERROR_UNKNOWN_ATTR = 60,
        WBXML_ERROR_UNKNOWN_ATTR_VALUE = 61,
        WBXML_ERROR_UNKNOWN_EXTENSION_TOKEN = 62,
        WBXML_ERROR_UNKNOWN_EXTENSION_VALUE = 63,
        WBXML_ERROR_UNKNOWN_PUBLIC_ID = 64,
        WBXML_ERROR_UNKNOWN_TAG = 65,
        WBXML_ERROR_UNVALID_MBUINT32 = 70,
        WBXML_ERROR_WV_INTEGER_OVERFLOW = 80,

        /* ---------- WBXML Encoder Errors */
        WBXML_ERROR_ENCODER_APPEND_DATA = 90,
        WBXML_ERROR_STRTBL_DISABLED = 100,
        WBXML_ERROR_UNKNOWN_XML_LANGUAGE = 101,
        WBXML_ERROR_XML_NODE_NOT_ALLOWED = 102,
        WBXML_ERROR_XML_NULL_ATTR_NAME = 103,
        WBXML_ERROR_XML_PARSING_FAILED = 104,
        WBXML_ERROR_XML_DEVINF_CONV_FAILED = 110,
        WBXML_ERROR_NO_XMLPARSER = 120,
        WBXML_ERROR_XMLPARSER_OUTPUT_UTF16 = 121,
    };

    /**
     * @brief Parameters when generating an XML document
     */
    internal struct WBXMLConvWBXML2XML
    {
        WBXMLGenXMLType gen_type;

        /** WBXML_GEN_XML_COMPACT | WBXML_GEN_XML_INDENT | WBXML_GEN_XML_CANONICAL (Default: WBXML_GEN_XML_INDENT) */
        WBXMLLanguage lang;

        /** Force document Language (overwrite document Public ID) */
        WBXMLCharsetMIBEnum charset;

        /** Set document Language (does not overwrite document character set) */
        byte indent;

        /** Indentation Delta, when using WBXML_GEN_XML_INDENT Generation Type (Default: 0) */
        bool keep_ignorable_ws; /** Keep Ignorable Whitespaces (Default: FALSE) */
    };

    /**
     * @brief Parameters when generating a WBXML document
     */
    internal struct WBXMLConvXML2WBXML
    {
        WBXMLVersion wbxml_version;

        /** WBXML Version */
        bool keep_ignorable_ws;

        /** Keep Ignorable Whitespaces (Default: FALSE) */
        bool use_strtbl;

        /** Generate String Table (Default: TRUE) */
        bool produce_anonymous; /** Produce an anonymous document (Default: FALSE) */
    };


    /**
     * Obsolete - Downward binary compatibility only
     */
    [Obsolete]
    internal struct WBXMLGenXMLParams
    {
        WBXMLGenXMLType gen_type;

        /** WBXML_GEN_XML_COMPACT | WBXML_GEN_XML_INDENT | WBXML_GEN_XML_CANONICAL (Default: WBXML_GEN_XML_INDENT) */
        WBXMLLanguage lang;

        /** Force document Language (overwrite document Public ID) */
        WBXMLCharsetMIBEnum charset;

        /** Set document Language (does not overwrite document character set) */
        byte indent;

        /** Indentation Delta, when using WBXML_GEN_XML_INDENT Generation Type (Default: 0) */
        bool keep_ignorable_ws; /** Keep Ignorable Whitespaces (Default: FALSE) */
    };

    [Obsolete]
    internal struct WBXMLGenWBXMLParams
    {
        WBXMLVersion wbxml_version;

        /** WBXML Version */
        bool keep_ignorable_ws;

        /** Keep Ignorable Whitespaces (Default: FALSE) */
        bool use_strtbl;

        /** Generate String Table (Default: TRUE) */
        bool produce_anonymous; /** Produce an anonymous document (Default: FALSE) */
    };
}