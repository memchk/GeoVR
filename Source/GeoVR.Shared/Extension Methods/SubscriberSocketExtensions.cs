using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public static class SubscriberSocketExtensions
    {
        public static T Deserialise<T>(this SubscriberSocket subscriberSocket)
        {
            var messageReceived = subscriberSocket.ReceiveFrameBytes();
            using (BsonReader reader = new BsonReader(new MemoryStream(messageReceived)))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        } 

        public static T Deserialise<T>(this SubscriberSocket subscriberSocket, out int length)
        {
            var messageReceived = subscriberSocket.ReceiveFrameBytes();
            length = messageReceived.Length;
            using (BsonReader reader = new BsonReader(new MemoryStream(messageReceived)))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
