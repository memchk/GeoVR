using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class ClientRadioRadius       //This is sent from the server to clients to let them know what their radio radii are
    {
        public string ClientID { get; set; }
        public double ReceiveRadiusM { get; set; }
        public double TransmitRadiusM { get; set; }
    }
}
