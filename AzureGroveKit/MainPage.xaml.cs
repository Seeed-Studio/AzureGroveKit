using ms_sample;
using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureGroveKit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Dowork();


        }

        private async void Dowork()
        {
            String iotHubConnectString = "HostName=grovekit.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=glNPXfVPc73OBOXW0ofPtabPSFJh37vnakPZ298H+bs=";

            String deviceId = await Utility.getMacAddress();
            IotHubClient iotClient = await IotHubClient.CreateAsync(iotHubConnectString, deviceId);
        }
    }
}
