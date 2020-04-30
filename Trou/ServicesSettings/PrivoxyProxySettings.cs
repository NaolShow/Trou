using System.Collections.Generic;
using Trou.Services;

namespace Trou.ServicesSettings {

    /// <summary>
    /// Allows to configure a <see cref="PrivoxyProxy"/>
    /// </summary>
    public class PrivoxyProxySettings {

        #region Paths

        /// <summary>
        /// Path to the extracted Privoxy Bundle folder (folder that contains the privoxy.exe file). This is the fastest way of configuring <see cref="PrivoxyProxy"/> paths.<br/>
        /// For a more advanced paths configuration, you can set every <see cref="PrivoxyProxy"/> paths manually (if those paths are set, they will not be recovered from <see cref="PrivoxyBundlePath"/> path):<br/>
        /// <see cref="PrivoxyFile"/>
        /// </summary>
        public string PrivoxyBundlePath { get; set; } = null;

        /// <summary>
        /// Path to the privoxy.exe file<br/>
        /// If this value isn't set, <see cref="TorProxy"/> will try to recover it using <see cref="PrivoxyFile"/><br/>
        /// (Default: null)
        /// </summary>
        public string PrivoxyFile { get; set; } = null;

        #endregion

        #region Process Arguments

        /// <summary>
        /// Arguments that are used when starting the <see cref="PrivoxyProxy"/> process<br/>
        /// Useful when you want to add arguments that are not natively supported by Trou<br/>
        /// -> <see href="https://www.privoxy.org/user-manual/startup.html#CMDOPTIONS">List of the supported Privoxy arguments here</see>
        /// </summary>
        public readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();

        #endregion
        #region Configuration

        /// <summary>
        /// Configuration pairs that are used when starting the <see cref="TorProxy"/> process<br/>
        /// Useful when you want to add configuration pairs that are not natively supported by Trou<br/>
        /// -> <see href="https://www.privoxy.org/user-manual/config.html">List of the supported Tor arguments here</see>
        /// </summary>
        public readonly Dictionary<string, string> Configuration = new Dictionary<string, string>();

        #region Network

        /// <summary>
        /// Address that the <see cref="PrivoxyProxy"/> is listening at (HTTP)<br/>
        /// -> <see href="https://www.privoxy.org/user-manual/config.html#LISTEN-ADDRESS">Documentation here</see><br/>
        /// (Default: 127.0.0.1)
        /// </summary>
        public string ListeningAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port that the <see cref="PrivoxyProxy"/> is listening at (HTTP)<br/>
        /// -> <see href="https://www.privoxy.org/user-manual/config.html#LISTEN-ADDRESS">Documentation here</see><br/>
        /// (Default: 8118)
        /// </summary>
        public int ListeningPort { get; set; } = 8118;

        #region Forward

        /// <summary>
        /// Address that the <see cref="PrivoxyProxy"/> will forward requests to (SOCKS 5)<br/>
        /// -> <see href="https://www.privoxy.org/user-manual/config.html#SOCKS">Documentation here</see><br/>
        /// (Default: 127.0.0.1)
        /// </summary>
        public string ForwardAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port that the <see cref="PrivoxyProxy"/> will forward requests to (SOCKS 5)<br/>
        /// -> <see href="https://www.privoxy.org/user-manual/config.html#SOCKS">Documentation here</see><br/>
        /// (Default: 9050)
        /// </summary>
        public int ForwardPort { get; set; } = 9050;

        #endregion

        #endregion

        #endregion

    }

}
