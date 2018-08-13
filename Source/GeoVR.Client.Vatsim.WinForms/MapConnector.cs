using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using LockheedMartin.Prepar3D.SimConnect;

namespace GeoVR.Client.VATSIM.WinForms
{
    public class MapConnector : SimConnector
    {
        #region Constructors
        /// <summary>
        /// Create a new simulation connection.
        /// </summary>
        /// <param name="connectionName">The name for this connection.</param>
        public MapConnector(string connectionName) : base(connectionName)
        {
        }
        #endregion

        #region Enums
        enum DATADEFINITIONS
        {
            UserPosition,
            AIPosition,
        }
        enum DataIdentifier
        {
            RetreiveUserObject = 0,
        }
        #endregion

        #region Structures
        /// <summary>
        /// Read User Position Structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct ReadUserPosStruct
        {
            public double Latitude;
            public double Longitude;
            public double HeightAboveGround;
            public Int32 ComFreq;
        }

        #endregion

        #region Events
            /// <summary>
            /// Fired off when a simobject request is received.
            /// </summary>
            public event EventHandler<GenericEventArgs<ReadUserPosStruct>> OnPositionReceived;
        #endregion

        #region SimConnector overrides
        /// <summary>
        /// Define the data structures that we need for talking to Prepar3D.
        /// </summary>
        protected override void DefineDataStructures()
        {
            // Define any base class data structures that are needed.
            base.DefineDataStructures();

            DefineStructures();
        }

        /// <summary>
        /// Called when the connector connects to Prepar3D.
        /// </summary>
        protected override void OnConnected()
        {
            base.OnConnected();

            // Get the SimObject data for this aircraft.
            GetSimObjectData();
        }

        /// <summary>
        /// Register for any events that need to be listened to.
        /// </summary>
        protected override void SimConnectEventRegistration()
        {
            // Register for the base classes desired events.
            base.SimConnectEventRegistration();

            // Register for the sim connect events we care about.
            simulation_connection.OnRecvSimobjectData += SimulationConnection_OnRecvSimobjectData;
        }

        

        #endregion

        #region Methods
        /// <summary>
        /// Defines the SimObject data structure that will be exchanged between this app and the Prepar3D simulation.
        /// </summary>
        private void DefineStructures()
        {
            simulation_connection.AddToDataDefinition(DATADEFINITIONS.UserPosition, "PLANE LATITUDE", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simulation_connection.AddToDataDefinition(DATADEFINITIONS.UserPosition, "PLANE LONGITUDE", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simulation_connection.AddToDataDefinition(DATADEFINITIONS.UserPosition, "PLANE ALT ABOVE GROUND", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simulation_connection.AddToDataDefinition(DATADEFINITIONS.UserPosition, "COM ACTIVE FREQUENCY:1", "Frequency BCD16", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simulation_connection.RegisterDataDefineStruct<ReadUserPosStruct>(DATADEFINITIONS.UserPosition);
        }

        /// <summary>
        /// Get the SimObject data for user at regular interval
        /// </summary>
        public void GetSimObjectData()
        {
            // Only do this if there is a valid simulation connection.
            if (simulation_connection != null)
            {
                try
                {
                    // Requests regular interval for simobject data
                    simulation_connection.RequestDataOnSimObject(DataIdentifier.RetreiveUserObject, DATADEFINITIONS.UserPosition, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.VISUAL_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 10, 0);
                }
                catch (Exception ex)
                {
                    SendError(ex.Message);
                }
            }
        }

        /// <summary>
        /// This is where the sim object data comes in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimulationConnection_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            // Determine what type of SimObject data we received by checking the data ID against our DataIdentifier Enum.
            switch ((DataIdentifier)data.dwRequestID)
            {
                case DataIdentifier.RetreiveUserObject:
                    ReadUserPosStruct position = (ReadUserPosStruct)data.dwData[0];
                    OnPositionReceived.SafeInvoke(this, new GenericEventArgs<MapConnector.ReadUserPosStruct>(position));
                    break;
            }
        }

        #endregion
    }
}
