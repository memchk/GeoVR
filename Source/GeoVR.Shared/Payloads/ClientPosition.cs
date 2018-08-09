using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class ClientPosition
    {
        public string ClientID { get; set; }
        public double LatDeg { get; set; }
        public double LonDeg { get; set; }
        public double GroundAltM { get; set; }
    }
}
