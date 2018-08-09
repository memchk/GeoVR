using FragLabs.Audio.Codecs;
using NAudio.Wave;
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

namespace GeoVR.Client
{
    public class Client
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private Task taskDataPub;
        private Task taskDataSub;
        private Task taskAudioPub;
        private Task taskAudioSub;
        private Task taskAudioPlayback;
        private BlockingCollection<object> dataPublishInputQueue = new BlockingCollection<object>();
        private BlockingCollection<byte[]> audioPublishInputQueue = new BlockingCollection<byte[]>();
        private BlockingCollection<byte[]> audioPlaybackQueue = new BlockingCollection<byte[]>();

        WaveIn _waveIn;
        WaveOut _waveOut;
        BufferedWaveProvider _playBuffer;
        OpusEncoder _encoder;
        OpusDecoder _decoder;
        int _segmentFrames;
        int _bytesPerSegment;
        long _bytesEncoded;
        long _bytesSent;
        long _bytesReceived;
        DateTime _startTime;
        System.Timers.Timer _timer = null;
        bool _ptt = false;

        public long BytesEncoded { get { return _bytesEncoded; } }
        public long BytesSent { get { return _bytesSent; } }
        public long BytesReceived { get { return _bytesReceived; } }

        private string _lastUser;
        public string LastUser { get { return _lastUser; } }
        public OneSecondInfo LastReceivedOneSecondInfo { get; set; }

        private string _username;

        public void Start(string ipAddress, string username)
        {
            taskDataPub = new Task(() => TaskDataPub(cancelTokenSource.Token, dataPublishInputQueue, "tcp://" + ipAddress + ":60001"), TaskCreationOptions.LongRunning);
            taskDataPub.Start();
            taskDataSub = new Task(() => TaskDataSub(cancelTokenSource.Token, audioPlaybackQueue, "tcp://" + ipAddress + ":60000"), TaskCreationOptions.LongRunning);
            taskDataSub.Start();
            taskAudioPub = new Task(() => TaskAudioPub(cancelTokenSource.Token, audioPublishInputQueue, "tcp://" + ipAddress + ":60003"), TaskCreationOptions.LongRunning);
            taskAudioPub.Start();
            taskAudioSub = new Task(() => TaskAudioSub(cancelTokenSource.Token, audioPlaybackQueue, "tcp://" + ipAddress + ":60002"), TaskCreationOptions.LongRunning);
            taskAudioSub.Start();

            LastReceivedOneSecondInfo = new OneSecondInfo();
            _startTime = DateTime.Now;
            _bytesEncoded = 0;
            _bytesSent = 0;
            _bytesReceived = 0;
            _username = username;
            _segmentFrames = 960;
            _encoder = OpusEncoder.Create(48000, 1, FragLabs.Audio.Codecs.Opus.Application.Voip);
            _encoder.Bitrate = 65536;
            _decoder = OpusDecoder.Create(48000, 1);
            _bytesPerSegment = _encoder.FrameByteCount(_segmentFrames);

            _waveIn = new WaveIn(WaveCallbackInfo.FunctionCallback());
            _waveIn.BufferMilliseconds = 50;
            Console.WriteLine("Input device: " + WaveIn.GetCapabilities(0).ProductName);
            _waveIn.DeviceNumber = 0;
            _waveIn.DataAvailable += _waveIn_DataAvailable;
            _waveIn.WaveFormat = new WaveFormat(48000, 16, 1);

            _playBuffer = new BufferedWaveProvider(new WaveFormat(48000, 16, 1));
            taskAudioPlayback = new Task(() => TaskAudioPlayback(cancelTokenSource.Token, audioPlaybackQueue), TaskCreationOptions.LongRunning);
            taskAudioPlayback.Start();

            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            Console.WriteLine("Output device: " + WaveOut.GetCapabilities(0).ProductName);
            _waveOut.DeviceNumber = 0;
            _waveOut.DesiredLatency = 300;      //Default is 300
            _waveOut.Init(_playBuffer);

            _waveOut.Play();
            _waveIn.StartRecording();

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
            _timer.Stop();
            _waveIn.StopRecording();
            _waveIn.Dispose();
            _waveIn = null;
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
            _playBuffer = null;
            _encoder.Dispose();
            _encoder = null;
            _decoder.Dispose();
            _decoder = null;
            cancelTokenSource.Cancel();
        }

        public void PTT(bool ptt)
        {
            _ptt = ptt;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeDiff = DateTime.Now - _startTime;
            var bpsSend = _bytesSent / timeDiff.TotalSeconds;
            var bpsReceive = _bytesReceived / timeDiff.TotalSeconds;
            Console.WriteLine("Send rate: {0:N1} B/s, Receive rate: {1:N1} B/s", bpsSend, bpsReceive);
            dataPublishInputQueue.Add(new ClientHeartbeat() { ClientID = _username });
        }

