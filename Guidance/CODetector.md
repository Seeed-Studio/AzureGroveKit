# Scenario 3: GAS monitor
## What problem does Scenario 3 solve?
In this scenario, we will use one Grove module to build a Gas monitor which will trigger relay module when it detect the CO‘s value exceeded.In this case, we using the Azure Function, when the CO‘s value exceeded it will trigger the Microsoft Azure Function, as well as opening mini fan.

>* Notice:Here we only provide a way to build the application. The sensor data can already be obtained from the iot-hub, and users should build the application according to their actual situation, instead of simply using the Demo.
## Hardware setup
Connecting `Grove - Gas Sensor` to GrovePi+'s A2 port, `Grove - Mini I2C Motor Driver` to GrovePi+'s I2C2 port. And then power the Raspberry Pi with USB.
hardware list:
>1. Raspberry Pi 3
>2. GrovePi+
>3. Grove - Gas Sensor, Grove - Mini I2C Motor Driver and mini fan
>4. 2 x Grove Cable

## Services
### Azure sevices
* Micrsoft Azure IoT Hub: Use to manage and monitor Grove module.
* Micrsoft Azure Functions:We can use fuction to monitor gas data, and turn on mini fan. 

## Up and run
### set up
1. create function app on `All resources --> New --> Compute --> Function App` Refer to: [https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function)
![create-temp-humidity-trigger-function](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/create-temp-humidity-function-app.png)
2. Copy IoTHub campatible EventHub endpoint name. Move on `Your_IoTHub --> GroveKitIotHub --> Endpoints --> Built-in endpoints`, Click `messages/events`, copy value of `Event Hub-compatible name`. And add another Consumer groups, named `function`.
![copy-EventHub-endpoint-name](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/copy-EventHub-endpoint-name.PNG)
3. Enter `CODetector` on `Name your function`.
![name-CODetector](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/name-CODetector.png)
4. Enter Event Hub name from step 2.Click "Integrate", modify value of "Event Hub consumer group" is `function`.
![creat-CODetector-function](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/creat-CODetector-function.png)
5. Copy `code/run.csx` to Azrue portal, and modify `IOTHUB_CONNECT_STRING` on the `run.csx`. On the right side to add file named `project.json`, copy `code/project.json`.
![copy-code-CO](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/copy-code-CO.png)
6. Now, You can run AzureGroveKit UWP app to test function.
### code/run.csx
```
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
```
### code/project.json
```
{
"frameworks": {
"net46": {
"dependencies": {
    "Microsoft.AspNet.WebApi.Client": "5.2.3",
    "Microsoft.AspNet.WebApi.Core": "5.2.3",
    "Microsoft.Azure.Devices": "1.2.4",
    "Newtonsoft.Json": "10.0.2"
}
}
}
}
```
## Reference
How to use Azure Event Hubs:[https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-hubs](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-hubs)