using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoVR.Shared;

namespace GeoVR.Server
{
    public class ClientHeartbeatOnServer : ClientHeartbeat
    {
        public DateTime ReceivedUTC { get; set; }

        public ClientHeartbeatOnServer(ClientHeartbeat clientHeartbeat)
        {
            ClientID = clientHeartbeat.ClientID;
            ReceivedUTC = DateTime.UtcNow;
        }
    }
}
