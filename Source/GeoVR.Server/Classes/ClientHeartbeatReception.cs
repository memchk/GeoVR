using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoVR.Shared;

namespace GeoVR.Server
{
    public class ClientHeartbeatReception : ClientHeartbeat
    {
        public DateTime ReceivedUTC { get; set; }

        public ClientHeartbeatReception(ClientHeartbeat clientHeartbeat)
        {
            ClientID = clientHeartbeat.ClientID;
            ReceivedUTC = DateTime.UtcNow;
        }
    }
}
