using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Server
{
    public class ClientSubscriptionData
    {
        public int ClientID { get; set; }
        public List<ulong> TransmitGridReferences { get; set; }
        public List<ulong> ReceiveGridReferences { get; set; }
    }
}
