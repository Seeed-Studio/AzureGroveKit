namespace AzureGroveKit
{
    class GroveMessage
    {
        public string DeviceId { get; set; }
        public double Hum { get; set; }
        public double Temp { get; set; }
        public int Sound { get; set; }
        public int Light { get; set; }
        public int GasSO { get; set; }
        public bool PIR { get; set; }
        public string Timestamp { get; set; }
    }

    class ButtonEvent
    {
        public string DeviceId { get; set; }
        public bool Click { get; set; }
    }
}
