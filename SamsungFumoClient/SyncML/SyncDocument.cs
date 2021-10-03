using SamsungFumoClient.SyncML.Elements;

namespace SamsungFumoClient.SyncML
{
    public class SyncDocument
    {
        public SyncDocument(SyncHdr? syncHdr = null, SyncBody? syncBody = null)
        {
            SyncHdr = syncHdr;
            SyncBody = syncBody;
        }

        public SyncHdr? SyncHdr { set; get; }
        public SyncBody? SyncBody { set; get; }
    }
}