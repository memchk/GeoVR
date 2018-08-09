using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoVR.Shared;

namespace GeoVR.Server
{
    public class ClientHeartbeatServer : ClientHeartbeat
    {
        public DateTime ReceivedUTC { get; set; }

        public ClientHeartbeatServer(ClientHeartbeat clientHeartbeat)
        {
            Username = clientHeartbeat.Username;
            ReceivedUTC = DateTime.UtcNow;
        }
    }
}
