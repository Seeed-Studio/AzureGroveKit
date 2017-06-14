#define SIMULATE
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
        SensorController sensorController;

        public IotHubClient(Action<object> callMeLogger, Action<object> errorHandler)
        {
            this.callMeLogger = callMeLogger;
            this.errorHandler = errorHandler;
            sensorController = new SensorController();
        }

        public async Task Start()
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

            await deviceClient.SetMethodHandlerAsync("DisplayLCD", Display, null);
            await deviceClient.SetMethodHandlerAsync("ControlMotor", ControlMotoDriver, null);
        }

        public Task CloseAsync()
        {
            return this.deviceClient.CloseAsync();
        }

        public async Task SendDeviceToCloudMessagesAsync(Message message)
        {
            try {
                await deviceClient.SendEventAsync(message);
            }
            catch (Microsoft.Azure.Devices.Client.Exceptions.UnauthorizedException e)
            {
                throw e;
            }
        }

        public async Task<Message> ReceiveC2dAsync()
        {
            Message receivedMessage = await deviceClient.ReceiveAsync();
            await deviceClient.CompleteAsync(receivedMessage);
            return receivedMessage;
        }

        private Task<MethodResponse> Display(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);
            MethodData m = JsonConvert.DeserializeObject<MethodData>(methodRequest.DataAsJson);
            sensorController.Display(m.text);
            this.callMeLogger(methodRequest.DataAsJson);
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        private Task<MethodResponse> ControlMotoDriver(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine("\t{0}", methodRequest.DataAsJson);
            MotorMethodData m = JsonConvert.DeserializeObject<MotorMethodData>(methodRequest.DataAsJson);
            sensorController.ControlMotoDriver(m.onoff);
            this.callMeLogger(methodRequest.DataAsJson);
            //return Task.FromResult(new MethodResponse(new byte[0], 200));
            return Task.FromResult(new MethodResponse(200));
        }
    }

    class MethodData
    {
        public String text { get; set; }
    }

    class MotorMethodData
    {
        public Boolean onoff { get; set; }
    }

}
