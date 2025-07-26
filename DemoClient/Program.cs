using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SamsungFumoClient;
using SamsungFumoClient.Exceptions;
using SamsungFumoClient.SyncML;
using SamsungFumoClient.SyncML.Commands;
using SamsungFumoClient.SyncML.Elements;
using SamsungFumoClient.SyncML.Enum;
using SamsungFumoClient.Utils;

namespace DemoClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var device = new Device
            {
                Model = "SM-R510",
                CustomerCode = "KOO",
            };

            var session = new DmSession(device);

            var body = new SyncBody
            {
                Cmds = new Cmd[]
                {
                    new Alert
                    {
                        CmdID = 1,
                        Data = AlertTypes.CLIENT_INITIATED_MGMT /* Client-initiated session */
                    },
                    new Replace
                    {
                        CmdID = 2,
                        Item = SyncMlUtils.BuildItemList(session.Device.AsDevInfNodes()) /* Upload device info */
                    },
                    new Alert
                    {
                        CmdID = 3,
                        Data = AlertTypes.GENERIC,
                        Item = new[] /* Submit FUMO service request */
                        {
                            new Item
                            {
                                Source = new Source
                                {
                                    LocURI = "./FUMO/DownloadAndUpdate"
                                },
                                Meta = new Meta
                                {
                                    Format = "chr",
                                    Type = "org.openmobilealliance.dm.firmwareupdate.devicerequest"
                                },
                                Data = new PcData
                                {
                                    Data = "0"
                                }
                            }
                        }
                    }
                }
            };

            // Initiate authentication handshakes by solving challenge objects
            SyncDocument? initialRequest = null;
            var attempt = 0;
            const int maxAttempt = 5;
            do
            {
                if (attempt >= maxAttempt)
                {
                    Log.E($"Giving up to authenticate after failed {maxAttempt} challenge transactions");
                    return;
                }

                attempt++;
                Log.I($"Authenticating with server ({attempt}/{maxAttempt} transactions)");
                try
                {
                    initialRequest = await session.SendAsync(body);
                }
                catch (HttpException ex)
                {
                    Log.I($"Failed to authenticate. Server returned error {ex}");
                }

                if (session.IsAborted)
                {
                    Log.E("Server has aborted the connection. Closing session.");
                    return;
                }
            } while (initialRequest == null || !SyncMlUtils.IsAuthorizationAccepted(initialRequest.SyncBody?.Cmds));

            // Check whether server is requested next information
            if (!SyncMlUtils.HasCommand<Get>(initialRequest?.SyncBody?.Cmds))
            {
                Log.E("Server is unexpectedly not requesting additional device information");
                return;
            }

            // Process and respond to GET request
            var dataSource = ArrayUtils.ConcatArray(
                session.Device.AsDevInfNodes(),
                await session.Device.AsDevDetailNodes()
            );
            var getResultCmds = SyncMlUtils.BuildGetResults(initialRequest, dataSource, 2);

            var fullResultCmds = ArrayUtils.ConcatArray(
                new[] {session.BuildAuthenticationStatus()},
                getResultCmds
            );
            var firmwareUpdateResult = await session.SendAsync(new SyncBody
            {
                Cmds = fullResultCmds
            });
            if (session.IsAborted)
            {
                Log.E("Server has aborted the connection. Closing session.");
                return;
            }

            // Check for SvcState error code in response
            var svcState = SyncMlUtils.ExtractSvcState(firmwareUpdateResult.SyncBody?.Cmds);
            switch (svcState)
            {
                case 260:
                    Log.E(
                        "SvcState = 260: No update for your device configuration detected. Please change the current firmware version parameter.");
                    return;
                case 220:
                    Log.E("SvcState = 220: No suitable firmware version found. Please check the supplied parameters.");
                    return;
                case >= 0:
                    Log.E($"Firmware request failed: ./FUMO/Ext/SvcState is set to {svcState.ToString()}");
                    break;
            }
            
            // Acknowledge message and end transaction
            await session.SendAsync(new SyncBody
            {
                Cmds = SyncMlUtils.BuildSuccessResponses(firmwareUpdateResult, new (string cmdType, string code)[]
                {
                    ("Exec", "202")
                })
            });
            
            // Extract firmware download URI from response if found
            var descriptorUri = SyncMlUtils.FindFirmwareUpdateUri(firmwareUpdateResult);
            if (descriptorUri == null)
            {
                Log.E("The server did not include a download descriptor uri with its response. Cannot continue.");
                return;
            }
            var firmware = await session.RetrieveFirmwareObjectAsync(descriptorUri);
            if (firmware == null)
            {
                Log.E("Download descriptor cannot be downloaded or is unreadable. Cannot continue.");
                return;
            }
            Log.I($"=======> Firmware found: {firmware.Version?.ApplicationProcessor}");

            await session.AbortSessionAsync();
        }
    }
}
