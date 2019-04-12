# Abstract
Grove Kit for Win10 IoT Core & Azure Platform is an IoT development kit which contains some Grove hardware module and designed for Microsoft Azure services.

Unlike traditional Grove Getting Started kit, this kit does not concerned with how to write hardware driver or embedded development, but helps you quickly understand and learn how to use Windows 10 IoT Core and Microsoft Azure services. And We wrote a guide book for this Grove kit which included five projects with separate scenario.

Here we only provide a way to build the application. The sensor data can already be obtained from the iot-hub, and users should build the application according to their actual situation, instead of simply using the Demo.

## Features:

* A guide book for five scenarios
* Support Azure IoT Hub and Azure Functions
* Based on Raspberry Pi and Windows 10 IoT Core

## Flow：
1. Install  Win10 IoT Core Dashboard.
2. Flash Win10 IoT Core into Raspberry Pi.
3. Configure Raspberry Pi's IP, Time and etc.
4. Configure IoT Core TPM String.
5. Deploy UWP in Raspberry Pi.
6. Test UWP and view data by DeviceExplorer.
7. Use this data to build your own Function or applications.

## Scenarios:
#### [Scenario 1: Don't catch cold](https://github.com/Jenkinlu001/AzureGroveKit/blob/master/Guidance/Weather.md)
Check the value of the temperature and humidity sensor. And then tell you not to catch cold when it low.

#### [Scenario 2: Sound&Light and relay](https://github.com/Jenkinlu001/AzureGroveKit/blob/master/Guidance/IFTTT.md)
When the Sound or Light sensor is greater than a value triggers the Microsoft Azure Function for relay sensor, then Function connects to the Maker channel of the IFTTT.

#### [Scenario 3: GAS monitor](https://github.com/Jenkinlu001/AzureGroveKit/blob/master/Guidance/CODetector.md)
Gas send data to Azure, if CO‘s value exceeded, triggering exception Microsoft Azure Function, send eamil to the user, as well as opening mini fan.

#### [Scenario 4: One-Click SOS](https://github.com/Jenkinlu001/AzureGroveKit/blob/master/Guidance/ButtonSOS.md)
Button triggers an SOS event, Microsoft Azure Function sends an email or a call to a family.

#### [Scenario 5: Human detector](https://github.com/Jenkinlu001/AzureGroveKit/blob/master/Guidance/PIRDetector.md)
PIR sensor sends human motion events to Microsoft Azure IoT Hub, within half an hour PIR triggers more than three times, recording this and then send a statistical report to PowerBI.