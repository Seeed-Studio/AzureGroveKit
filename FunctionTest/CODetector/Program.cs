using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TestLib;

namespace CODetector
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(TestData.GetTestDataString(), new TraceWriter());
        }

        const string IOTHUB_CONNECT_STRING = "HostName=GroveKitIotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=szPdDDGjl5cxPbkJhuqty16K7PiQtDt3ON2Xi7Pfofk=";
        const int DANGER_VALUE = 100;

        public static void Run(string myEventHubMessage, TraceWriter log)
        {
            log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
            GroveMessage m = JsonConvert.DeserializeObject<GroveMessage>(myEventHubMessage);
            if (m.GasSO < DANGER_VALUE)
            {
                ControlMotor("GroveKitDevice", true).Wait();
            }
            else
            {
                ControlMotor("GroveKitDevice", false).Wait();
            }
        }

        public static async Task ControlMotor(String deviceId, Boolean onoff)
        {
            var methodInvocation = new CloudToDeviceMethod("ControlMotor") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            var messag = new { onoff = onoff };
            string messageString = JsonConvert.SerializeObject(messag);
            methodInvocation.SetPayloadJson(messageString);
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOTHUB_CONNECT_STRING);
            var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);
        }

        private class GroveMessage
        {
            public string Hum { get; set; }
            public string Temp { get; set; }
            public string Sound { get; set; }
            public string Light { get; set; }
            public int GasSO { get; set; }
            public string PIR { get; set; }
            public string Timestamp { get; set; }
        }

    }
}
