using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Trou.ServicesSettings;

namespace Trou.Services {

    /// <summary>
    /// Allows to start a <see cref="TorController"/> which can interact with a <see cref="TorProxy"/>
    /// </summary>
    public class TorController : IDisposable {

        #region Variables

        /// <summary>
        /// <see cref="TorController"/> settings
        /// </summary>
        public TorControllerSettings Settings { get; set; }

        #region TCPClient

        /// <summary>
        /// <see cref="TorController"/> tcp client
        /// </summary>
        private TcpClient ControlClient;

        /// <summary>
        /// <see cref="TorController"/> writer
        /// </summary>
        private StreamWriter ControlWriter;
        /// <summary>
        /// <see cref="TorController"/> reader
        /// </summary>
        private StreamReader ControlReader;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new intance of the <see cref="TorController"/> service
        /// </summary>
        public TorController(TorControllerSettings settings) {

            // Save
            Settings = settings;

        }

        #endregion

        /// <summary>
        /// Refresh the circuit of <see cref="TorProxy"/> (changes every nodes so get new IP Address)<br/>
        /// Returns true if the refresh was successful
        /// </summary>
        public bool RefreshCircuit() {
            return Send("SIGNAL NEWNYM");
        }

        /// <summary>
        /// Authenticates to <see cref="TorProxy"/><br/>
        /// Returns true if the authentication was successful
        /// </summary>
        public bool Authenticate() {
            return Send(string.Format("AUTHENTICATE \"{0}\"", Settings.ControlPassword));
        }

        #region Quit, Shutdown, Dormant/Active

        /// <summary>
        /// Quit/Log-Off from <see cref="TorProxy"/><br/>
        /// Returns true if the log-off was successful
        /// </summary>
        public bool Quit() {
            return Send("QUIT");
        }

        /// <summary>
        /// Stops completely the <see cref="TorProxy"/><br/>
        /// Returns true if the shutdown was successful
        /// </summary>
        public bool Shutdown() {
            return Send("SIGNAL SHUTDOWN");
        }

        /// <summary>
        /// Sets <see cref="TorProxy"/> to a "sleep mode"<br/>
        /// Returns true if the sleep mode set was successful
        /// </summary>
        public bool Dormant() {
            return Send("SIGNAL DORMANT");
        }

        /// <summary>
        /// Removes the "sleep mode" (Dormant) from <see cref="TorProxy"/>
        /// Returns true if the sleep mode remove was successful
        /// </summary>
        public bool Active() {
            return Send("SIGNAL ACTIVE");
        }

        #endregion

        #region TcpClient

        /// <summary>
        /// Sends a text message to <see cref="TorProxy"/><br/>
        /// Returns true if the sending was successful
        /// </summary>
        public bool Send(string text) {

            try {

                // Send text to tor controller
                ControlWriter.WriteLine(text);
                ControlWriter.Flush();

                // Return response state (valid or not)
                return (ControlReader.ReadLine()).Contains("250");

            } catch { return false; }

        }

        #endregion

        #region Start & Stop

        /// <summary>
        /// Starts the <see cref="TorController"/> service
        /// </summary>
        public void Start() {

            // Control client is already started
            if (ControlClient != null) {

                // Stop TorController
                Dispose();

            }

            // Create ControlClient
            ControlClient = new TcpClient();

            // Connect the client to Tor and check if the connection was successful
            ControlClient.Connect(Settings.TorAddress, Settings.TorPort);
            if (ControlClient.Connected) {

                // Save network stream
                NetworkStream stream = ControlClient.GetStream();

                // Create stream writer & reader
                ControlWriter = new StreamWriter(stream, Encoding.ASCII, 4096, true);
                ControlReader = new StreamReader(stream, Encoding.ASCII, false, 4096, true);

                // If the authentification to Tor didn't work (due to wrong password)
                if (!Authenticate()) {

                    // Throw exception
                    throw new Exception("authentification_failed: Control client authentification failed");

                }

            } else {

                // Throw exception
                throw new Exception("connection_failed: Control client connection to Tor service failed");

            }


        }

        /// <summary>
        /// Dispose the <see cref="TorController"/> service
        /// </summary>
        public void Dispose() {

            // If ControlClient is connected
            if (ControlClient != null && ControlClient.Connected) {

                // Quit Tor connection
                Quit();

            }

            // Dispose client & streams
            ControlWriter?.Dispose();
            ControlReader?.Dispose();

            ControlClient?.Dispose();

        }

        #endregion

    }

}
