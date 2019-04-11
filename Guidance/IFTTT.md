# Scenario 2: Sound&Light and relay
## What problem does Scenario 2 solve?
In this scenario, we will use three Grove modules to build a sound&light monitor which will trigger relay module when it detect the sound is too loud.

In this case, we using the Azure Function, when the sound is too loud it will trigger the Function, which control the relay to do something. In addition we also use the IFTTT Maker channel, post a trigger request to IFTTT then you can connect it to other IFTTT channel, such as Facebook, Twitter and etc.
## Hardware setup
Connecting `Grove - Sound Sensor` to GrovePi+'s A0 port, `Grove - Light Sensor` to GrovePi+'s A1 port and Grove - Relay to D5. And then power the Raspberry Pi with USB.
hardware list:
>1. Raspberry Pi 3
>2. GrovePi+
>3. Grove - Sound Sensor, Grove - Light Sensor and Grove - Relay
>4. 3 x Grove Cable

![sound-light-azure](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/sound-light-azure.png)
## Services
### Azure sevices
* Micrsoft Azure IoT Hub: Use to manage and monitor Grove module.
* Micrsoft Azure Functions: We can use Azure Fuction to post request for IFTTT webhook service, and trigger Grove - Relay module.
### Other services
* IFTTT Maker Channel: It's a webhook services, you need to sign up account and generate an unique URL by yourself
## Up and run
### set up
Creating IFTTT Maker Channel webhook

Go to [https://ifttt.com/services/maker_webhooks/settings](https://ifttt.com/services/maker_webhooks/settings) and then you will see this:
![ifttt-webhook-page](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/ifttt-webhook-page.png)
Click `Connect` Button, then get the link
![ifttt-webhook-finish](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/ifttt-webhook-finish.png)

1. Copy Event Hub's name
![event-hub-ifttt](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/event-hub-ifttt.png)
2. Create function for this case
![create-function-for-ifttt](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/create-function-for-ifttt.png)
![new-eventhub-trigger](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/new-eventhub-trigger.png)
![new-name](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/new-name.png)
![function-coding](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/function-coding.png)
![sound-light-relay-function-project-json](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/sound-light-relay-function-project-json.png)
![sound-light-relay-function-succeeded](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/sound-light-relay-function-succeeded.png)
3. Test function(please copy code section to function)
![sound-light-ifttt](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/sound-light-ifttt.png)
### code
```
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using RestSharp;
using System;

// Get ifttt maker profile, refer to: https://ifttt.com/services/maker_webhooks/settings
// Copy URL of Account Info to browser, then set the event name.
const String maker_service_key = "YOUR-MAKER-SERVICE-KEY";
const string trigger_event_name = "YOUR-MARKER-EVENT-NAME";

const string IOTHUB_CONNECT_STRING = "YOUR_IOTHUB_CONNECT_STRING";

const int BIG_SOUND = 150;

public static void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    GroveMessage m = JsonConvert.DeserializeObject<GroveMessage>(myEventHubMessage);
    if (m.Sound > BIG_SOUND)
    {
        TriggerIFTTTMaker(m.Sound);
        ControlRelayAsync(m.DeviceId, true).Wait();
    } else
    {
        ControlRelayAsync(m.DeviceId, false).Wait();
    }
}

public static void TriggerIFTTTMaker(int sound)
{
    var client = new RestClient(string.Format("https://maker.ifttt.com/trigger/{0}/with/key/{1}", trigger_event_name, maker_service_key));
    var request = new RestRequest(Method.POST);
    request.AddHeader("cache-control", "no-cache");
    request.AddHeader("content-type", "application/json");
    var body = new
    {
        value1 = sound,
        value2 = "",
        value3 = ""
    };
    request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
    IRestResponse response = client.Execute(request);
}

public static async Task ControlRelayAsync(String deviceId, bool onOff)
{
    var methodInvocation = new CloudToDeviceMethod("ControlRelay") { ResponseTimeout = TimeSpan.FromSeconds(30) };
    var message = new { onoff = onOff };
    string messageString = JsonConvert.SerializeObject(message);
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

```
## Outcome
Nothing.