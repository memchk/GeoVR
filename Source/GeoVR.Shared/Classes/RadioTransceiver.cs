using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class RadioTransceiver
    {
        public double ReceiveRadiusMeters { get; set; }
        public double TransmitRadiusMeters { get; set; }
        public double LatDeg { get; set; }
        public double LonDeg { get; set; }
        public double GroundAltMeters { get; set; }

        public static RadioTransceiver MobileClientDefault()
        {
            return new RadioTransceiver()
            {
                ReceiveRadiusMeters = 50 * Consts.MilesToMeters * 1.25,
                TransmitRadiusMeters = 50 * Consts.MilesToMeters,
                LatDeg = 0,
                LonDeg = 0,
                GroundAltMeters = 0
            };
        }
    }
}
