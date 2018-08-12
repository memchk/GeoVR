using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class AdminInfo      //This is periodically sent to admin users
    {
        public List<Client> Clients { get; set; }

        public AdminInfo()
        {
            Clients = new List<Client>();
        }
    }
}
