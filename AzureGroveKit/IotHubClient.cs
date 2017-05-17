using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace ms_sample
{
    class IotHubClient
    {
        private String iotHubUri;
        private String iotHubConnectString;
        private String deviceId;
        public String deviceKey;
        public DeviceClient deviceClient;

        private IotHubClient() { }

        public static async Task<IotHubClient> CreateAsync(String iotHubConnectString, String deviceId)
        {
            IotHubClient x = new IotHubClient();
            await x.Initialize(iotHubConnectString, deviceId);
            return x;
        }

        private async Task Initialize(String iotHubConnectString, String deviceId)
        {
            this.iotHubConnectString = iotHubConnectString;
            this.deviceId = deviceId;
            parseIotString(iotHubConnectString);
            deviceKey = await GetDeviceKeyAsync(deviceId);
            deviceClient = DeviceClient.Create(iotHubUri,
                new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey),
                Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            await deviceClient.OpenAsync();
            await deviceClient.SetMethodHandlerAsync("DisplayLCD", DisplayLCD, null);
            //await deviceClient.SetMethodHandlerAsync("TurnOnMotorDriver", SensorController.TurnOnMotoDriver, null);
        }

        public static Task<MethodResponse> DisplayLCD(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);
            //IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();
            //display.SetText("Hello from Dexter Industries!").SetBacklightRgb(255, 50, 255);

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }


        public async Task UnHandlerAsync()
        {
            await deviceClient?.SetMethodHandlerAsync("DisplayLCD", null, null);
            await deviceClient?.SetMethodHandlerAsync("TurnOnMotorDriver", null, null);
            await deviceClient?.CloseAsync();
        }

        private void parseIotString(String iotString)
        {
            try
            {
                var builder = Microsoft.Azure.Devices.IotHubConnectionStringBuilder.Create(iotHubConnectString);
                this.iotHubUri = builder.HostName;
            }
            catch
            {
                throw;
            }
        }

        private async Task<String> GetDeviceKeyAsync(String deviceId)
        {
            Device device;
            RegistryManager registryManager;
            registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectString);    
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public async Task SendDeviceToCloudMessagesAsync(Microsoft.Azure.Devices.Client.Message message)
        {
            await deviceClient.SendEventAsync(message);
        }

        public async Task<Microsoft.Azure.Devices.Client.Message> ReceiveC2dAsync()
        {
            Microsoft.Azure.Devices.Client.Message receivedMessage = await deviceClient.ReceiveAsync();
            await deviceClient.CompleteAsync(receivedMessage);
            return receivedMessage;
        }
    }
}
