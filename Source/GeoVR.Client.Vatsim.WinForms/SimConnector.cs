using System;
using System.Collections.Generic;
using System.Threading;
using LockheedMartin.Prepar3D.SimConnect;


namespace GeoVR.Client.VATSIM.WinForms
{
    public class SimConnector
    {
        #region Events
        /// <summary>
        /// Fires off when this connects to the simulation.
        /// </summary>
        public event EventHandler<EventArgs> Connected;

        /// <summary>
        /// Fires off when this disconnects from the simulation.
        /// </summary>
        public event EventHandler<EventArgs> Disconnected;

        /// <summary>
        /// Fires off when an error has occured.
        /// </summary>
        public event EventHandler<MessageEventArgs> Error;
        #endregion

        #region Fields
        /// <summary>
        /// The name for this sim connect connection.
        /// </summary>
        private string connection_name;

        /// <summary>
        /// The connection to the Prepar3D simulator.
        /// </summary>
        protected SimConnect simulation_connection;

        /// <summary>
        /// This helps time the messages from sim connect so we can read when
        /// the message thread is no longer blocked. This is what blocks and waits for a signal.
        /// </summary>
        private EventWaitHandle message_pump;

        /// <summary>
        /// The thread that will constantly run and pump messages for us.
        /// </summary>
        private Thread message_thread;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new simulation connection.
        /// </summary>
        /// <param name="connectionName">The name for this connection.</param>
        public SimConnector(string connectionName)
	    {
            // Store the name for this simulation connection.
            connection_name = connectionName;

            // Set the simulation connection members to their default values.
            simulation_connection = null;
            message_pump = null;
            message_thread = null;
	    }
        #endregion

        #region Methods
        /// <summary>
        /// Connects to the Prepar3D simulation.
        /// </summary>
        public void Connect()
        {
            // Try to start the SimConnect connection.
            try
            {
                // If we are already connected, then don't do anything.
                if (!IsConnectionActive())
                {
                    // Create a simulation connection. Since we are using WPF we should create a
                    // threaded event message pump that can easily get the data coming from SimConnect.
                    message_pump = new EventWaitHandle(false, EventResetMode.AutoReset);
                    simulation_connection = new SimConnect(connection_name, IntPtr.Zero, 0, message_pump, 0);

                    // Register for the events and data structures we care about.
                    SimConnectEventRegistration();
                    DefineDataStructures();

                    // Now create a thread to handle to handle the message processing and
                    // run it as a background thread.
                    message_thread = new Thread(new ThreadStart(MessageProcessor));
                    message_thread.IsBackground = true;
                    message_thread.Start();
                }
            }
            catch (Exception ex)
            {
                // Throw an Error explaining what happened.
                SendError(ex.Message);
            }
        }

        /// <summary>
        /// Disconnects from the Prepa3D simulation.
        /// </summary>
        public void Disconnect()
        {
            // Stop the message pump thread.
            if (IsConnectionActive())
            {
                // Handle disconnecting properly.
                OnDisconnected();
            }
        }

        /// <summary>
        /// Register for the SimConnect events we care about.
        /// </summary>
        protected virtual void SimConnectEventRegistration()
        {
            // Register for the events we care about.
            simulation_connection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimulationConnectionOpen);
            simulation_connection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimulationConnectionClosed);
            simulation_connection.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimulationConnectionException);
        }

        /// <summary>
        /// define the data structures that we care about.
        /// </summary>
        protected virtual void DefineDataStructures()
        {
        }
        
        /// <summary>
        /// Called when this connector has connected with Prepar3D.
        /// </summary>
        protected virtual void OnConnected()
        {
            // Send the event that says we have connected to the simulation.
            Connected.SafeInvoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Called when this connector has been disconnected from Prepar3D.
        /// Handle the base call after the derived classes stuff if you need simulation_connection to be valid.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            // Stop the message pump thread.
            message_thread.Abort();

            // Since we are no longer processing messages, close the connection to the simulation.
            simulation_connection.Dispose();
            message_pump.Dispose();

            // Reset the state of the data model.
            Reset();

            // Send the disconnected event.
            Disconnected.SafeInvoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Allows derived classes to raise the error event.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected void SendError(string message)
        {
            Error.SafeInvoke(this, new MessageEventArgs(message));
        }

        /// <summary>
        /// Resets the connection data.
        /// </summary>
        private void Reset()
        {
            simulation_connection = null;
            message_pump = null;
            message_thread = null;
        }

        /// <summary>
        /// Returns whether or not the simulation is currently connected.
        /// </summary>
        /// <returns><c>true</c> if the simulation is connected; otherwise <c>false</c>.</returns>
        public bool IsConnectionActive()
        {
            return (simulation_connection != null);
        }

        /// <summary>
        /// Handles when exceptions occur from SimConnect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimulationConnectionException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            // Show the exception to the user.
            SendError("Exception received: " + data.dwException.ToString());
        }

        /// <summary>
        /// Handles the simulation Quit event. This is fired off if Prepar3D is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimulationConnectionClosed(SimConnect sender, SIMCONNECT_RECV data)
        {
            // Disconnect from the simulation.
            Disconnect();
        }

        /// <summary>
        /// Handles the simulation Open event. This is fired off when we first connect to the simulator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimulationConnectionOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            OnConnected();
        }

        /// <summary>
        /// The message processing function.
        /// </summary>
        private void MessageProcessor()
        {
            // Process this thread unless we are aborted.
            while (true)
            {
                // Block until a message comes through.
                message_pump.WaitOne();

                // Process the simconnect message.
                if (simulation_connection != null)
                {
                    try
                    {
                        // Try to receive a message.
                        simulation_connection.ReceiveMessage();
                    }
                    catch (Exception ex)
                    {
                        // Throw an error with the exceptions message.
                        SendError(ex.Message);
                    }
                }
            }
        }
        #endregion
    }
}
