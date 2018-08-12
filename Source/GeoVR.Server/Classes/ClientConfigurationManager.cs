using GeoVR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Server
{
    public class ClientConfigurationManager
    {
        //Singleton Pattern
        private static readonly Lazy<ClientConfigurationManager> lazy = new Lazy<ClientConfigurationManager>(() => new ClientConfigurationManager());

        public static ClientConfigurationManager Instance { get { return lazy.Value; } }

        private ClientConfigurationManager()
        {
            RadioFixedConfigs = new List<ClientConfig>()
            {
                new ClientConfig()
                {
                    Callsign = "EGLL_2_GND",
                                                Frequency = "121.700",
                    Transceivers = new List<RadioTransceiver>()
                    {
                        new RadioTransceiver() {
                            ReceiveRadiusMeters = 50 * Consts.MilesToMeters,
                            TransmitRadiusMeters = 50 * Consts.MilesToMeters,
                            LatDeg = 51.470020,
                            LonDeg = -0.454295,
                            GroundAltMeters = 0
                        }
                    }
                },
                new ClientConfig()
                {
                    Callsign = "EGLL_M_GND",
                                                Frequency = "121.700",
                    Transceivers = new List<RadioTransceiver>()
                    {
                        new RadioTransceiver() {

                            ReceiveRadiusMeters = 50 * Consts.MilesToMeters,
                            TransmitRadiusMeters = 50 * Consts.MilesToMeters,
                            LatDeg = 51.470020,
                            LonDeg = -0.454295,
                            GroundAltMeters = 0
                        }
                    }
                },
            };
            VoipRoomConfigs = new List<ClientConfig>();
        }
        //End of Singleton Pattern

        // Default settings if there is no matching ClientID in the configuration
        public ClientConfig DefaultRadioMobileConfig
        {
            get
            {
                return new ClientConfig()
                {
                    Callsign = string.Empty,
                    Transceivers = new List<RadioTransceiver>() { RadioTransceiver.MobileClientDefault() }
                };
            }
        }
        // All the allowable fixed positions
        public List<ClientConfig> RadioFixedConfigs { get; set; }
        // All the conference call rooms
        public List<ClientConfig> VoipRoomConfigs { get; set; }
    }
}
