using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace AzureGroveKit
{
    class IotHubClient
    {
        public DeviceClient deviceClient;
        private string deviceConnectionString;

        Action<object> callMeLogger;
        //Action<object> getDeviceNameLogger;
        Action<object> errorHandler;

        public IotHubClient(String deviceConnectionString, Action<object> callMeLogger, Action<object> errorHandler)
        {
            //if (String.IsNullOrEmpty(deviceConnectionString))
            //    this.deviceConnectionString = ConnectionStringProvider.Value;
            //else
            this.deviceConnectionString = deviceConnectionString;
            this.callMeLogger = callMeLogger;
            //this.getDeviceNameLogger = getDeviceNameLogger;
            this.errorHandler = errorHandler;
        }

        public async Task Start()
        {
            try
            {
                this.deviceClient = DeviceClient.CreateFromConnectionString(this.deviceConnectionString, TransportType.Mqtt);

                await this.deviceClient.OpenAsync();

                // Set up callbacks:
                await deviceClient.SetMethodHandlerAsync("DisplayLCD", DisplayLCD, null);
                await deviceClient.SetMethodHandlerAsync("TurnOnMotoDriver", ControlMotoDriver, null);

                Debug.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        public Task CloseAsync()
        {
            return this.deviceClient.CloseAsync();
        }

        public async Task SendDeviceToCloudMessagesAsync(Message message)
        {
            await deviceClient.SendEventAsync(message);
        }

        public async Task<Message> ReceiveC2dAsync()
        {
            Message receivedMessage = await deviceClient.ReceiveAsync();
            await deviceClient.CompleteAsync(receivedMessage);
            return receivedMessage;
        }

        private Task<MethodResponse> DisplayLCD(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);
            //IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();
            //display.SetText("Hello from Dexter Industries!").SetBacklightRgb(255, 50, 255);

            SensorController.DisplayLCD(methodRequest.DataAsJson);
            this.callMeLogger(methodRequest.DataAsJson);
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        private Task<MethodResponse> ControlMotoDriver(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }
}
