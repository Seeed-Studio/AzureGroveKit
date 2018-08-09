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
        SensorController sensorController;
        private string deviceId;
        private string hubUri;
        private string sasToken;

        public IotHubClient(Action<object> callMeLogger, Action<object> errorHandler)
        {
            this.callMeLogger = callMeLogger;
            this.errorHandler = errorHandler;
            sensorController = new SensorController();
#if SIMULATE
            var builder = IotHubConnectionStringBuilder.Create(ConnectionStringProvider.Value);
            hubUri = builder.HostName;
            deviceId = builder.DeviceId;
#else
            initConnectString();
#endif 
        }

        private void initConnectString()
        {
            TpmDevice myDevice = new TpmDevice(0);
            hubUri = myDevice.GetHostName();
            deviceId = myDevice.GetDeviceId();
            sasToken = myDevice.GetSASToken();
            
            if (String.IsNullOrEmpty(hubUri) || String.IsNullOrEmpty(deviceId) || String.IsNullOrEmpty(sasToken))
            {
                throw new ArgumentNullException("","Please fill Device ConnectString (From Azure) on TPM.");
            }
        }

        public async Task Start()
        {
#if SIMULATE
            this.deviceClient = DeviceClient.CreateFromConnectionString(ConnectionStringProvider.Value, TransportType.Mqtt);
#else
            this.deviceClient = DeviceClient.Create(hubUri,
                AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Mqtt);
#endif
            await this.deviceClient.OpenAsync();

            await deviceClient.SetMethodHandlerAsync("DisplayOLED", DisplayOLED, null);
            await deviceClient.SetMethodHandlerAsync("ControlMotor", ControlMotoDriver, null);
            await deviceClient.SetMethodHandlerAsync("ControlRelay", ControlRelay, null);

            Debug.WriteLine("Exited!\n");
        }


        public string getDeviceId()
        {
            return deviceId;
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

        private Task<MethodResponse> DisplayOLED(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine(String.Format("method DisplayOLED: {0}", methodRequest.DataAsJson));
            try
            {
                MethodData m = JsonConvert.DeserializeObject<MethodData>(methodRequest.DataAsJson);
                sensorController.DisplayOLED(m.text);
            }
            catch (Exception e)
            {
                this.callMeLogger(String.Format("Wrong message: {0}", methodRequest.DataAsJson));
                return Task.FromResult(new MethodResponse(400));
            }
            this.callMeLogger(methodRequest.DataAsJson);
            return Task.FromResult(new MethodResponse(200));
        }

        private Task<MethodResponse> ControlMotoDriver(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine(String.Format("method ControlMotoDriver: {0}", methodRequest.DataAsJson));
            try
            {
                MotorMethodData m = JsonConvert.DeserializeObject<MotorMethodData>(methodRequest.DataAsJson);
                sensorController.ControlMotoDriver(m.onoff);
            }
            catch (Exception e)
            {
                this.callMeLogger(String.Format("Wrong message: {0}", methodRequest.DataAsJson));
                return Task.FromResult(new MethodResponse(400));
            }
            this.callMeLogger(methodRequest.DataAsJson);
            return Task.FromResult(new MethodResponse(200));
        }

        private Task<MethodResponse> ControlRelay(MethodRequest methodRequest, object userContext)
        {
            Debug.WriteLine(String.Format("method ControlRelay: {0}", methodRequest.DataAsJson));
            try
            {
                MotorMethodData m = JsonConvert.DeserializeObject<MotorMethodData>(methodRequest.DataAsJson);
                sensorController.ControlRelay(m.onoff);
            }
            catch (Exception e)
            {
                this.callMeLogger(String.Format("Wrong message: {0}", methodRequest.DataAsJson));
                return Task.FromResult(new MethodResponse(400));
            }
            this.callMeLogger(methodRequest.DataAsJson);
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
