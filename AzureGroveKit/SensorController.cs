using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
//using GrovePi;
//using GrovePi.I2CDevices;

namespace AzureGroveKit
{
    class SensorController
    {
        public static GroveMessage GetSensorData()
        {
            GroveMessage message = new GroveMessage();
            Random rd = new Random();
            message.Temp = rd.Next().ToString();
            message.Hum = rd.Next().ToString();
            message.Sound = rd.Next().ToString();
            message.Light = rd.Next().ToString();
            message.GasSO = rd.Next().ToString();
            message.Timestamp = DateTime.Now.ToString();

            return message;
        }

        public static void DisplayLCD(String msg)
        {
            Debug.WriteLine("\t{0}", msg);
            //IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();
            //display.SetText("Hello from Dexter Industries!").SetBacklightRgb(255, 50, 255);
        }

        public static Task<MethodResponse> TurnOnMotoDriver(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }
}
