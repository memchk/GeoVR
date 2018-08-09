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
        private Task taskClientSub;
        private Task taskClientPub;
        private BlockingCollection<object> clientTransmitQueue = new BlockingCollection<object>();
        long _bytesSent;
        long _bytesReceived;
        System.Timers.Timer _timer = null;
        DateTime _startTime;
        List<ClientHeartbeatServer> clientHeartbeats = new List<ClientHeartbeatServer>();
        OneSecondInfo oneSecondInfo = new OneSecondInfo();

        public void Start()
        {
            taskClientPub = new Task(() => TaskClientPub(cancelTokenSource.Token, clientTransmitQueue, "tcp://*:60000"), TaskCreationOptions.LongRunning);
            taskClientPub.Start();
            taskClientSub = new Task(() => TaskClientSub(cancelTokenSource.Token, clientTransmitQueue, "tcp://*:60001"), TaskCreationOptions.LongRunning);
            taskClientSub.Start();


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
            clientHeartbeats = clientHeartbeats.Where(c => c.ReceivedUTC > DateTime.UtcNow.AddSeconds(-5)).ToList();       //Filter to clients who've sent heartbeats in the last 10 seconds.
            oneSecondInfo.Usernames = clientHeartbeats.Select(c => c.Username).ToList();
            clientTransmitQueue.Add(oneSecondInfo);

            Console.WriteLine("Clients connected: {0}", oneSecondInfo.Usernames.Count);
        }

        private void TaskClientSub(CancellationToken cancelToken, BlockingCollection<object> queue, string bind)
        {
            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 100;
                subSocket.Bind(bind);
                subSocket.Subscribe("");

                while (!cancelToken.IsCancellationRequested)
                {
                    var messageTopicReceived = subSocket.ReceiveFrameString();
                    switch (messageTopicReceived)
                    {
                        case "ClientAudioData":
                            int length = 0;
                             ClientAudioData clientAudioData = subSocket.Deserialise<ClientAudioData>(out length);
                            _bytesReceived += length;
                            queue.Add(clientAudioData);
                            if (!oneSecondInfo.Usernames.Contains(clientAudioData.ClientName))
                                oneSecondInfo.Usernames.Add(clientAudioData.ClientName);
                            break;
                        case "ClientHeartbeat":
                            ClientHeartbeat clientHeartbeat = subSocket.Deserialise<ClientHeartbeat>();
                            clientHeartbeats.RemoveAll(c => c.Username == clientHeartbeat.Username);
                            clientHeartbeats.Add(new ClientHeartbeatServer(clientHeartbeat));                          
                            break;
                    }
                }
                taskClientSub = null;
            }
        }

        private void TaskClientPub(CancellationToken cancelToken, BlockingCollection<object> queue, string bind)
        {
            using (var pubSocket = new PublisherSocket())
            {
                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind(bind);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (queue.TryTake(out object data, 500))
                    {
                        switch (data.GetType().Name)
                        {
                            case "ClientAudioData":
                                _bytesSent += pubSocket.Serialise<ClientAudioData>(data);
                                break;
                            case "OneSecondInfo":
                                _bytesSent += pubSocket.Serialise<OneSecondInfo>(data);
                                break;
                        }
                    }
                }
            }
            taskClientPub = null;
        }
    }

}
