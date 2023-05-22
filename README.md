# Wirepas-.Net-Sample

<!-- MarkdownTOC levels="1,2,3" autolink="true"  -->

- [Introduction](#introduction)
- [Technical Description](#technical-description)


# Introduction
This repository provide an integration sample to use the Blue MESH tags provided by ELA Innovation. You will find here a Visual Studio 2019 project (you can download the VS 2019 Community [here](https://visualstudio.microsoft.com/fr/downloads/)) using WPF and C# to define an simple User Interface to get data from Wirepas MESH Network using Blue MESH from ELA Innovation.

# Technical Description
Before running and starting the project, you need to understand the Wirepas Technology, the network and the place of ELA Blue MESH Tags in the Wirepas Network. If you are a developper, you can consult the different Wirepas Github repository [here](https://github.com/wirepas) to understand the Network, the topology, the different tools requiered to build a Wirepas MESH infrastructure. But if you are already informed, there you can continue and use this project and the tag you receoved from ELA to run this project and try to get information from your Blue MESH.

First, you need to be sure that your Wirepas Gateway publish the network information on a MQTT Broker. You can use software like [MQTT Box](http://workswithweb.com/mqttbox.html) for Windows, [Mosquitto](https://mosquitto.org) for Unix to test your data. You need to install a MQTT Broker on you local network to publish and get the data from the network. To install a broker on Unix device, you can find more information [here](https://www.instructables.com/Installing-MQTT-BrokerMosquitto-on-Raspberry-Pi/). Then, you can conncet to your broker and subscribe to a main topic (for example #) but you can find more information about the wirepas topic [here](https://github.com/wirepas/backend-apis/blob/master/gateway_to_backend/README.md)

Finally, this project uses the **ElaWirepasLibrary** nuget developped by ELA Innovation that you can find [here](https://www.nuget.org/packages/ElaWirepasLibrary/), specifically the **WirepasMqttController** class: it's a wrapper around an MQTT client (from [MQTTNet library](https://github.com/dotnet/MQTTnet)) which exposes Wirepas mesh functions (scan for data, send commands, etc).


Figure 1 illustrates how we can use Backend Client and how we can use this sample

![here_schematics](https://github.com/elaInnovation/Wirepas-.Net-Sample/blob/master/Images/wirepas_sample_01.png)


