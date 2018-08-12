using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class ClientConfig
    {
        public ClientType Type { get; set; }
        public string Callsign { get; set; }
        public string Frequency { get; set; }
        public List<RadioTransceiver> Transceivers { get; set; }
    }
}
