using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Client
{
    public class ClientMixerInput
    {
        public string Callsign { get; set; }
        public bool InUse { get; set; }
        public BufferedWaveProvider Provider { get; set; }
        public DateTime LastUsedUTC { get; set; }
    }
}
