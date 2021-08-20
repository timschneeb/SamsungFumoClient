using System;
using System.Threading.Tasks;
using SamsungFumoClient.Network;
using SamsungFumoClient.Secure;
using SamsungFumoClient.SyncML;
using SamsungFumoClient.SyncML.Commands;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.Utils;

namespace SamsungFumoClient
{
    public class DmSession
    {
        private readonly OspHttpClient _client = new();

        private bool _isAlreadyRegistered = true;
        private SyncDocument? _lastResponse;
        private byte[] _serverNonce = Array.Empty<byte>();

        public DmSession(Device device, bool register = true,
            string serverUrl = "https://dms.ospserver.net/v1/device/magicsync/mdm",
            string serverId = "x6g1q14r75", string serverPassword = "T1NQIERNIFNlcnZlcg==")
        {
            IsAborted = false;
            Device = device;
            ServerId = serverId;
            ServerPassword = serverPassword;
            ServerUrl = serverUrl;
            ClientName = device.DeviceId;
            ClientPassword = CryptUtils.GenerateClientPassword(ClientName, serverId) ?? string.Empty;
            SessionId = DateTime.Now.Minute.ToString("X") + DateTime.Now.Second.ToString("X");

            _client.Device = device;
            _isAlreadyRegistered = register;
        }

        public string SessionId { get; }
        public int CurrentMessageId { private set; get; } = 1;
        public bool IsAborted { private set; get; }

        public Device Device { get; }
        public string ClientName { get; }
        public string ClientPassword { get; }
        public string ServerId { get; }
        public string ServerPassword { get; }
        public string ServerUrl { private set; get; }

        public async Task<SyncDocument> SendAsync(SyncBody body)
        {
            if (IsAborted)
            {
                throw new Exception("Refusing to send message. The server has already aborted the session.");
            }

            if (_isAlreadyRegistered)
            {
                _isAlreadyRegistered = false;
                await _client.SendFumoRegisterAsync(Device);
            }

            var syncMlWriter = new SyncMlWriter();
            syncMlWriter.BeginDocument();
            syncMlWriter.WriteSyncHdr(BuildHeader());
            syncMlWriter.WriteSyncBody(body);
            syncMlWriter.EndDocument();

            var responseBinary = await _client.SendWbxmlAsync(ServerUrl, syncMlWriter.GetBytes());

            SyncDocument responseDocument;
            responseDocument = new SyncMlParser(responseBinary).Parse();
            ProcessServerResponse(responseDocument);
            _lastResponse = responseDocument;

            return responseDocument;
        }

        public Cmd BuildAuthenticationStatus(int cmdId = 1)
        {
            return new Status
            {
                CmdID = cmdId,
                MsgRef = CurrentMessageId - 1,
                CmdRef = 0,
                Cmd = "SyncHdr",
                TargetRef = ClientName,
                SourceRef = ServerUrl.Contains('?')
                    ? ServerUrl.Substring(0, ServerUrl.LastIndexOf("?", StringComparison.Ordinal))
                    : ServerUrl,
                Data = "212"
            };
        }

        public SyncHdr BuildHeader()
        {
            Cred? cred = null;
            if (_lastResponse == null || !SyncMlUtils.IsAuthorizationAccepted(_lastResponse?.SyncBody?.Cmds))
            {
                cred = new Cred
                {
                    Meta = new Meta
                    {
                        Format = "b64",
                        Type = "syncml:auth-md5"
                    },
                    Data = GenerateAuthDigest()
                };
            }

            return new SyncHdr()
            {
                SessionID = SessionId,
                MsgID = CurrentMessageId,
                Target = new Target
                {
                    LocURI = ServerUrl
                },
                Source = new Source
                {
                    LocURI = ClientName,
                    LocName = ClientName
                },
                Cred = cred,
                Meta = new Meta
                {
                    MaxMsgSize = 5120,
                    MaxObjSize = 1048576
                }
            };
        }

        public void ProcessServerResponse(SyncDocument document)
        {
            // Find challenge section
            foreach (var cmd in document.SyncBody?.Cmds ?? Array.Empty<Cmd>())
            {
                if (cmd is Status status)
                {
                    if (status.Chal is {Meta: {NextNonce: { } nextNonce} meta})
                    {
                        if (meta.Type != "syncml:auth-md5" || meta.Format != "b64")
                        {
                            Log.W("DmSession: Challenge object uses an unsupported type or format");
                            continue;
                        }

                        _serverNonce = Base64.Decode(nextNonce);
                    }
                }
            }

            IsAborted = SyncMlUtils.HasServerAborted(document?.SyncBody?.Cmds);
            if (IsAborted)
            {
                Log.E("The server has aborted the session. No more messages must be sent.");
                return;
            }

            ServerUrl = document?.SyncHdr?.RespURI ?? ServerUrl;
            CurrentMessageId++;
        }

        private string? GenerateAuthDigest()
        {
            var nextNonce = NextNonce();
            return CryptUtils.MakeDigest(AuthTypes.Md5, ClientName, ClientPassword,
                nextNonce, null);
        }

        private byte[] NextNonce()
        {
            if (CurrentMessageId == 1 || _serverNonce.Length <= 0)
            {
                return Convert.FromBase64String(GenerateFactoryNonce());
            }

            return _serverNonce;
        }

        private static string GenerateFactoryNonce()
        {
            return Base64.Encode(RandomProvider.Random.Next() + "SSNextNonce");
        }
    }
}