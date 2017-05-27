//#define SIMULATE
using Microsoft.Azure.Devices.Client;
using Microsoft.Devices.Tpm;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace AzureGroveKit
{
    class IotHubClient
    {
        public DeviceClient deviceClient;
        Action<object> callMeLogger;
        Action<object> errorHandler;

        public IotHubClient(Action<object> callMeLogger, Action<object> errorHandler)
        {
            this.callMeLogger = callMeLogger;
            this.errorHandler = errorHandler;
        }

        public async Task Start()
        {
            try
            {
#if SIMULATE
                this.deviceClient = DeviceClient.CreateFromConnectionString(ConnectionStringProvider.Value, TransportType.Mqtt);
#else
                TpmDevice myDevice = new TpmDevice(0); 
                string hubUri = myDevice.GetHostName();
                string deviceId = myDevice.GetDeviceId();
                string sasToken = myDevice.GetSASToken();
                this.deviceClient = DeviceClient.Create(hubUri,
                    AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Mqtt);
#endif
                await this.deviceClient.OpenAsync();

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
            SensorController sensorController = new SensorController();
            MethodData m = JsonConvert.DeserializeObject<MethodData>(methodRequest.DataAsJson);
            sensorController.DisplayLCD(m.Msg);
            this.callMeLogger(methodRequest.DataAsJson);
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        private Task<MethodResponse> ControlMotoDriver(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }

    class MethodData
    {
        public String Msg { get; set; }
    }


}
