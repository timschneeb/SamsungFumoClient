using SamsungFumoClient.SyncML;
using SamsungFumoClient.SyncML.Elements;

namespace SamsungFumoClient.Utils
{
    public static class SyncDocGenerator
    {
        public static SyncHdr BuildReqHeader(string sessionId, int msgId, string deviceId, string targetUri,
            string authToken)
        {
            return new()
            {
                SessionID = sessionId,
                MsgID = msgId,
                Target = new Target
                {
                    LocURI = targetUri
                },
                Source = new Source
                {
                    LocURI = deviceId,
                    LocName = deviceId
                },
                Cred = new Cred
                {
                    Meta = new Meta
                    {
                        Format = "b64",
                        Type = "syncml:auth-md5"
                    },
                    Data = authToken
                },
                Meta = new Meta
                {
                    MaxMsgSize = 5120,
                    MaxObjSize = 1048576
                }
            };
        }
    }
}