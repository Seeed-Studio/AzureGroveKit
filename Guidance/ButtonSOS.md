# Scenario 4: One-Click SOS
## What problem does Scenario 4 solve?
In this scenario, we will use `Grove – Button` to develop a project SOS event. In this case, we are check the value of the button sensor, and then call for help when it is pressed. 

In this case, we using the Azure Function, when the button is pressed it will trigger the Microsoft Azure Function, and use twilio send sos message.
>* Notice:Here we only provide a way to build the application. The sensor data can already be obtained from the iot-hub, and users should build the application according to their actual situation, instead of simply using the Demo.
## Hardware setup
Connecting `Grove – Button` to GrovePi+'s D4 port, and then power the Raspberry Pi with USB.
hardware list:
>1. Raspberry Pi 3
>2. GrovePi+
>3. Grove – Button
>4. 1 x Grove Cable
## Services
### Azure services
* Micrsoft Azure IoT Hub: Use to manage and monitor Grove module.
* Micrsoft Azure Functions: We can use Azure Fuction to send an alert email via Twilio service.
## Up and run
### set up
1. create function app on `All resources --> New --> Compute --> Function App` Refer to: [https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function)
![create-temp-humidity-trigger-function](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/create-temp-humidity-function-app.png)
2. Copy IoTHub campatible EventHub endpoint name. Move on `Your_IoTHub --> GroveKitIotHub --> Endpoints --> Built-in endpoints`, Click `messages/events`, copy value of `Event Hub-compatible name`. And add another Consumer groups, named `function`.
3. Copy `code/run.csx` to Azrue portal, and modify `Account SID` , `YOUR_TWILIO_NUMBER` and `YOUR_FAMILY_NUMBER` on the `run.csx`. On the right side to add file named `project.json`, copy `code/project.json`.

4. Now, You can run AzureGroveKit UWP app to test function.
### code/run.csx
```
using System;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account; 
using Twilio.Types;

// Your Account SID from twilio.com/console
const string accountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
// Your Auth Token from twilio.com/console
const string authToken = "auth_token"; 
const string YOUR_TWILIO_NUMBER = "";
const string YOUR_FAMILY_NUMBER = "";

public static void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    ButtonEvent m = JsonConvert.DeserializeObject<ButtonEvent>(myEventHubMessage);
    if (m.Click)
    {
        SendSOS("Please help me. From Azure button.");
    }
}

public static void SendSOS(String msg)
{
    TwilioClient.Init(accountSid, authToken);
    var message = MessageResource.Create(
        to: new PhoneNumber(YOUR_FAMILY_NUMBER),
        from: new PhoneNumber(YOUR_TWILIO_NUMBER),
        body: msg);
}

internal class ButtonEvent
{
    public string DeviceId { get; set; }
    public bool Click { get; set; }
    public string Timestamp { get; set; }
}
```
### code/project.json
```
{
"frameworks": {
"net46": {
"dependencies": {
    "Microsoft.AspNet.WebApi.Client": "5.2.3",
    "Microsoft.AspNet.WebApi.Core": "5.2.3",
    "Newtonsoft.Json": "10.0.2",
    "Twilio": "5.5.1"
}
}
}
}
```
## Reference
Twilio binding for Azure Functions:[https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-twilio](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-twilio)