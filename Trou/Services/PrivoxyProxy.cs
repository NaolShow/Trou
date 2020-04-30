using System;
using System.IO;
using System.Linq;
using Trou.ServicesSettings;
using Trou.Tools;

namespace Trou.Services {

    /// <summary>
    /// Allows to start a <see cref="PrivoxyProxy"/> which will listen to an IP and a Port for HTTP requests
    /// </summary>
    public class PrivoxyProxy : IDisposable {

        #region Variables

        #region PrivoxyProxy

        /// <summary>
        /// <see cref="PrivoxyProxy"/> hidden process
        /// </summary>
        public HiddenProcess Process { get; private set; }
        /// <summary>
        /// <see cref="PrivoxyProxy"/> process executable path
        /// </summary>
        public string PrivoxyExe { get; private set; }

        /// <summary>
        /// <see cref="PrivoxyProxy"/> settings
        /// </summary>
        public PrivoxyProxySettings Settings { get; set; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new intance of the <see cref="PrivoxyProxy"/> service
        /// </summary>
        public PrivoxyProxy(PrivoxyProxySettings settings) {

            // Save
            Settings = settings;

        }

        #endregion

        /// <summary>
        /// Refreshes every Arguments and Configuration keys from the <see cref="PrivoxyProxySettings"/> before starting the <see cref="PrivoxyProxy"/> process
        /// </summary>
        private void RefreshArguments() {

            #region Paths

            // Privoxy executable path
            PrivoxyExe = Settings.PrivoxyFile ?? Path.Combine(Settings.PrivoxyBundlePath, "privoxy.exe");

            #endregion

            #region Network

            // Listening address & port
            Settings.Configuration["listen-address"] = string.Format("{0}:{1}", Settings.ListeningAddress, Settings.ListeningPort);

            #region Forward

            // Forward to SOCKS 5 proxy
            Settings.Configuration["forward-socks5t"] = string.Format("/ {0}:{1} .", Settings.ForwardAddress, Settings.ForwardPort);

            #endregion

            #endregion

            #region Arguments

            // Set process as not a daemon
            Settings.Arguments["--no-daemon"] = string.Empty;

            // Set configuration file in arguments
            Settings.Arguments["configfile"] = Path.Combine(Path.GetDirectoryName(PrivoxyExe), "trou_privoxy_config");

            #endregion

        }

        #region Start & Stop

        /// <summary>
        /// Starts the <see cref="PrivoxyProxy"/> service
        /// </summary>
        public void Start() {

            // Privoxy process is already started
            if (Process != null) {

                // Stop Privoxy process
                Dispose();

            }

            // Refresh arguments and get them as a single string
            RefreshArguments();
            string arguments = string.Join(" ", Settings.Arguments.ToList().Select(a => string.Format("{0} {1}", a.Key, a.Value)));

            // Write the configuration file
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(PrivoxyExe), "trou_privoxy_config"), string.Join(Environment.NewLine, Settings.Configuration.ToList().Select(a => string.Format("{0} {1}", a.Key, a.Value))));

            // Create hidden process
            Process = new HiddenProcess(Path.GetDirectoryName(PrivoxyExe), PrivoxyExe, arguments, HiddenProcess.PriorityClass.NORMAL_PRIORITY_CLASS, true);

            // Start hidden process
            if (!Process.Start()) {

                // Throw exception
                throw new Exception("start_failed: Privoxy process start failed");

            }

        }

        /// <summary>
        /// Dispose the <see cref="PrivoxyProxy"/> service
        /// </summary>
        public void Dispose() {

            // Dispose process
            Process?.Dispose();

        }

        #endregion

    }

}
