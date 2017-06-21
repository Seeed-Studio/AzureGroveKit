// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AzureGroveKit
{
    static class ConnectionStringProvider
    {
        // String containing Hostname, Device Id & Device Key in one of the following formats:
        //  "HostName=<iothub_host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"
        //  "HostName=<iothub_host_name>;CredentialType=SharedAccessSignature;DeviceId=<device_id>;SharedAccessSignature=SharedAccessSignature sr=<iot_host>/devices/<device_id>&sig=<token>&se=<expiry_time>";
        //public static String Value => "HostName=GroveKitIotHub.azure-devices.cn;DeviceId=GroveKitDevice;SharedAccessKey=XKW2mqi6HC9uMebs3HRgeYludNGxmLgqYY7AWPJo56M=";
        public static String Value => "HostName=GroveKitIotHub.azure-devices.net;DeviceId=GroveKitDevice;SharedAccessKey=euhAzJiJjZt1aHdZqRztyA6gQ8MK+umg4zEoQKC7lUM=";

    }
}