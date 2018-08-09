
# Overview
The UWP App is running on Win10 IoT Core. It can periodically get sensor data and send to Azure IoTHub. Second it can respond events immediately to IoTHub. Third it can receive the command from IoTHub and control actuator.

The version of the Virsual Studio this project used is 2017.

The version of  Windows 10 SDK selected in Virsual Studio is 10.0.15063. The latest Windows 10 SDK version  that the App is tested to be able to run on is 10.0.17134.1.

## Basic reference library
- Microsoft.Azure.Devices.Client   
  Device SDK for Azure IoT Devices
  
- GrovePi   
  an open source platform for connecting Grove Sensors to the Raspberry Pi. 
  Refer to: https://github.com/DexterInd/GrovePi/tree/master/Software/CSharp
  
## Interface for Azure IoTHub
### Send message
A message is json format, you can find all properties on here:
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-construct.
The app will seed two type data to IotHub. Below is data payload.
Grove Sensor Data example:
```
{
    DeviceId = "GroveDevice",
    Temp = 25.0,
    Hum = 69.0,
    Sound = 123,
    Light = 112,
    GasSO = 23,
    PIR = true,
    Timestamp = ""
}
```

Button Event Data example:
```
{
    DeviceId = "GroveDevice",
    Click = true,
    Timestamp = ""
}
```
### Handle method
IoT Hub gives you ability to invoke direct methods on devices from the cloud. Direct methods represent a request-reply interaction with a device similar to an HTTP call in that they succeed or fail immediately (after a user-specified timeout).
```
POST {iot hub}/twins/{device id}/methods
  - Display text on Grvoe OLED
    Body:
    {
      "methodName": "DisplayOLED",
      "responseTimeoutInSeconds": 200,
      "payload": {
          "text": "your text",
      }
    }
    
  - Control Grove Motor Driver on/off
    {
      "methodName": "ControlMotor",
      "responseTimeoutInSeconds": 200,
      "payload": {
          "onoff": true,
      }
    }
  
  - Control Grove Relay on/off
    {
      "methodName": "ControlRelay",
      "responseTimeoutInSeconds": 200,
      "payload": {
          "onoff": true,
      }
    }
```
## How to deploy to Raspberry Pi 3
Please refer to: https://developer.microsoft.com/en-us/windows/iot/docs/appdeployment

## How to simulate test on local machine
This will save you a lot of time when testing Azure functionality.
1. Uncomment first line "#define SIMULATE" on IotHubClient.cs and SensorController.
2. Remove "References -> GrovePi"
3. Enter your IoTHub connect string on ConnectStringProvider.cs
3. Choice Debug, x86, Local Machine
4. Run

## Change the interval of sending messages
MainPage.xaml.cs line 26:
```
private static int sendMessageDelay = 3000;
```
