using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Server.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            Console.WriteLine("Server started");
            Console.ReadKey();
            server.Stop();
        }
    }
}
