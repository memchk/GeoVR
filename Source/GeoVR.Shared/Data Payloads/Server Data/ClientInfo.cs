using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class ClientInfo     //This is periodically sent to non-admin users, so the radio radius can be displayed
    {
        public string Callsign { get; set; }
        public RadioTransceiver Transceiver { get; set; }
    }
}
