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
    public class ClientManager
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private Task taskDataPub;
        private Task taskDataSub;
        private Task taskAudioPub;
        private Task taskAudioSub;
        private Task taskAudioPlayback;
        private BlockingCollection<object> dataPublishInputQueue = new BlockingCollection<object>();
        private BlockingCollection<byte[]> audioPublishInputQueue = new BlockingCollection<byte[]>();
        private BlockingCollection<ClientAudio> audioPlaybackQueue = new BlockingCollection<ClientAudio>();

        WaveIn _waveIn;
        WaveOut _waveOut;
        BufferedWaveProvider _playBuffer;
        OpusEncoder _encoder;
        OpusDecoder _decoder;
        int _segmentFrames;
        int _bytesPerSegment;
        public ClientStatistics ClientStatistics { get; private set; }
        DateTime _startTime;
        System.Timers.Timer _timer = null;
        
        public AdminInfo LastReceivedAdminInfo { get; set; }

        private string callsign;
        private bool ptt = false;
        private bool started = false;
        private ClientPositionUpdate _lastClientPosition;
        private ClientFrequencyUpdate _lastClientFrequencyUpdate;

        public ClientManager()
        {
            LastReceivedAdminInfo = new AdminInfo();
            ClientStatistics = new ClientStatistics();
        }

        public void Start(string serverIpAddress, string clientID, string inputDevice, string outputDevice)
        {           
            _startTime = DateTime.Now;            
            callsign = clientID;

            taskDataPub = new Task(() => TaskDataPub(cancelTokenSource.Token, dataPublishInputQueue, "tcp://" + serverIpAddress + ":60001"), TaskCreationOptions.LongRunning);
            taskDataPub.Start();
            taskDataSub = new Task(() => TaskDataSub(cancelTokenSource.Token, "tcp://" + serverIpAddress + ":60000"), TaskCreationOptions.LongRunning);
            taskDataSub.Start();
            taskAudioPub = new Task(() => TaskAudioPub(cancelTokenSource.Token, audioPublishInputQueue, "tcp://" + serverIpAddress + ":60003"), TaskCreationOptions.LongRunning);
            taskAudioPub.Start();
            taskAudioSub = new Task(() => TaskAudioSub(cancelTokenSource.Token, audioPlaybackQueue, "tcp://" + serverIpAddress + ":60002"), TaskCreationOptions.LongRunning);
            taskAudioSub.Start();

            _segmentFrames = 960;
            _encoder = OpusEncoder.Create(48000, 1, FragLabs.Audio.Codecs.Opus.Application.Voip);
            _encoder.Bitrate = 65536;
            _decoder = OpusDecoder.Create(48000, 1);
            _bytesPerSegment = _encoder.FrameByteCount(_segmentFrames);

            _waveIn = new WaveIn(WaveCallbackInfo.FunctionCallback());
            _waveIn.BufferMilliseconds = 50;
            Console.WriteLine("Input device: " + WaveIn.GetCapabilities(0).ProductName);
            _waveIn.DeviceNumber = MapInputDevice(inputDevice);
            _waveIn.DataAvailable += _waveIn_DataAvailable;
            _waveIn.WaveFormat = new WaveFormat(48000, 16, 1);

            _playBuffer = new BufferedWaveProvider(new WaveFormat(48000, 16, 1));
            taskAudioPlayback = new Task(() => TaskAudioPlayback(cancelTokenSource.Token, audioPlaybackQueue), TaskCreationOptions.LongRunning);
            taskAudioPlayback.Start();

            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            Console.WriteLine("Output device: " + WaveOut.GetCapabilities(0).ProductName);
            _waveOut.DeviceNumber = MapOutputDevice(outputDevice);
            _waveOut.DesiredLatency = 200;      //Default is 300
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

            started = true;
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
            taskDataPub.Wait();
            taskDataSub.Wait();
            taskAudioPub.Wait();
            taskAudioSub.Wait();
            taskAudioPlayback.Wait();
            started = false;
        }

        public bool PTT(bool ptt)
        {
            if (started)
            {
                this.ptt = ptt;
                return true;
            }
            else
            {
                this.ptt = false;
                return false;
            }
        }

        public void Frequency(string frequency)
        {
            _lastClientFrequencyUpdate = new ClientFrequencyUpdate() { Callsign = callsign, Frequency = frequency };
            if (started)
                dataPublishInputQueue.Add(_lastClientFrequencyUpdate);         //Server will just ignore if it's a fixed position
        }

        public void Position(double latDeg, double lonDeg, double groundAltM)
        {
            _lastClientPosition = new ClientPositionUpdate() { Callsign = callsign, LatDeg = latDeg, LonDeg = lonDeg, GroundAltMeters = groundAltM };
            if (started)
                dataPublishInputQueue.Add(_lastClientPosition);         //Server will just ignore if it's a fixed position
        }

        public IEnumerable<string> GetInputDevices()
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                yield return WaveIn.GetCapabilities(i).ProductName;
            }
        }

        public IEnumerable<string> GetOutputDevices()
        {
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                yield return WaveOut.GetCapabilities(i).ProductName;
            }
        }

        private int MapInputDevice(string inputDevice)
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                if (WaveIn.GetCapabilities(i).ProductName == inputDevice)
                    return i;
            }
            return 0;       //Else use default
        }

        private int MapOutputDevice(string inputDevice)
        {
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                if (WaveOut.GetCapabilities(i).ProductName == inputDevice)
                    return i;
            }
            return 0;       //Else use default
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeDiff = DateTime.Now - _startTime;
            var bpsSend = ClientStatistics.AudioBytesSent / timeDiff.TotalSeconds;
            var bpsReceive = ClientStatistics.AudioBytesReceived / timeDiff.TotalSeconds;
            Console.WriteLine("Send rate: {0:N1} B/s, Receive rate: {1:N1} B/s", bpsSend, bpsReceive);
            dataPublishInputQueue.Add(new ClientHeartbeat() { Callsign = callsign });

            if (_lastClientPosition != null)                        //If the server is restarted, this will resync it
                dataPublishInputQueue.Add(_lastClientPosition);

            if (_lastClientFrequencyUpdate != null)                 //If the server is restarted, this will resync it
                dataPublishInputQueue.Add(_lastClientFrequencyUpdate);
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
            int segmentCount = (int)System.Math.Floor((decimal)soundBuffer.Length / byteCap);
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
                ClientStatistics.AudioBytesEncoded += len;
                //buff = _decoder.Decode(buff, len, out len);
                //_playBuffer.AddSamples(buff, 0, len);
                if (ptt & started)
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
                            case "ClientPositionUpdate":
                                pubSocket.Serialise<ClientPositionUpdate>(data);
                                break;
                            case "ClientFrequencyUpdate":
                                pubSocket.Serialise<ClientFrequencyUpdate>(data);
                                break;
                        }
                    }
                }
            }
            taskDataPub = null;
        }

        private void TaskDataSub(CancellationToken cancelToken, string address)
        {
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
                        case "AdminInfo":
                            LastReceivedAdminInfo = subSocket.Deserialise<AdminInfo>();
                            break;
                    }
                }
            }
            taskDataSub = null;
        }

        private void TaskAudioPub(CancellationToken cancelToken, BlockingCollection<byte[]> inputQueue, string address)
        {
            string usernameCache = callsign;
            using (var pubSocket = new PublisherSocket())
            {
                //pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Connect(address);

                while (!cancelToken.IsCancellationRequested)
                {
                    if (inputQueue.TryTake(out byte[] data, 500))
                    {
                        pubSocket.SendMoreFrame(usernameCache).SendFrame(data);
                        ClientStatistics.AudioBytesSent += data.Length;
                    }
                }
            }
            taskAudioPub = null;
        }

        private void TaskAudioSub(CancellationToken cancelToken, BlockingCollection<ClientAudio> outputQueue, string address)
        {
            using (var subSocket = new SubscriberSocket())
            {
                //subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect(address);
                subSocket.Subscribe(callsign);

                while (!cancelToken.IsCancellationRequested)
                {
                    var messageTopicReceived = subSocket.ReceiveFrameString();      //Should always == _username
                    int bytesReceived = 0;
                    ClientAudio clientAudio = subSocket.Deserialise<ClientAudio>(out bytesReceived);
                    ClientStatistics.AudioBytesReceived += bytesReceived;
                    outputQueue.Add(clientAudio);
                }
            }
            taskAudioSub = null;
        }

        private void TaskAudioPlayback(CancellationToken cancelToken, BlockingCollection<ClientAudio> queue)
        {
            var lastTransmitTime = DateTime.UtcNow;
            string lastTransmitClientID = "";

            while (!cancelToken.IsCancellationRequested)
            {
                if (queue.TryTake(out ClientAudio data, 500))       //We can use the clientID to input into different mixer channels later
                {
                    if ((data.Callsign == lastTransmitClientID) || (DateTime.UtcNow > lastTransmitTime.AddMilliseconds(200)))
                    {
                        lastTransmitTime = DateTime.UtcNow;
                        lastTransmitClientID = data.Callsign;
                        byte[] decoded = _decoder.Decode(data.Data, data.Data.Length, out int decodedLength);
                        _playBuffer.AddSamples(decoded, 0, decodedLength);
                    }
                }
            }
            taskAudioPlayback = null;
        }
    }
}
