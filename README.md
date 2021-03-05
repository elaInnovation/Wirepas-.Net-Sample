# Wirepas-.Net-Sample
This repository provide an integration sample to use the Blue MESH tags provided by ELA Innovation. You will find here a Visual Studio 2019 project (you can download the VS 2019 Community [here](https://visualstudio.microsoft.com/fr/downloads/)) using WPF and C# to define an simple User Interface to get data from Wirepas MESH Network using Blue MESH from ELA Innovation.

## Technical Description
Before running and starting the project, you need to understand the Wirepas Technology, the network and the place of ELA Blue MESH Tags in the Wirepas Network. If you are a developper, you can consult the different Wirepas Github repository [here](https://github.com/wirepas) to understand the Network, the topologu, the different tools requiered to build a Wirepas MESH infrastructure. But if you are already informed, there you can continue and use this project and the tag you receoved from ELA to run this project and try to get information from your Blue MESH.

First, you need to be sure that your Wirepas Gateway publish the network information on a MQTT Broker. You can use software like [MQTT Box](http://workswithweb.com/mqttbox.html) for Windows, [Mosquitto](https://mosquitto.org) for Unix to test your data. You need to install a MQTT Broker on you local network to publish and get the data from the network. To install a broker on Unix device, you can find more information [here](https://www.instructables.com/Installing-MQTT-BrokerMosquitto-on-Raspberry-Pi/). Then, you can conncet to your broker and subscribe to a main topic (for example #) but you can find more information about the wirepas topic [here](https://github.com/wirepas/backend-apis/blob/master/gateway_to_backend/README.md)

Then, this program the [M2MMqtt](https://github.com/mohaqeq/paho.mqtt.m2mqtt) nuget to implement a MQTT Client and get the data publish on the MQTT Broker. Only MQTTClient object from the library is used on this example to use a client object to get the data.

## Project Description
To run the solution, open with Visual Studio 2019 the **Wirepas-ELA-Mesh-Sample.sln** File. Then, your solution will display in the ***Solution Explorer***, here you will find a brief description for all the important classes composing the solution. This project use diffenret nuget, but **ElaWirepasLibrary** is the one you need to decode the diffenrent buffer received from MQTT. The project use [M2MMqtt](https://github.com/mohaqeq/paho.mqtt.m2mqtt) to generate a client which connect and subscribe to the MQTT flow. The subsciption is done to a main topic **"#"** to get the entire flow from the broker. If you want to specify another topic, you can do it directly by code with the constant **MQTT_MAIN_TOPIC**.

### Project Overview
You will find all the code in **Wirepas_ELA_Mesh_Sample** namespace:
- MainWindows.cs : main UI. Only one file defined the User Interface
- Communication : find the communication class for MQTT communication **SampleMqttClient** inherit from **MqttClient** available in **M2Mqtt** nuget
- Model : where you can find the configurtion object to initialize a communication with the MQTT broker and another object to subscribe to a topic

#### MainWindows.cs
This class is a simple link with the configuration input from the User Interface. if you want to define another topics (or other), you can directly change the constant:
```C#
  private const String MQTT_MAIN_TOPIC = "#";
```

You can also enable the input **TextBox** directly in the MainWindows.xaml and User Interface:
```xaml
  <TextBox x:Name="tbBrokerTopicInput" Text="#" Width="200" Margin="10" Padding="5" IsEnabled="False"/>
```

#### SampleMqttClient.cs
This class inherit from **MqttClient**. You can find more documentation [here](https://github.com/mohaqeq/paho.mqtt.m2mqtt) about the implementation done in this nuget. For the sample, we won't use security around mqtt commmunication. You can use this object to subscribe and publish using a MQTT broker. Here we just only use the subscription:
```C#
  MqttSubscribeInfo SubscribeInformation = new MqttSubscribeInfo(new string[] { MQTT_MAIN_TOPIC }, new byte[] { 0x00 });
  MqttBrokerInfo BrokerInfos = new MqttBrokerInfo("127.0.0.1", 1883);
  //
  SampleMqttClient client = new SampleMqttClient(BrokerInfos, false, null, null, uPLibrary.Networking.M2Mqtt.MqttSslProtocols.None);
  client.Connect();
  client.Subscribe(SubscribeInformation, SampleMqttClient_MqttMsgPublishReceived);
```

The callback associated in the subscribe function call directly the **ElaWirepasLibrary** nuget to decode the payload received from the MQTT broker. Data from Wirepas MESH Network are here:
```C#
  object decoded_message = ElaWirepasLibrary.Model.Wirepas.Payload.WirepasPayloadFactory.GetInstance().Get(e.Message);
  //
  var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(decoded_message);
  Console.WriteLine(jsonString);
```

Don't forget to unsubscribe before closing the application:
```C#
  client.Unsubscribe(this.m_currentSubscribeInformation, SampleMqttClient_MqttMsgPublishReceived);
  client.Disconnect();
  client = null;
```
