namespace SamsungFumoClient.SyncML.Elements
{
    public interface IXmlElement
    {
        public void Write(SyncMlWriter writer);
        public IXmlElement Parse(SyncMlParser parser, object? param = null);
    }
}