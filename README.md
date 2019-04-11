# Overview
The full name of this kit is "Grove Kit for Win10 Iot Core & Azure Platform". This kit and guide book will help you quickly get familiar with Windows 10 IoT Core and Azure. 

The version of Windows 10 IoT Core this guide used is 10.0.17134.1.
![Data flow](data-flow.png)
![Physical](physical.jpg)

# Get Started
## Setup Your Raspberry Pi
### Install Win10 IoT Core
1. Download IOT Core Dashboard from https://developer.microsoft.com/en-us/windows/iot/Downloads and install.
2. Open the Windows 10 IoT Core Dashboard.
3. Click "Set up a new device".
4. Select Raspberry Pi 3 from the dropdown.
5. Enter device name, password and Wi-Fi network to connect to, the password can't be too simple otherwise you won't login the Windows 10 IoT Core web console.
6. Download and flash Windows 10 IoT Core into your SD card.
7. A window will pop up to show you the progress. This step can take several minutes depending on the speed of your SD card.
8. Put SD Card into Raspberry Pi
9. Connect a display with your Raspberry Pi, either HDMI or LVDS screen. 
10. Power on the Raspberry Pi.
11. If you configured the WiFi SSID and password in step 5, you should see the IP address at the screen now. If you don't, please connect the Pi to a wired ethernet, or find an USB keyboard and mouse to configure the WiFi in the GUI of Windows 10 IoT Core.
12. Set down the IP adress.
>Notice:
If there is no device found in my device in Dashboard, please make PC and raspberry Pi in the same IP segment.
### Setup Azure IoT Hub Devices Connect String for TPM

We assume that you've already setup an IoT Hub and created a device (Refer to [Setup Azure IoT Hub](#setup-azure-iot-hub)).

1. Open the browser of your PC, login Windows 10 IoT Core web console "http://ip-address:8080", Account: Administrator, Password: (what you entered in the IoT Core Dashboard)
2. Click the "TPM configuration" menu on the left.
3. Intall "Software TPM Emulator(NoSecurity)", and click "OK" to reboot the system when prompt.
4. After the system is rebooted, navigate to "http://ip-address:8080" - "TPM configuration" again. 
5. Copy the "Connection string—primary key" of your created device in Azure IoT Hub
6. Paste into "TPM configuration --> Logical devices settings --> Logical device ID: 0 --> Azure Connection String", save.


### Deploy AzureGroveKit UWP App
The UWP App will connect to Azure IoT Hub, collect Grove sensor data and control Grove output.
1. Open the browser of your PC, login Windows 10 IoT Core web console "http://ip-address:8080"
2. Navigate "Apps --> Apps manager", if you've installed the AzureGroveKit App before, please uninstall it first (Mac Safari is proved to be not showing the App list properly, Chrome is recommended).
3. Click "Add", select "Install app package from network or web location", input https://raw.githubusercontent.com/Seeed-Studio/AzureGroveKit/master/UWP/Release/AzureGroveKit_1.0.8.0_Test/Dependencies/ARM/Microsoft.NET.CoreRuntime.1.1.appx. Click "Next", and wait the installation done.
>Notice:
Due to the version update, cancel "Add", download the files linked above, and then select local installation.
4. Click "Add", select "Install app package from network or web location", input https://raw.githubusercontent.com/Seeed-Studio/AzureGroveKit/master/UWP/Release/AzureGroveKit_1.0.8.0_Test/AzureGroveKit_1.0.8.0_arm.appxbundle. Click "Next", and wait the installation done.
5. Wait a few seconds or refresh, you will find AzureGroveKit in the App list.

### Test AzureGroveKit App
1. Power off the Raspberry Pi, plug the GrovePi board.
2. Connect necessary Groves on GrovePi:

  Grove| GrovePi Port
  -----| ------------
  D2   | Grove - Temp&Humi Sensor
  D3   | Grove - PIR Motion Sensor
  D4   | Grove – Button
  D5   | Grove - Relay
  A0   | Grove - Sound Sensor
  A1   | Grove - Light Sensor
  A2   | Grove - Gas Sensor
  I2C1 | Grove - OLED Display 0.96"
  I2C2 | Grove - Mini I2C Motor Driver
3. Power on the Raspberry Pi and login Windows 10 IoT Core web console "http://ip-address:8080"
4. Move on "Apps --> Apps manager", start "AzureGroveKit" via its "Actions" dropdown.
5. The App will display on the screen, find an USB mouse, click "Run", now the App starts to send messages to Azure IoT Hub, and can response to the downlink commands.
>Notice: 
If you have an unauthorized connection or something like that, you can use `Dashboard- >connect to Azure`, choose your iot-hub, choose your device, choose raspberry pi, and provision. Reinstall and run the UWP.
6. Check "YOUR IOTHUB --> Overview --> Usage", it will show the message count.
7. If you're using Windows, you can [use DeviceExplorer](#setup-deviceexplorer) to monitor the uploaded messages.
8. More UWP overview, please refer to https://github.com/Seeed-Studio/AzureGroveKit/tree/master/UWP
>Notice:
If UWP is not available, try using VS 2017 to open and run the project : [https://github.com/Seeed-Studio/AzureGroveKit/tree/master/UWP](https://github.com/Seeed-Studio/AzureGroveKit/tree/master/UWP)
## Setup Azure IoT Hub
1. Prepare your Azure account and login "https://portal.azure.com/".
2. Create "New --> Internet of Things --> IoT hub", for instructions please refer to https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal
3. Create a device, move on "All resources --> YOUR_IOTHUB --> Explorers --> IoT devices --> Add", save
4. Click the device, copy "Connection string—primary key"

## Setup DeviceExplorer

1. Download DeviceExplorer [here](https://github.com/Azure/azure-iot-sdk-csharp/blob/master/tools/DeviceExplorer/readme.md)
2. Prepare your Azure account and login "https://portal.azure.com/".
3. Navigate "your-iot-hub --> Shared access policies -->iothubowner", copy "Connection string—primary key"
4. Paste the connection string into DeviceExplorer, "Configuration --> Connection Information --> IoT Hub Connection String", and clieck "Update"
5. Click the "Management" tab, click "List" or "Refresh"
6. Click the "Data" tab, select your added device in "Device ID:" dropdown
7. Click "Monitor"

![image](https://user-images.githubusercontent.com/5130185/43897130-b35e6b4e-9c0d-11e8-846c-95570be41168.png)



