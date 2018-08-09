using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Client
{
    public class ClientStatistics
    {
        public long AudioBytesEncoded { get; internal set; }
        public long AudioBytesSent { get; internal set; }
        public long AudioBytesReceived { get; internal set; }
    }
}
