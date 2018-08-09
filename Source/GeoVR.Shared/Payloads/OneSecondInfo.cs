using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class OneSecondInfo
    {
        public List<string> ClientIDs { get; set; }

        public OneSecondInfo()
        {
            ClientIDs = new List<string>();
        }
    }
}
