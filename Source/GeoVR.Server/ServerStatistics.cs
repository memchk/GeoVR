using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Server
{
    public class ServerStatistics
    {
        public DateTime StartDateTime { get; set; }
        public long AudioBytesSent { get; internal set; }
        public long AudioBytesReceived { get; internal set; }
        public long DataBytesSent { get; internal set; }
        public long DataBytesReceived { get; internal set; }
    }
}
