# GeoVR
Geographical Virtual Radio

## Purpose

The Geo VR solution provides a client/server implementation simulating VHF radio.

Although designed to be used as the replacement 'voice codec' for VATSIM this project has other uses such as Amatuer Radio simulation.

## Technology

GeoVR is written using the very 'anti faff' ZeroMQ in C# using the OPUS protocol.  There is a .NET interface as well as a native C++ DLL allowing it to be called from both .NET applications or any C++ application natively.

## Concept

The GeoVR voice server acts as a 'router' to route traffic from clients based on geographical location and frequency tuned.

The client sends and receives data as well as adding additional the VHF simulation.

Voice data is transmitted at a sufficient bit rate to provide high quality audio.

The world is split up into 1000km square quadrants for each frequency in the VHF spectrum.

In order for minimum overhead on the server the client will fine tune line of sight simulation.

