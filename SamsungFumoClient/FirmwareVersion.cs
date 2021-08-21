namespace SamsungFumoClient
{
    public class FirmwareVersion
    {
        public FirmwareVersion(string applicationProcessor, string? coreProcessor, string? consumerSoftwareCustomization = null)
        {
            ApplicationProcessor = applicationProcessor;
            CoreProcessor = coreProcessor;
            ConsumerSoftwareCustomization = consumerSoftwareCustomization;
        }

        public string ApplicationProcessor { get; }
        public string? CoreProcessor { get; }
        public string? ConsumerSoftwareCustomization { get; }
    }
}