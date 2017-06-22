using Newtonsoft.Json;
using System;
using Microsoft.Azure.Devices;

const string IOTHUB_CONNECT_STRING = "YOUR_IOTHUB_CONNECT_STRING";
const int DANGER_VALUE = 50;

public static void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    GroveMessage m = JsonConvert.DeserializeObject<GroveMessage>(myEventHubMessage);
    if (m.GasSO < DANGER_VALUE)
    {
        ControlMotor(m.DeviceId, true).Wait();
    }
    else
    {
        ControlMotor(m.DeviceId, false).Wait();
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
    public string DeviceId { get; set; }
    public double Hum { get; set; }
    public double Temp { get; set; }
    public int Sound { get; set; }
    public int Light { get; set; }
    public int GasSO { get; set; }
    public bool PIR { get; set; }
    public string Timestamp { get; set; }
}