using System;
using SamsungFumoClient.SyncML.Commands;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient.SyncML.Elements
{
    public class SyncBody : IXmlElement
    {
        public Cmd[]? Cmds { set; get; }

        public Meta? Meta { set; get; }
        public bool IsFinal { set; get; } = true;

        public void Write(SyncMlWriter writer)
        {
            AssertUtils.AreRequiredPropsNonNull(this);

            writer.WriteStartElement(WbxmlElement.SYNCBODY);
            {
                if (Cmds != null)
                {
                    foreach (var cmd in Cmds)
                    {
                        cmd.Write(writer);
                    }
                }

                if (IsFinal)
                {
                    writer.WriteSelfClosingElement(WbxmlElement.FINAL);
                }
            }
            writer.WriteEndElement();
        }

        public IXmlElement Parse(SyncMlParser parser, object? param = null)
        {
            var checkElementResult = parser.ParseCheckElement(WbxmlElement.SYNCBODY);
            if (checkElementResult != 0)
            {
                throw new SyncMlParseException(checkElementResult);
            }

            var error = parser.ZeroBitTagCheck();
            if (error == WbxmlError.ZEROBIT_TAG)
            {
                return this;
            }

            if (error != WbxmlError.ERR_OK)
            {
                throw new SyncMlParseException(error);
            }

            do
            {
                var element = parser.CurrentElement();

                switch (element)
                {
                    case WbxmlElement.END:
                        parser.ParseReadElement();
                        return this;
                    case WbxmlElement.CODEPAGE:
                        parser.ParseReadElement();
                        element = parser.ParseReadElement();
                        parser.CurrentCodePage = (WbxmlCodepage) element;
                        continue;
                    case WbxmlElement.ADD:
                        Log.E("Parser: Add not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.ALERT:
                        try
                        {
                            Cmds ??= Array.Empty<Cmd>();
                            Cmds = Cmds.ArrayPush(new Alert().Parse(parser));
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.ATOMIC:
                        Log.E("Parser: Atomic not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.COPY:
                        Log.E("Parser: Copy not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.DELETE:
                        Log.E("Parser: Delete not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.EXEC:
                        try
                        {
                            Cmds ??= Array.Empty<Cmd>();
                            Cmds = Cmds.ArrayPush(new Exec().Parse(parser));
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.FINAL:
                        parser.ParseBlankElement(element);
                        continue;
                    case WbxmlElement.GET:
                        try
                        {
                            Cmds ??= Array.Empty<Cmd>();
                            Cmds = Cmds.ArrayPush(new Get().Parse(parser));
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.MAP:
                        Log.E("Parser: Map not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.PUT:
                        Log.E("Parser: Put not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.REPLACE:
                        try
                        {
                            Cmds ??= Array.Empty<Cmd>();
                            Cmds = Cmds.ArrayPush(new Replace().Parse(parser));
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.RESULTS:
                        try
                        {
                            Cmds ??= Array.Empty<Cmd>();
                            Cmds = Cmds.ArrayPush(new Replace().Parse(parser));
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.SEQUENCE:
                        Log.E("Parser: Sequence not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    case WbxmlElement.STATUS:
                        try
                        {
                            Cmds ??= Array.Empty<Cmd>();
                            Cmds = Cmds.ArrayPush(new Status().Parse(parser));
                        }
                        catch (SyncMlParseException ex)
                        {
                            error = ex.WbXmlError;
                        }

                        continue;
                    case WbxmlElement.SYNC:
                        Log.E("Parser: Sync not implemented");
                        throw new SyncMlParseException(WbxmlError.NOT_IMPLEMENTED);
                    default:
                        error = WbxmlError.UNKNOWN;
                        continue;
                }
            } while (error == 0);

            throw new SyncMlParseException(error);
        }
    }
}