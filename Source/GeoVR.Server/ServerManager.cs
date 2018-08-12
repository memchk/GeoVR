using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GeoVR.Shared;
using System.Device.Location;

namespace GeoVR.Server
{
    public class ServerManager
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        //---------------------------------------
        private Task taskClientDataSub;
        // publishes objects onto:        
        private BlockingCollection<object> clientDataPubInputQueue = new BlockingCollection<object>();
        // which is consumed by:
        private Task taskClientDataPub;
        //---------------------------------------

        //---------------------------------------
        private Task taskClientAudioSub;
        // publishes objects onto:
        private BlockingCollection<ClientAudio> brokerInputQueue = new BlockingCollection<ClientAudio>();
        // which is consumed by:
        private Task taskAudioBroker;
        // which publishes objects onto:
        private BlockingCollection<Tuple<List<string>, ClientAudio>> clientAudioPubInputQueue = new BlockingCollection<Tuple<List<string>, ClientAudio>>();
        // which is consumed by:
        private Task taskClientAudioPub;
        //---------------------------------------

        ServerStatistics serverStatistics;
        System.Timers.Timer _timer = null;

        Dictionary<string, Client> clients = new Dictionary<string, Client>();
        //Dictionary<string, ConferenceCall> conferenceCalls = new Dictionary<string, ConferenceCall>();
        Dictionary<string, List<string>> audioReceiversLookup = new Dictionary<string, List<string>>();       //Periodically computed hash table lookup for each client
        AdminInfo adminInfo = new AdminInfo();

        public void Start()
        {
            foreach (var fixedClient in ClientConfigurationManager.Instance.RadioFixedConfigs)
            {
                clients.Add(fixedClient.Callsign, Client.NewFixedClientOffline(fixedClient));
            }

            taskClientDataPub = new Task(() => TaskClientDataPub(cancelTokenSource.Token, clientDataPubInputQueue, "tcp://*:60000"), TaskCreationOptions.LongRunning);
            taskClientDataPub.Start();
            taskClientDataSub = new Task(() => TaskClientDataSub(cancelTokenSource.Token, clientDataPubInputQueue, "tcp://*:60001"), TaskCreationOptions.LongRunning);
            taskClientDataSub.Start();

            taskClientAudioPub = new Task(() => TaskClientAudioPub(cancelTokenSource.Token, clientAudioPubInputQueue, "tcp://*:60002"), TaskCreationOptions.LongRunning);
            taskClientAudioPub.Start();
            taskAudioBroker = new Task(() => TaskAudioBroker(cancelTokenSource.Token, brokerInputQueue, clientAudioPubInputQueue), TaskCreationOptions.LongRunning);
            taskAudioBroker.Start();
            taskClientAudioSub = new Task(() => TaskClientAudioSub(cancelTokenSource.Token, brokerInputQueue, "tcp://*:60003"), TaskCreationOptions.LongRunning);
            taskClientAudioSub.Start();

            serverStatistics = new ServerStatistics();
            serverStatistics.StartDateTime = DateTime.UtcNow;

            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 1000;
                _timer.Elapsed += _timer_Elapsed;
            }
            _timer.Start();
        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Go through and remove very old offline clients and tag recently disconnected clients as offline
            var clientKeys = clients.Keys.ToList();
            foreach (var clientKey in clientKeys)
            {
                if (clients[clientKey].LastSeenUTC < DateTime.UtcNow.AddSeconds(-120) && clients[clientKey].Type != ClientType.RadioFixed)
                    clients.Remove(clientKey);
            }
            foreach (var clientKey in clientKeys)
            {
                if (clients[clientKey].LastSeenUTC < DateTime.UtcNow.AddSeconds(-3))
                    clients[clientKey].Online = false;
            }

            //Now populate the admin info and send
            adminInfo.Clients = clients.Values.ToList();
            clientDataPubInputQueue.Add(adminInfo);

            //Go through the hash table and remove any offline clients
            var onlineClients = clients.Values.Where(c => c.Online == true);
            var onlineCallsigns = onlineClients.Select(c => c.Callsign).ToList();
            var hashTableCallsigns = audioReceiversLookup.Keys.ToList();

            foreach (var callsign in hashTableCallsigns)
                if (!onlineCallsigns.Contains(callsign))
                    audioReceiversLookup.Remove(callsign);       //Possible bug here - if malicious client continues to send audio data but no heartbeat is occurring, then the dict will try access this member in another thread.

            Console.WriteLine("Clients connected: {0}", onlineCallsigns.Count());

