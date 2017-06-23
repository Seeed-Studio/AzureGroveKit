using Newtonsoft.Json;
using System;
using TestLib;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ButtonSOS
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(TestData.GetTestButtonEventString(), new TraceWriter());
        }

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
    }
}
