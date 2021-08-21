using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SamsungFumoClient.SyncML;
using SamsungFumoClient.SyncML.Commands;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;

namespace SamsungFumoClient.Utils
{
    public static class SyncMlUtils
    {
        public static bool IsAuthorizationAccepted(Cmd[]? cmds)
        {
            if (cmds is {Length: > 0})
            {
                if (cmds.Any(cmd => cmd is Status {Cmd: "SyncHdr", Data: "212" or "200"}))
                {
                    return true;
                }
            }
            else
            {
                Log.W("SyncMlUtils.IsAuthorizationAccepted: Unknown state: No commands answered by server");
            }

            return false;
        }

        public static bool HasCommand<T>(Cmd[]? cmds)
        {
            return cmds is { } && cmds.Any(cmd => cmd is T);
        }

        public static bool HasServerAborted(Cmd[]? cmds)
        {
            return cmds is { } && cmds.Any(cmd => cmd is Alert {Data: AlertTypes.SESSION_ABORT});
        }

        public static Cmd[] BuildGetResults(SyncDocument? doc, (string path, string value)[] dataSource,
            int cmdIdStart = 1)
        {
            if (doc?.SyncHdr == null)
            {
                Log.E("SyncMlUtils.BuildGetResponse: Null header");
                throw new ArgumentNullException();
            }

            var cmds = doc.SyncBody?.Cmds;
            if (!HasCommand<Get>(cmds) || cmds == null)
            {
                Log.W("SyncMlUtils.BuildGetResponse: Cannot build GET response. No GET commands supplied.");
                return new Cmd[0];
            }

            var newCmds = new List<Cmd>();
            var currentNewCmdId = cmdIdStart;
            foreach (var cmd in cmds)
            {
                if (cmd is not Get)
                {
                    continue;
                }

                if ((cmd.Item?.Length ?? 0) != 1)
                {
                    Log.W(
                        $"SyncMlUtils.BuildGetResponse: GET request ({cmd.CmdID}) contained {cmd.Item?.Length ?? 0} instead of 1 target");
                    continue;
                }

                var locUri = cmd.Item?[0].Target?.LocURI;
                if (locUri == null)
                {
                    Log.W($"SyncMlUtils.BuildGetResponse: GET request ({cmd.CmdID}) contained an item without a target");
                    continue;
                }

                string? valueResult = null;
                foreach (var (path, value) in dataSource)
                {
                    if (path == locUri)
                    {
                        valueResult = value;
                        break;
                    }
                }

                if (valueResult == null)
                {
                    Log.W($"SyncMlUtils.BuildGetResponse: Unable to find value for path '{locUri}' (GET request {cmd.CmdID})");
                    continue;
                }

                var status = new Status
                {
                    CmdRef = cmd.CmdID,
                    MsgRef = doc.SyncHdr.MsgID,
                    CmdID = currentNewCmdId++,
                    Cmd = "Get",
                    TargetRef = locUri,
                    Data = "200"
                };

                var result = new Results
                {
                    CmdRef = cmd.CmdID,
                    MsgRef = doc.SyncHdr.MsgID,
                    CmdID = currentNewCmdId++,
                    Item = new Item[]
                    {
                        new()
                        {
                            Source = new Source
                            {
                                LocURI = locUri
                            },
                            Meta = new Meta
                            {
                                Format = "chr",
                                Type = "text/plain",
                                Size = valueResult.Length
                            },
                            Data = new PcData
                            {
                                Data = valueResult
                            }
                        }
                    }
                };

                newCmds.Add(status);
                newCmds.Add(result);
            }

            return newCmds.ToArray();
        }

        public static Item[] BuildItemList(IEnumerable<(string path, string value)> dataSource)
        {
            var itemList = new List<Item>();
            foreach (var (path, value) in dataSource)
            {
                itemList.Add(new Item
                {
                    Source = new Source
                    {
                        LocURI = path
                    },
                    Data = new PcData
                    {
                        Data = value
                    }
                });
            }

            return itemList.ToArray();
        }

        public static Item? FindItemByTargetUri<T>(SyncDocument? doc, string targetUri)
        {
            var cmds = doc?.SyncBody?.Cmds;
            if (!HasCommand<T>(cmds) || cmds == null)
            {
                return null;
            }

            foreach (var cmd in cmds)
            {
                if ((cmd.Item?.Length ?? 0) <= 0)
                {
                    continue;
                }

                foreach (var item in cmd.Item ?? Array.Empty<Item>())
                {
                    var locUri = item.Target?.LocURI;
                    if (locUri == null)
                    {
                        continue;
                    }

                    if (locUri == targetUri)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
        
        public static string? FindFirmwareUpdateUri(SyncDocument? doc)
        {
            var uri = Regex.Replace(
                FindItemByTargetUri<Replace>(doc, "./FUMO/DownloadAndUpdate/PkgURL")
                    ?.Data?.Data ?? string.Empty, @"[\s-[\p{Zs}\t]]+", "");

            if (string.IsNullOrWhiteSpace(uri) && Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
            {
                return null;
            }

            return uri;
        }


        public static Cmd[] BuildSuccessResponses(SyncDocument? doc,
            (string cmdType, string code)[]? responseMap = null, int cmdIdStart = 1)
        {
            if (doc?.SyncHdr == null)
            {
                Log.E("SyncMlUtils.BuildSuccessResponses: Null header");
                throw new ArgumentNullException();
            }

            var cmds = doc.SyncBody?.Cmds;
            if (!HasCommand<Get>(cmds) || cmds == null)
            {
                return new Cmd[0];
            }

            var newCmds = new List<Cmd>();
            var currentNewCmdId = cmdIdStart;
            foreach (var cmd in cmds)
            {
                if (cmd is Status {Cmd: "SyncHdr"})
                {
                    continue;
                }

                if ((cmd.Item?.Length ?? 0) != 1)
                {
                    Log.W(
                        $"SyncMlUtils.BuildSuccessResponses: Request ({cmd.CmdID}) contained {cmd.Item?.Length ?? 0} instead of 1 target");
                    continue;
                }

                var cmdItem = cmd.Item?[0];

                var locUri = cmdItem?.Target?.LocURI;
                if (locUri == null)
                {
                    Log.W($"SyncMlUtils.BuildSuccessResponses: GET request ({cmd.CmdID}) contained an item without a target");
                    continue;
                }

                var responseCode = "200";
                foreach (var (cmdType, code) in responseMap ?? new (string, string)[0])
                {
                    if (cmdType == cmd.GetType().Name)
                    {
                        responseCode = code;
                        break;
                    }
                }

                var status = new Status
                {
                    CmdRef = cmd.CmdID,
                    MsgRef = doc.SyncHdr.MsgID,
                    CmdID = currentNewCmdId++,
                    Cmd = cmd.GetType().Name,
                    TargetRef = locUri,
                    Data = responseCode
                };

                newCmds.Add(status);
            }

            return newCmds.ToArray();
        }


        public static int ExtractSvcState(Cmd[]? cmds)
        {
            if (cmds == null)
            {
                return -1;
            }

            foreach (var cmd in cmds)
            {
                if (cmd is Replace replace)
                {
                    var matches = replace.Item?.Where(x => x.Target?.LocURI == "./FUMO/Ext/SvcState");
                    var enumerable = matches as Item[] ?? matches?.ToArray();
                    if (enumerable?.Length > 0)
                    {
                        return int.Parse(enumerable.First().Data?.Data ?? "-1");
                    }
                }
            }

            return -1;
        }
    }
}