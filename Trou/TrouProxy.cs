using System;
using Trou.Services;
using Trou.ServicesSettings;

namespace Trou {

    /**
     * 
     * Trou Proxy - .NET Core library made by NaolShow
     * -> https://github.com/NaolShow/Trou
     * 
     * If you like the project, please star it :) (It's helping me a lot :D)
     * 
     **/

    /// <summary>
    /// Allows to start <see cref="TorProxy"/>, <see cref="PrivoxyProxy"/> and <see cref="TorController"/> simultaneously
    /// </summary>
    public class TrouProxy : IDisposable {

        /// <summary>
        /// <see cref="TorProxy"/> instance
        /// </summary>
        public TorProxy Tor;

        /// <summary>
        /// <see cref="TorController"/> instance
        /// </summary>
        public TorController Controller;

        /// <summary>
        /// <see cref="PrivoxyProxy"/> instance
        /// </summary>
        public PrivoxyProxy Privoxy;

        #region Constructors

        /// <summary>
        /// Initializes a new intance of the <see cref="PrivoxyProxy"/> service
        /// </summary>
        public TrouProxy(TorProxySettings torSettings, TorControllerSettings controllerSettings, PrivoxyProxySettings privoxySettings) {

            // -- Instance

            Tor = new TorProxy(torSettings);
            Controller = new TorController(controllerSettings);

            Privoxy = new PrivoxyProxy(privoxySettings);

        }

        #endregion

        #region On/Off/Terminate

        /// <summary>
        /// Starts the <see cref="TrouProxy"/> service
        /// </summary>
        public void Start() {
            Tor.Start();
            Controller.Start();
            Privoxy.Start();
        }

        /// <summary>
        /// Dispose the <see cref="TrouProxy"/> service
        /// </summary>
        public void Dispose() {
            Tor.Dispose();
            Controller.Dispose();
            Privoxy.Dispose();
        }

        #endregion

    }

}
