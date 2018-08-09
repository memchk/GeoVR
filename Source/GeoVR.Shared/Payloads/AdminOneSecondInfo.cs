using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class AdminOneSecondInfo
    {
        public List<string> ClientIDs { get; set; }
        public List<ClientPosition> ClientPositions { get; set; }
        public List<ClientRadioRadius> ClientRadioRadii { get; set; }

        public AdminOneSecondInfo()
        {
            ClientIDs = new List<string>();
            ClientPositions = new List<ClientPosition>();
            ClientRadioRadii = new List<ClientRadioRadius>();
        }
    }
}