        byte[] _notEncodedBuffer = new byte[0];
        void _waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] soundBuffer = new byte[e.BytesRecorded + _notEncodedBuffer.Length];
            for (int i = 0; i < _notEncodedBuffer.Length; i++)
                soundBuffer[i] = _notEncodedBuffer[i];
            for (int i = 0; i < e.BytesRecorded; i++)
                soundBuffer[i + _notEncodedBuffer.Length] = e.Buffer[i];

            int byteCap = _bytesPerSegment;
            int segmentCount = (int)Math.Floor((decimal)soundBuffer.Length / byteCap);
            int segmentsEnd = segmentCount * byteCap;
            int notEncodedCount = soundBuffer.Length - segmentsEnd;
            _notEncodedBuffer = new byte[notEncodedCount];
            for (int i = 0; i < notEncodedCount; i++)
            {
                _notEncodedBuffer[i] = soundBuffer[segmentsEnd + i];
            }

            for (int i = 0; i < segmentCount; i++)
            {
                byte[] segment = new byte[byteCap];
                for (int j = 0; j < segment.Length; j++)
                    segment[j] = soundBuffer[(i * byteCap) + j];
                int len;
                byte[] buff = _encoder.Encode(segment, segment.Length, out len);
                _bytesEncoded += len;
                //buff = _decoder.Decode(buff, len, out len);
                //_playBuffer.AddSamples(buff, 0, len);
                if (_ptt)
                {
                    byte[] trimmedBuff = new byte[len];
                    Buffer.BlockCopy(buff, 0, trimmedBuff, 0, len);
                    audioPublishInputQueue.Add(trimmedBuff);
                }
            }
        }

        private void TaskDataPub(CancellationToken cancelToken, BlockingCollection<object> queue, string address)
        {
            using (var pubSocket = new PublisherSocket())
            {
                //pubSocket.Options.SendHighWatermark = 1;
                pubSocket.Connect(address);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (queue.TryTake(out object data, 500))
                    {
                        switch (data.GetType().Name)
                        {
                            case "ClientHeartbeat":
                                pubSocket.Serialise<ClientHeartbeat>(data);
                                break;
                        }
                    }
                }
            }
            taskDataPub = null;
        }

        private void TaskDataSub(CancellationToken cancelToken, BlockingCollection<byte[]> playbackQueue, string address)
        {
            //var lastTransmitTime = DateTime.UtcNow;
            //string lastTransmitUser = "";

            using (var subSocket = new SubscriberSocket())
            {
                //subSocket.Options.ReceiveHighWatermark = 10;
                subSocket.Connect(address);
                subSocket.Subscribe("");

                while (!cancelToken.IsCancellationRequested)
                {
                    var messageTopicReceived = subSocket.ReceiveFrameString();
                    //var messageReceived = subSocket.ReceiveFrameBytes();

                    switch (messageTopicReceived)
                    {
                        //case "ClientAudioData":
                        //    int length = 0;
                        //    ClientAudioData clientAudioData = subSocket.Deserialise<ClientAudioData>(out length);
                        //    _bytesReceived += length;
                        //    if (clientAudioData.ClientID != _username)  //Don't loopback audio
                        //    {
                        //        if (clientAudioData.ClientID == lastTransmitUser)
                        //        {
                        //            _lastUser = clientAudioData.ClientID;
                        //            lastTransmitTime = DateTime.UtcNow;
                        //            lastTransmitUser = clientAudioData.ClientID;
                        //            playbackQueue.Add(clientAudioData.Data);
                        //        }
                        //        else if (DateTime.UtcNow > lastTransmitTime.AddMilliseconds(200))
                        //        {
                        //            _lastUser = clientAudioData.ClientID;
                        //            lastTransmitTime = DateTime.UtcNow;
                        //            lastTransmitUser = clientAudioData.ClientID;
                        //            playbackQueue.Add(clientAudioData.Data);
                        //        }
                        //    }
                        //    break;
                        case "OneSecondInfo":
                            LastReceivedOneSecondInfo = subSocket.Deserialise<OneSecondInfo>();
                            break;
                    }
                }
            }
            taskDataSub = null;
        }

        private void TaskAudioPub(CancellationToken cancelToken, BlockingCollection<byte[]> inputQueue, string address)
        {
            string usernameCache = _username;
            using (var pubSocket = new PublisherSocket())
            {
                //pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Connect(address);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (inputQueue.TryTake(out byte[] data, 500))
                    {
                        pubSocket.SendMoreFrame(usernameCache).SendFrame(data);
                        _bytesSent += data.Length;
                    }
                }
            }
            taskAudioPub = null;
        }

        private void TaskAudioSub(CancellationToken cancelToken, BlockingCollection<byte[]> outputQueue, string address)
        {
            using (var subSocket = new SubscriberSocket())
            {
                //subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect(address);
                subSocket.Subscribe(_username);

                while (!cancelToken.IsCancellationRequested)
                {
                    var messageTopicReceived = subSocket.ReceiveFrameString();      //Should always == _username
                    var messageReceived = subSocket.ReceiveFrameBytes();
                    _bytesReceived += messageReceived.Length;
                    outputQueue.Add(messageReceived);
                }
            }
            taskAudioSub = null;
        }

        private void TaskAudioPlayback(CancellationToken cancelToken, BlockingCollection<byte[]> queue)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                if (queue.TryTake(out byte[] data, 500))
                {
                    byte[] decoded = _decoder.Decode(data, data.Length, out int decodedLength);
                    _playBuffer.AddSamples(decoded, 0, decodedLength);
                }
            }
            taskAudioPlayback = null;
        }
    }
}
