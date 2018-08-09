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
    public static class PublisherSocketExtensions
    {
        public static long Serialise<T>(this PublisherSocket publisherSocket, object data)
        {
            T typedData = (T)data;
            MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, typedData);
                publisherSocket.SendMoreFrame(typedData.GetType().Name).SendFrame(ms.ToArray());
                return ms.Length;
            }
        }
    }
}
