using System;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Text;

const String IOTHUB_CONNECT_STRING = "HostName=GroveKitIotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=szPdDDGjl5cxPbkJhuqty16K7PiQtDt3ON2Xi7Pfofk=";
const Int DANGER_VALUE = 100;

public static void Run(string myEventHubMessage, TraceWriter log)
{
    // log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    GroveMessage m = JsonConvert.DeserializeObject<GroveMessage>(myEventHubMessage);
    string name = m.GasSO;
    if (m.GasS0 > DANGER_VALUE) {
        ControlMotor("GroveKitDevice", true).Wait();
    } else {
        ControlMotor("GroveKitDevice", false).Wait();
    }
}

public static async Task ControlMotor(String deviceId, Boolean onoff)
{
    var methodInvocation = new CloudToDeviceMethod("ControlMotor") { ResponseTimeout = TimeSpan.FromSeconds(30) };
    var messag = new { onoff = onoff};
    string messageString = JsonConvert.SerializeObject(messag);
    methodInvocation.SetPayloadJson(messageString);
    ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOTHUB_CONNECT_STRING);
    var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);
}

class GroveMessage
{
    public string Hum { get; set; }
    public string Temp { get; set; }
    public string Sound { get; set; }
    public string Light { get; set; }
    public string GasSO { get; set; }
    public string PIR { get; set; }
    public string Timestamp { get; set; }
}
