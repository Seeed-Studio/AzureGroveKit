# Scenario 1: Don't catch cold
## What problem does Scenario 1 solve?
In this scenario, we will use `Grove - Temp & Humidity` to develop a project called Don't catch cold. In this case, we are check the value of the temperature and humidity sensor, and then tell you not to catch cold when it lower then 30 degree. As the first project, we hope to let everyone get started quickly, without technical details. So just include one sensor: `Grove - Temp & Humidity`.
## Hardware setup
Connecting `Grove - Temp&Humidity` sensor to GrovePi's D2 port, and then power the Raspberry Pi with USB.
hardware list:
>1. Raspberry Pi 3
>2. GrovePi+
>3. Grove - Temp&Humidity
>4. One Grove Cable
## Diagram:
![diagram-for-scenario1](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/diagram-for-scenario1.png)
## Services
### Azure sevices
* Micrsoft Azure IoT Hub: Use to manage and monitor Grove module.
* Micrsoft Azure Functions: We can use fuction to measures temperature and humidity data, and send an alert email via third-party email service.
### Other services
* Gmail STMP service.
### Up and run
#### setup
1. Create a function app and deploy new function app
![create-temp-humidity-function-app](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/create-temp-humidity-function-app.png)
![deploy-temp-humidity-function-app](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/deploy-temp-humidity-function-app.png)
2. Create trigger function
![create-temp-humidity-trigger-function](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/create-temp-humidity-trigger-function.png)
3. Test function(please copy `code` section to function)
![test-temp-humidity-function](https://raw.githubusercontent.com/Jenkinlu001/Seeed_Picture/master/test-temp-humidity-function.png)
#### code
```
using System;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Net.Mail;
using System.Net;

// When the temperature is below this value, send mail notification
const int coldTemp = 30;

const String fromEmailAddress = "YOUR-GMAIL-ADDRESS";

// From here https://myaccount.google.com/apppasswords generate specify password
const String fromEmailPassword = "YOUR-GMAIL-PASSWORD";

const String toEmailAddress = "RECEIVE-EMAIL-ADDRESS";

const string IOTHUB_CONNECT_STRING = "YOUR_IOTHUB_CONNECT_STRING";


public static void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    GroveMessage m = JsonConvert.DeserializeObject<GroveMessage>(myEventHubMessage);
    var temp = m.Temp;
    var deviceId = m.DeviceId;
    if (temp < coldTemp)
    {
        SendEmail(temp);
        ShowOnGroveOLEDAsync(deviceId, temp).Wait();
    }
}

public static void SendEmail(double temp)
{
    MailMessage mail = new MailMessage(fromEmailAddress, toEmailAddress);
    SmtpClient client = new SmtpClient();
    client.Port = 587;
    client.DeliveryMethod = SmtpDeliveryMethod.Network;
    client.UseDefaultCredentials = false;
    client.EnableSsl = true;
    client.Host = "smtp.gmail.com";
    client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
    mail.Subject = "The weather is cold.";
    mail.Body = string.Format("The temperature has reached {0} degrees Celsius, please add clothes.", temp);
    client.Send(mail);
}

public static async Task ShowOnGroveOLEDAsync(string deviceId, double temp)
{
    var methodInvocation = new CloudToDeviceMethod("DisplayOLED") { ResponseTimeout = TimeSpan.FromSeconds(30) };
    var message = new { text = "Cold Temp: " + temp + "¡æ" };
    string messageString = JsonConvert.SerializeObject(message);
    methodInvocation.SetPayloadJson(messageString);
    ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOTHUB_CONNECT_STRING);
    await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);
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
### Outcome
Get an alert email from Azure Function.
### Refer links:
[Creating Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function)
[Souce code for this scenario](https://github.com/Seeed-Studio/AzureGroveKit/blob/master/Function/Weather/run.csx)