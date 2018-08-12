using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public class Client : ClientConfig
    {
        public bool Online { get; set; }
        public DateTime LastSeenUTC { get; set; }

        public void MobileClientOnline()
        {
            Online = true;
            LastSeenUTC = DateTime.Now;
        }

        public IEnumerable<string> GetReceivingClients(IEnumerable<Client> remoteClients)
        {
            foreach(var remoteClient in remoteClients)
            {
                if (IsClientWithinRange(remoteClient))
                    yield return remoteClient.Callsign;
            }
        }

        private bool IsClientWithinRange(Client remoteClient)
        {
            switch (Type)
            {
                case ClientType.VoipRoom:
                    if (remoteClient.Type == ClientType.VoipRoom & remoteClient.Frequency == Frequency)
                        return true;
                    else
                        return false;
                case ClientType.RadioMobile:
                case ClientType.RadioFixed:
                    foreach (var localTransceiver in Transceivers)
                    {
                        foreach (var remoteTransceiver in remoteClient.Transceivers)
                        {
                            var distance = Math.DistanceBetweenTwoPointsMeters(localTransceiver.LatDeg, localTransceiver.LonDeg, remoteTransceiver.LatDeg, remoteTransceiver.LonDeg);
                            if (distance < (localTransceiver.TransmitRadiusMeters + remoteTransceiver.ReceiveRadiusMeters))
                                return true;
                        }
                    }
                    return false;
                default:
                    throw new Exception("Unknown client type.");
            }
        }

        public static Client NewMobileClientOnline(ClientConfig config, string callsign)
        {
            return new Client()
            {
                Type = ClientType.RadioMobile,
                Callsign = callsign,
                Frequency = config.Frequency,
                Transceivers = config.Transceivers,
                Online = true,
                LastSeenUTC = DateTime.UtcNow
            };
        }

        public static Client NewFixedClientOffline(ClientConfig config)
        {
            return new Client()
            {
                Type = ClientType.RadioFixed,
                Callsign = config.Callsign,
                Frequency = config.Frequency,
                Transceivers = config.Transceivers,
                Online = false,
                LastSeenUTC = DateTime.UtcNow
            };
        }
    }
}
