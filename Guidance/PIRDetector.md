# Scenario 5: Human detector
## What problem does Scenario 5 solve?
In this scenario, we will use `Grove - PIR Motion Sensor` to monitor human motion event. In this case, the PIR sensor sends human motion events to Microsoft Azure IoT Hub, within half an hour PIR triggers more than three times, recording this and then send a statistical report to PowerBI.
## Hardware setup
Connecting `Grove - PIR Motion Sensor` sensor to GrovePi's D3 port, and then power the Raspberry Pi with USB.
hardware list:
>1. Raspberry Pi 3
>2. GrovePi+
>3. Grove - PIR Motion Sensor
>4. One Grove Cable

## Reference
* PowerBI go from data to insights in minutes. Any data, any way, anywhere. And all in one view.Here we use PowerBI visualize real-time sensor data from Azure IoT Hub and display PIR count per hour.

>1. If you have created a output on SA, The datasets of your workspace should exist.
>2. Click "My Workspace --> DATASETS", choice the dataset from SA.
>3. Make your own diagram, refer to [https://powerbi.microsoft.com/en-us/guided-learning/](https://powerbi.microsoft.com/en-us/guided-learning/)

* An on-demand real-time analytics service to power intelligent action. Here we use it process IoTHub data and send to PowerBI.
code refer to:[https://github.com/Seeed-Studio/AzureGroveKit/tree/master/StreamAnalytics](https://github.com/Seeed-Studio/AzureGroveKit/tree/master/StreamAnalytics)

>1. Add new SA, "New --> Internet of Things --> New Stream Analytics Job"
>2. Click Inputs, Click Add, Source choice "IoT hub", Consumer group choice "sa"
>3. Click Outputs, Click Add, Sink choice "Power BI", Authorize or Create.
>4. Click Query, copy content of query.txt to here.
>5. Click Overview, start.