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
        private BlockingCollection<Tuple<string, byte[]>> brokerInputQueue = new BlockingCollection<Tuple<string, byte[]>>();
        // which is consumed by:
        private Task taskAudioBroker;
        // which publishes objects onto:
        private BlockingCollection<Tuple<List<string>, byte[]>> clientAudioPubInputQueue = new BlockingCollection<Tuple<List<string>, byte[]>>();
        // which is consumed by:
        private Task taskClientAudioPub;
        //---------------------------------------

        long _bytesSent;
        long _bytesReceived;
        System.Timers.Timer _timer = null;
        DateTime _startTime;
        List<ClientHeartbeatOnServer> clientHeartbeats = new List<ClientHeartbeatOnServer>();
        OneSecondInfo oneSecondInfo = new OneSecondInfo();

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

            _startTime = DateTime.Now;
            _bytesSent = 0;
            _bytesReceived = 0;

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
            clientDataPubInputQueue.Add(oneSecondInfo);

            Console.WriteLine("Clients connected: {0}", oneSecondInfo.ClientIDs.Count);

            //Recalculate receiving clients for each client
            var clients = clientReceivingClients.Keys.ToList();
            foreach (var client in clients)
                clientReceivingClients[client] = clients.Where(c => c != client).ToList();  //Everyone can hear everyone else except themselves!
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
                    switch (messageTopicReceived)
                    {
                        case "ClientHeartbeat":
                            ClientHeartbeat clientHeartbeat = subSocket.Deserialise<ClientHeartbeat>();
                            clientHeartbeats.RemoveAll(c => c.ClientID == clientHeartbeat.ClientID);
                            clientHeartbeats.Add(new ClientHeartbeatOnServer(clientHeartbeat));
                            //Add to our dictionary
                            if (!clientReceivingClients.ContainsKey(clientHeartbeat.ClientID))
                                clientReceivingClients.Add(clientHeartbeat.ClientID, new List<string>());
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
                            case "OneSecondInfo":
                                _bytesSent += pubSocket.Serialise<OneSecondInfo>(data);
                                break;
                        }
                    }
                }
            }
            taskClientDataPub = null;
        }

        private void TaskClientAudioSub(CancellationToken cancelToken, BlockingCollection<Tuple<string, byte[]>> outputQueue, string bind)
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

                    //_bytesReceived += length;
                    outputQueue.Add(new Tuple<string, byte[]>(clientID, audioData));
                }
                taskClientDataSub = null;
            }
        }

        private void TaskAudioBroker(CancellationToken cancelToken, BlockingCollection<Tuple<string, byte[]>> inputQueue, BlockingCollection<Tuple<List<string>, byte[]>> outputQueue)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                if (inputQueue.TryTake(out Tuple<string, byte[]> data, 500))
                {
                    //This is where we filter and disseminate to multiple other clientIDs
                    if (clientReceivingClients.ContainsKey(data.Item1))
                    {
                        var receivingClientIDs = clientReceivingClients[data.Item1];
                        outputQueue.Add(new Tuple<List<string>, byte[]>(receivingClientIDs, data.Item2));

                    }
                }
            }
        }

        private void TaskClientAudioPub(CancellationToken cancelToken, BlockingCollection<Tuple<List<string>, byte[]>> inputQueue, string bind)
        {
            using (var pubSocket = new PublisherSocket())
            {
                pubSocket.Options.SendHighWatermark = 100;
                pubSocket.Bind(bind);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (inputQueue.TryTake(out Tuple<List<string>, byte[]> data, 500))
                    {
                        foreach (var clientID in data.Item1)
                            pubSocket.SendMoreFrame(clientID).SendFrame(data.Item2);
                    }
                }
            }
            taskClientAudioPub = null;
        }
    }

}
