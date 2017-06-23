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
