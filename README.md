# GeoVR
Geographical Virtual Radio

## Purpose

The Geo VR solution provides a C# client/server VOIP implementation simulating radio broadcasting.

The primary goal is for Air Traffic Control simulation however this project has other uses such as Amateur Radio simulation and Trucker CB Radio simulation.

## Technology

GeoVR is written using the very 'anti faff' ZeroMQ Pub/Sub in C# using the OPUS codec.  There is a .NET interface as well as a C++ DLL allowing it to be called from both .NET applications or any C++ application natively.

## Concept

The GeoVR voice server acts as a 'router' to route voice from clients based on geographical location and frequency tuned. Every client has a receive radius and a transmit radius, which varies with ground altitude (i.e. Line Of Sight). Two way conversation can only occur when 2 clients are within each others receive/transmit radii and are tuned to the same frequency.

Voice data is transmitted at a sufficient bit rate to provide high quality audio, with the intention that further simulation effects be optionally applied on the client.



