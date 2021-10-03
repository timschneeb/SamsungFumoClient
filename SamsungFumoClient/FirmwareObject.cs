
namespace SamsungFumoClient
{
    public class FirmwareObject
    {
        public FirmwareObject(){}
        
        public string Description { init; get; } = null!;
        public string Uri { init; get; } = null!;
        public int Size { init; get; }
        public FirmwareVersion Version { init; get; } = null!;
        public string? Md5 { init; get; }
        public string? SecurityPatchVersion { init; get; }
    }
}