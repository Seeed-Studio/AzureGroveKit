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
        public Boolean lockState;

        public GroveMessage GetSensorValue()
        {
            BooleanGenerator boolGen = new BooleanGenerator();
            Random rnd = new Random();
            GroveMessage message = new GroveMessage();
            message.Temp = rnd.Next(10, 30);
            message.Hum = rnd.Next(40, 100);
            message.Sound = rnd.Next(100, 200);
            message.Light = rnd.Next(100, 200);
            message.GasSO = rnd.Next(10, 100);
            message.PIR = boolGen.NextBoolean();
            message.Timestamp = DateTime.Now.ToString();

            return message;
        }

        public bool GetEvent()
        {
            return true;
        }

        public void DisplayOLED(String msg)
        {
            Debug.WriteLine("Display: " + msg);
        }

        public void ControlMotoDriver(Boolean onoff)
        {
            Debug.WriteLine("\t Motor turn " + onoff);
        }

        internal bool GetButtonValue()
        {
            return false;
        }

        internal void ControlRelay(bool onoff)
        {
            Debug.WriteLine("\t Realy turn " + onoff);
        }

        public class BooleanGenerator
        {
            Random rnd;

            public BooleanGenerator()
            {
                rnd = new Random();
            }

            public bool NextBoolean()
            {
                return Convert.ToBoolean(rnd.Next(0, 2));
            }
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
        IRelay relay;
        public Boolean lockState;

        public SensorController() {
            temphumiSensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin2, DHTModel.Dht11);
            pirMotion = DeviceFactory.Build.PIRMotionSensor(Pin.DigitalPin3);
            button = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin4);
            relay = DeviceFactory.Build.Relay(Pin.DigitalPin5);
            soundSensor = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);
            lightSensor = DeviceFactory.Build.LightSensor(Pin.AnalogPin1);
            gasSensor = DeviceFactory.Build.GasSensorMQ2(Pin.AnalogPin2);
            display = DeviceFactory.Build.OLEDDisplay128X64();
            motor = DeviceFactory.Build.MiniMotorDriver();
            lockState = false;
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

        public void ControlRelay(Boolean onoff)
        {
            if (onoff)
            {
                relay.ChangeState(SensorStatus.On);
            }
            else
            {
                relay.ChangeState(SensorStatus.Off);
            }
        }
#endif
    }

}
