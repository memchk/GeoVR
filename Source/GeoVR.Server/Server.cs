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
    public class Server
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

        List<ClientHeartbeatReception> clientHeartbeats = new List<ClientHeartbeatReception>();
        Dictionary<string, ClientPosition> clientPositions = new Dictionary<string, ClientPosition>();
        List<ClientRadioRadius> clientRadioRadii = new List<ClientRadioRadius>();

        AdminOneSecondInfo oneSecondInfo = new AdminOneSecondInfo();

        Dictionary<string, List<string>> clientReceivingClients = new Dictionary<string, List<string>>();

        public void Start()
        {
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
            clientHeartbeats = clientHeartbeats.Where(c => c.ReceivedUTC > DateTime.UtcNow.AddSeconds(-5)).ToList();       //Filter to clients who've sent heartbeats in the last 5 seconds.
            oneSecondInfo.ClientIDs = clientHeartbeats.Select(c => c.ClientID).ToList();
            oneSecondInfo.ClientPositions = clientPositions.Values.ToList();
            oneSecondInfo.ClientRadioRadii = clientRadioRadii;
            clientDataPubInputQueue.Add(oneSecondInfo);

            Console.WriteLine("Clients connected: {0}", oneSecondInfo.ClientIDs.Count);

            //Recalculate receiving clients for each client
            var clientIDs = clientReceivingClients.Keys.ToList();
            foreach (var localClientID in clientIDs)
            {
                clientReceivingClients[localClientID].Clear();      //Clear all receivers

                foreach (var remoteClientID in clientIDs.Where(c => c != localClientID))        //Get list of all potential receivers that isn't me
                {
                    var localClientPosition = clientPositions[localClientID];
                    var remoteClientPosition = clientPositions[remoteClientID];
                    var localClientRadioRadius = clientRadioRadii.First(c => c.ClientID == localClientID);
                    var remoteClientRadioRadius = clientRadioRadii.First(c => c.ClientID == remoteClientID);
                    if (DistanceTwoPoint(localClientPosition.LatDeg, localClientPosition.LonDeg, remoteClientPosition.LatDeg, remoteClientPosition.LonDeg) <
                        (localClientRadioRadius.TransmitRadiusM + remoteClientRadioRadius.ReceiveRadiusM))
                    {
                        clientReceivingClients[localClientID].Add(remoteClientID);
                    }
                }
            }
            //clientReceivingClients[client] = clientIDs.Where(c => c != client).ToList();  //Everyone can hear everyone else except themselves!
        }

        public static double DistanceTwoPoint(double startLat, double startLong, double endLat, double endLong)
        {

            var startPoint = new GeoCoordinate(startLat, startLong);
            var endPoint = new GeoCoordinate(endLat, endLong);

            return startPoint.GetDistanceTo(endPoint);
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
                            clientHeartbeats.RemoveAll(c => c.ClientID == clientHeartbeat.ClientID);
                            clientHeartbeats.Add(new ClientHeartbeatReception(clientHeartbeat));

                            if (!clientRadioRadii.Any(c => c.ClientID == clientHeartbeat.ClientID))
                                clientRadioRadii.Add(new ClientRadioRadius() { ClientID = clientHeartbeat.ClientID, ReceiveRadiusM = 80467 * 1.25, TransmitRadiusM = 80467 });

                            if (!clientReceivingClients.ContainsKey(clientHeartbeat.ClientID))
                                clientReceivingClients.Add(clientHeartbeat.ClientID, new List<string>());
                            break;
                        case "ClientPosition":
                            ClientPosition clientPosition = subSocket.Deserialise<ClientPosition>(out bytesReceived);
                            serverStatistics.DataBytesReceived += bytesReceived;
                            clientPositions[clientPosition.ClientID] = clientPosition;
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
                            case "AdminOneSecondInfo":
                                serverStatistics.DataBytesSent += pubSocket.Serialise<AdminOneSecondInfo>(data);
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
                    outputQueue.Add(new ClientAudio() { ClientID = clientID, Data = audioData });
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
                    if (clientReceivingClients.ContainsKey(data.ClientID))
                    {
                        var receivingClientIDs = clientReceivingClients[data.ClientID];
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
