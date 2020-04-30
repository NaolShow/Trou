using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Trou.ServicesSettings;
using Trou.Tools;

namespace Trou.Services {

    /// <summary>
    /// Allows to start a <see cref="TorProxy"/> which will listen to an IP and a Port for SOCKS 5 requests
    /// </summary>
    public class TorProxy : IDisposable {

        #region Variables

        #region TorProxy

        /// <summary>
        /// <see cref="TorProxy"/> hidden process
        /// </summary>
        public HiddenProcess Process { get; private set; }
        /// <summary>
        /// <see cref="TorProxy"/> process executable path
        /// </summary>
        public string TorExe { get; private set; }

        /// <summary>
        /// <see cref="TorProxy"/> settings
        /// </summary>
        public TorProxySettings Settings { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Triggered when <see cref="TorProxy"/> process output informations
        /// </summary>
        public event EventHandler<string> OnOutput;
        /// <summary>
        /// Triggered when <see cref="TorProxy"/> process output warns or errors
        /// </summary>
        public event EventHandler<string> OnErrorOutput;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new intance of the <see cref="TorProxy"/> service
        /// </summary>
        public TorProxy(TorProxySettings settings) {

            // Save
            Settings = settings;

        }

        #endregion

        /// <summary>
        /// Refreshes every Arguments from the <see cref="TorProxySettings"/> before starting the <see cref="TorProxy"/> process
        /// </summary>
        private void RefreshArguments() {

            #region Paths

            // Tor executable path
            TorExe = Settings.TorFile ?? Path.Combine(Settings.TorBundlePath, "Tor", "tor.exe");
            // Tor cache path
            SetPathArgument("CacheDirectory", Settings.CachePath);

            #region GeoIPFiles

            // Geo IP File (IPV4)
            SetPathArgument("GeoIPFile", Settings.GeoIPFile ?? Path.Combine(Settings.TorBundlePath, "Data", "Tor", "geoip"));
            // Geo IPv6 File (IPV6)
            SetPathArgument("GeoIPv6File", Settings.GeoIPv6File ?? Path.Combine(Settings.TorBundlePath, "Data", "Tor", "geoip6"));

            #endregion

            #endregion

            #region Network

            // Listening address & port
            SetArgument("SocksPort", string.Format("{0}:{1}", Settings.ListeningAddress, Settings.ListeningPort));

            #region Controller

            // Listening address & port
            SetArgument("ControlPort", string.Format("{0}:{1}", Settings.ListeningControlAddress, Settings.ListeningControlPort), Settings.UseController);

            // Set the control password, use the hashed one if it has been set, or hash the non hashed one
            SetArgument("HashedControlPassword", Settings.HashedControlPassword ?? HashPassword(Settings.ControlPassword), Settings.ControlPassword != null || Settings.HashedControlPassword != null);

            #endregion

            #region Nodes filtering

            // If Tor should respect the nodes filtering
            SetArgument("StrictNodes", (Settings.StrictNodes) ? "1" : "0");

            #region Whitelists

            SetListArgument("ExitNodes", Settings.ExitNodes);
            SetListArgument("MiddleNodes", Settings.MiddleNodes);
            SetListArgument("EntryNodes", Settings.EntryNodes);

            #endregion
            #region Blacklists

            SetListArgument("ExcludeNodes", Settings.ExcludeNodes);
            SetListArgument("ExcludeExitNodes", Settings.ExcludeExitNodes);

            #endregion

            #endregion
            #region Traffic

            // No traffic to .onion websites
            SetToggleArgument("NoOnionTraffic", Settings.NoOnionTraffic);
            // Accept traffic only to .onion websites
            SetToggleArgument("OnionTrafficOnly", Settings.OnionTrafficOnly);

            #endregion

            #endregion

        }

        /// <summary>
        /// Triggered when we receive process output that needs to be forwarded to events
        /// </summary>
        private void ForwardOutput(string message) {

            // If message is a normal output
            if (message.Contains("[notice]")) {

                // Trigger event
                OnOutput?.Invoke(this, message);

            } else {

                // Trigger event
                OnErrorOutput?.Invoke(this, message);

            }

        }

        #region Start & Stop

        /// <summary>
        /// Starts the <see cref="TorProxy"/> service
        /// </summary>
        public void Start() {

            // Tor process is already started
            if (Process != null) {

                // Stop Tor process
                Dispose();

            }

            // Refresh arguments and get them as a single string
            RefreshArguments();
            string arguments = string.Join(" ", Settings.Arguments.ToList().Select(a => string.Format("{0} {1}", a.Key, a.Value)));

            // Create hidden process
            Process = new HiddenProcess(Path.GetDirectoryName(TorExe), TorExe, arguments, HiddenProcess.PriorityClass.NORMAL_PRIORITY_CLASS, true);

            // Set output event (forward to ForwardOuput function)
            Process.OnOutput += (sender, message) => ForwardOutput(message);

            // Start hidden process
            if (Process.Start()) {

                // Process already stopped (this is due to a critical error, like port already used..)
                if (Process.Process != null && Process.Process.HasExited) {

                    // Throw exception
                    throw new Exception("start_critical_error: Tor process start failed due to a critical error, check if the port isn't already used");

                }

            } else {

                // Throw exception
                throw new Exception("start_failed: Tor process start failed, try increasing the privileges");

            }

        }

        /// <summary>
        /// Dispose the <see cref="TorProxy"/> service
        /// </summary>
        public void Dispose() {

            // Dispose process
            Process?.Dispose();

        }

        #endregion

        #region Arguments utility

        /// <summary>
        /// Sets an argument that can be active or not<br/>
        /// (If the argument isn't active it will be removed)
        /// </summary>
        public void SetArgument(string name, string value, bool active = true) {

            // We accept nullable arguments or the value isn't empty
            if (active) {

                // Set argument
                Settings.Arguments[name] = value;

            } else {

                // Remove argument
                Settings.Arguments.Remove(name);

            }

        }

        /// <summary>
        /// Sets a toggle argument  and remove it if the value is false
        /// </summary>
        public void SetToggleArgument(string name, bool value) {

            // We have to set the argument
            if (value) {

                // Set argument
                Settings.Arguments[name] = string.Empty;

            } else {

                // Clear entry with key
                Settings.Arguments.Remove(name);

            }

        }

        /// <summary>
        /// Sets an argument to a list and remove it if the list is empty
        /// </summary>
        public void SetListArgument(string name, List<string> value) {

            // The list isn't empty
            if (value.Count > 0) {

                // Set argument
                Settings.Arguments[name] = string.Join(",", value.Select(a => "{" + a + "}"));

            } else {

                // Remove argument
                Settings.Arguments.Remove(name);

            }



        }

        /// <summary>
        /// Sets a path argument, throw an exception if the path isn't valid
        /// </summary>
        public void SetPathArgument(string name, string path) {

            // If path isn't null
            if (path != null) {

                // Path exists
                if (File.Exists(path) || Directory.Exists(path)) {

                    // Set argument
                    Settings.Arguments[name] = path;

                } else {

                    // Throw exception
                    throw new Exception(string.Format("invalid_path: Path '{0}' assigned to '{1}' isn't valid", path, name));

                }

            }

        }

        #endregion

        /// <summary>
        /// Hash a password using the <see cref="TorExe"/> process
        /// </summary>
        public string HashPassword(string password) {

            // Password isn't empty
            if (!string.IsNullOrEmpty(password)) {

                // Create process runner (the process will stop itself)
                HiddenProcess runner = new HiddenProcess(null, TorExe, string.Format("--hash-password \"{0}\"", password), HiddenProcess.PriorityClass.NORMAL_PRIORITY_CLASS, false);

                // Process on output event (get password hash)
                string passwordHash = null;
                runner.OnOutput += (sender, message) => {

                    // If it's the password hash
                    if (message.StartsWith("16:")) {

                        // Save password hash
                        passwordHash = message;

                    }

                };

                // Start process
                runner.Start();

                // Wait to get the password hash or wait for the end of the timeout
                Stopwatch timeout = Stopwatch.StartNew();
                while (timeout.ElapsedMilliseconds < 5000 && passwordHash == null) { Thread.Sleep(500); }

                // Stop process
                runner.Dispose();

                return passwordHash;

            }
            return null;

        }

    }

}