            //Recalculate receiving clients for each client
            foreach (var onlineClient in onlineClients)
            {
                //audioReceiversLookup[onlineClient.Callsign].Clear();
                audioReceiversLookup[onlineClient.Callsign] = onlineClient.GetReceivingClients(onlineClients.Where(c => c.Callsign != onlineClient.Callsign)).ToList();
            }
        }

        private void CheckAndAddIfNewClient(ClientHeartbeat clientHeartbeat)
        {
            if (clients.ContainsKey(clientHeartbeat.Callsign))
                clients[clientHeartbeat.Callsign].MobileClientOnline();
            else
                clients[clientHeartbeat.Callsign] = Client.NewMobileClientOnline(ClientConfigurationManager.Instance.DefaultRadioMobileConfig, clientHeartbeat.Callsign);
        }

        private void CheckAndAddIfNewClient(ClientPositionUpdate clientPositionUpdate)
        {
            if (clients.ContainsKey(clientPositionUpdate.Callsign))
                clients[clientPositionUpdate.Callsign].MobileClientOnline();
            else
                clients[clientPositionUpdate.Callsign] = Client.NewMobileClientOnline(ClientConfigurationManager.Instance.DefaultRadioMobileConfig, clientPositionUpdate.Callsign);
        }

        private void CheckAndAddIfNewClient(ClientFrequencyUpdate clientFrequencyUpdate)
        {
            if (clients.ContainsKey(clientFrequencyUpdate.Callsign))
                clients[clientFrequencyUpdate.Callsign].MobileClientOnline();
            else
                clients[clientFrequencyUpdate.Callsign] = Client.NewMobileClientOnline(ClientConfigurationManager.Instance.DefaultRadioMobileConfig, clientFrequencyUpdate.Callsign);
        }

        private void UpdateMobileClientPosition(ClientPositionUpdate clientPositionUpdate)
        {
            clients[clientPositionUpdate.Callsign].Transceivers[0].LatDeg = clientPositionUpdate.LatDeg;
            clients[clientPositionUpdate.Callsign].Transceivers[0].LonDeg = clientPositionUpdate.LonDeg;
            clients[clientPositionUpdate.Callsign].Transceivers[0].GroundAltMeters = clientPositionUpdate.GroundAltMeters;
        }

        private void UpdateMobileClientFrequency(ClientFrequencyUpdate clientFrequencyUpdate)
        {
            clients[clientFrequencyUpdate.Callsign].Frequency = clientFrequencyUpdate.Frequency;
        }

        private void TaskClientDataSub(CancellationToken cancelToken, BlockingCollection<object> queue, string bind)
        {
            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 50;
                subSocket.Bind(bind);
                subSocket.Subscribe("");

                while (!cancelToken.IsCancellationRequested)
                {
                    var messageTopicReceived = subSocket.ReceiveFrameString();
                    int bytesReceived = 0;
                    switch (messageTopicReceived)
                    {
                        case "ClientHeartbeat":
                            ClientHeartbeat clientHeartbeat = subSocket.Deserialise<ClientHeartbeat>(out bytesReceived);
                            serverStatistics.DataBytesReceived += bytesReceived;
                            CheckAndAddIfNewClient(clientHeartbeat);
                            break;
                        case "ClientPositionUpdate":
                            ClientPositionUpdate clientPositionUpdate = subSocket.Deserialise<ClientPositionUpdate>(out bytesReceived);
                            serverStatistics.DataBytesReceived += bytesReceived;
                            CheckAndAddIfNewClient(clientPositionUpdate);
                            UpdateMobileClientPosition(clientPositionUpdate);
                            break;
                        case "ClientFrequencyUpdate":
                            ClientFrequencyUpdate clientFrequencyUpdate = subSocket.Deserialise<ClientFrequencyUpdate>(out bytesReceived);
                            serverStatistics.DataBytesReceived += bytesReceived;
                            CheckAndAddIfNewClient(clientFrequencyUpdate);
                            UpdateMobileClientFrequency(clientFrequencyUpdate);
                            break;
                    }
                }
                taskClientDataSub = null;
            }
        }

        private void TaskClientDataPub(CancellationToken cancelToken, BlockingCollection<object> queue, string bind)
        {
            using (var pubSocket = new PublisherSocket())
            {
                pubSocket.Options.SendHighWatermark = 10;
                pubSocket.Bind(bind);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (queue.TryTake(out object data, 500))
                    {
                        switch (data.GetType().Name)
                        {
                            case "AdminInfo":
                                serverStatistics.DataBytesSent += pubSocket.Serialise<AdminInfo>(data);
                                break;
                        }
                    }
                }
            }
            taskClientDataPub = null;
        }

        private void TaskClientAudioSub(CancellationToken cancelToken, BlockingCollection<ClientAudio> outputQueue, string bind)
        {
            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 50;
                subSocket.Bind(bind);
                subSocket.Subscribe("");

                while (!cancelToken.IsCancellationRequested)
                {
                    var clientID = subSocket.ReceiveFrameString();
                    var audioData = subSocket.ReceiveFrameBytes();

                    serverStatistics.AudioBytesReceived += audioData.Length;
                    outputQueue.Add(new ClientAudio() { Callsign = clientID, Data = audioData });
                }
                taskClientDataSub = null;
            }
        }

        private void TaskAudioBroker(CancellationToken cancelToken, BlockingCollection<ClientAudio> inputQueue, BlockingCollection<Tuple<List<string>, ClientAudio>> outputQueue)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                if (inputQueue.TryTake(out ClientAudio data, 500))
                {
                    //This is where we filter and disseminate to multiple other clientIDs
                    if (audioReceiversLookup.ContainsKey(data.Callsign))
                    {
                        var receivingClientIDs = audioReceiversLookup[data.Callsign];
                        outputQueue.Add(new Tuple<List<string>, ClientAudio>(receivingClientIDs, data));

                    }
                }
            }
        }

        private void TaskClientAudioPub(CancellationToken cancelToken, BlockingCollection<Tuple<List<string>, ClientAudio>> inputQueue, string bind)
        {
            using (var pubSocket = new PublisherSocket())
            {
                pubSocket.Options.SendHighWatermark = 100;
                pubSocket.Bind(bind);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (inputQueue.TryTake(out Tuple<List<string>, ClientAudio> data, 500))
                    {
                        foreach (var clientID in data.Item1)
                            pubSocket.Serialise(clientID, data.Item2);
                    }
                }
            }
            taskClientAudioPub = null;
        }
    }

}
