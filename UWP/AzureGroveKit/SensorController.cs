//#define SIMULATE
using System;
using System.Diagnostics;
#if SIMULATE
#else
using GrovePi;
using GrovePi.I2CDevices;
using GrovePi.Sensors;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
#endif

namespace AzureGroveKit
{
    class SensorController
    {
#if SIMULATE
        public GroveMessage GetSensorValue()
        {
            GroveMessage message = new GroveMessage();
            message.Temp = 25.0;
            message.Hum = 69.0;
            message.Sound = 123;
            message.Light = 112;
            message.GasSO = 23;
            message.PIR = true;
            message.Timestamp = DateTime.Now.ToString();

            return message;
        }

        public bool GetEvent()
        {
            return true;
        }

        public void DisplayLCD(String msg)
        {
            Debug.WriteLine("Display: " + msg);
        }

        public void ControlMotoDriver(Boolean onoff)
        {
            Debug.WriteLine("\t turn " + onoff);
        }

        internal bool GetButtonValue()
        {
            return false;
        }
#else
        IDHTTemperatureAndHumiditySensor temphumiSensor;
        ISoundSensor soundSensor;
        ILightSensor lightSensor;
        IGasSensorMQ2 gasSensor;
        IPIRMotionSensor pirMotion;
        IOLEDDisplay128X64 display;
        IMiniMotorDriver motor;
        IButtonSensor button;

        public SensorController() {
            temphumiSensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin2, DHTModel.Dht11);
            soundSensor = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);
            lightSensor = DeviceFactory.Build.LightSensor(Pin.AnalogPin1);
            gasSensor = DeviceFactory.Build.GasSensorMQ2(Pin.AnalogPin2);
            pirMotion = DeviceFactory.Build.PIRMotionSensor(Pin.DigitalPin3);
            display = DeviceFactory.Build.OLEDDisplay128X64();
            motor = DeviceFactory.Build.MiniMotorDriver();
            button = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin4);
        }

        public GroveMessage GetSensorValue()
        {
            GroveMessage message = new GroveMessage();

            try
            {
                temphumiSensor.Measure();
                message.Temp = temphumiSensor.TemperatureInCelsius;
                message.Hum = temphumiSensor.Humidity;
                message.Sound = soundSensor.SensorValue();
                message.Light = lightSensor.SensorValue();
                message.GasSO = gasSensor.SensorValue();
                message.PIR = pirMotion.IsPeopleDetected();
                message.Timestamp = DateTime.Now.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return message;
        }

        public bool GetButtonValue()
        {
            string buttonon = button.CurrentState.ToString();
            bool buttonState = buttonon.Equals("On", StringComparison.OrdinalIgnoreCase);
            return buttonState;
        }

        public void DisplayOLED(String msg)
        {
            display.init();  
            display.clearDisplay();
            display.setNormalDisplay();
            display.setPageMode();
            display.setTextXY(0, 0);
            display.putString(msg);
        }

        public void ControlMotoDriver(Boolean onoff)
        {
            if (onoff)
            {
                motor.drive1(100);
                motor.drive2(100);
            } else
            {
                motor.drive1(0);
                motor.drive2(0);
            }
        }
#endif
    }

}
