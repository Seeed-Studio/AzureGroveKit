using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureGroveKit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CancellationTokenSource ctsForStart;
        IotHubClient iotClient;
        SensorController sensorController;
        int callMeCounter;
        int sendMessageCounter;

        public MainPage()
        {
            this.InitializeComponent();
            callMeCounter = 0;
            sendMessageCounter = 0;
            sensorController = new SensorController();
        }

        private async void runbutton_Click(object sender, RoutedEventArgs e)
        {
            ctsForStart = new CancellationTokenSource();
            runbutton.IsEnabled = false;
            iotClient = new IotHubClient(CallMeLogger, null);
            await iotClient.Start();
            //await sendMessageAsync(3000);
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            callMeCounter = 0;
            sendMessageCounter = 0;
            messageSendList.Items.Clear();
            methodCallList.Items.Clear();
        }

        private async Task sendMessageAsync(int delayTime)
        {
            while (true)
            {
                if (ctsForStart.IsCancellationRequested)
                {
                    return;
                }
                GroveMessage groveMessage = sensorController.GetSensorValue();
                var messageSerialized = JsonConvert.SerializeObject(groveMessage);
                var encodedMessage = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageSerialized));
                await iotClient.SendDeviceToCloudMessagesAsync(encodedMessage);
                SendMessageLoger(messageSerialized);
                await Task.Delay(delayTime);
            }
        }

        private async Task recieiveMessageAsync(String iotHubUri, String iotHubConnectString)
        {
            while (true)
            {
                if (ctsForStart.IsCancellationRequested)
                {
                    return;
                }

                Microsoft.Azure.Devices.Client.Message message = await iotClient.ReceiveC2dAsync();
                if (message == null)
                    continue;
                Debug.WriteLine("Received message: " + Encoding.ASCII.GetString(message.GetBytes()));
            }
        }


        private void SendMessageLoger(object element)
        {
            AddItemToListBox(messageSendList, string.Format("[{0}] {1}", sendMessageCounter++, element));
        }


        private void CallMeLogger(object element)
        {
            AddItemToListBox(methodCallList, string.Format("[{0}] {1}", callMeCounter++, element));
        }

        private void AddItemToListBox(ListBox list, string item)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    list.Items.Add(item);

                    var selectedIndex = list.Items.Count - 1;
                    if (selectedIndex < 0)
                        return;

                    list.SelectedIndex = selectedIndex;
                    list.UpdateLayout();

                    list.ScrollIntoView(list.SelectedItem);
                });
        }

        private async void stopButton_Click(object sender, RoutedEventArgs e)
        {
            ctsForStart.Cancel();
            runbutton.IsEnabled = true;
            await iotClient.CloseAsync();
        }

    }
}
