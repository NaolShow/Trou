using System.Collections.Generic;
using Trou.Services;

namespace Trou.ServicesSettings {

    /// <summary>
    /// Allows to configure a <see cref="TorProxy"/>
    /// </summary>
    public class TorProxySettings {

        #region Paths

        /// <summary>
        /// Path to the extracted Tor Bundle folder (folder that contains the Tor and Data folders). This is the fastest way of configuring <see cref="TorProxy"/> paths<br/>
        /// For a more advanced paths configuration, you can set every <see cref="TorProxy"/> paths manually (if those paths are set, they will not be recovered from <see cref="TorBundlePath"/> path):<br/>
        /// <see cref="TorFile"/>, <see cref="CachePath"/>, <see cref="GeoIPFile"/>, <see cref="GeoIPv6File"/>
        /// </summary>
        public string TorBundlePath { get; set; } = null;

        /// <summary>
        /// Path to the tor.exe file<br/>
        /// If this value isn't set, <see cref="TorProxy"/> will try to recover it using <see cref="TorBundlePath"/><br/>
        /// (Default: null)
        /// </summary>
        public string TorFile { get; set; } = null;

        /// <summary>
        /// Path where the <see cref="TorProxy"/> cache will be stored<br/>
        /// If this value isn't set, <see cref="TorProxy"/> will use your ApplicationData folder<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#CacheDirectory">Documentation here</see><br/>
        /// (Default: null)
        /// </summary>
        public string CachePath { get; set; } = null;

        #region GeoIPFiles

        /// <summary>
        /// Path to the geoip file (this file is used when using nodes filtering)
        /// If this value isn't set, <see cref="TorProxy"/> will try to recover it using <see cref="TorBundlePath"/><br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#GeoIPFile">Documentation here</see><br/>
        /// (Default: null)
        /// </summary>
        public string GeoIPFile { get; set; } = null;

        /// <summary>
        /// Path to the geoip6 file (this file is used when using nodes filtering)
        /// If this value isn't set, <see cref="TorProxy"/> will try to recover it using <see cref="TorBundlePath"/><br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#GeoIPv6File">Documentation here</see><br/>
        /// (Default: null)
        /// </summary>
        public string GeoIPv6File { get; set; } = null;

        #endregion

        #endregion

        #region Process Arguments

        /// <summary>
        /// Arguments that are used when starting the <see cref="TorProxy"/> process<br/>
        /// Useful when you want to add arguments that are not natively supported by Trou<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en">List of the supported Tor arguments here</see>
        /// </summary>
        public readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();

        #region Network

        /// <summary>
        /// Address that the <see cref="TorProxy"/> is listening at (SOCKS 5)<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#SocksPort">Documentation here</see><br/>
        /// (Default: 127.0.0.1)
        /// </summary>
        public string ListeningAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port that the <see cref="TorProxy"/> is listening at (SOCKS 5)<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#SocksPort">Documentation here</see><br/>
        /// (Default: 9050)
        /// </summary>
        public int ListeningPort { get; set; } = 9050;

        #region Controller

        /// <summary>
        /// Determines if <see cref="TorProxy"/> should listen to <see cref="TorController"/><br/>
        /// (Default: true)
        /// </summary>
        public bool UseController { get; set; } = true;

        /// <summary>
        /// Address that the <see cref="TorProxy"/> is listening at (for the <see cref="TorController"/>)<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#ControlPort">Documentation here</see><br/>
        /// (Default: 127.0.0.1)
        /// </summary>
        public string ListeningControlAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port that the <see cref="TorProxy"/> is listening at (for the <see cref="TorController"/>)<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#ControlPort">Documentation here</see><br/>
        /// (Default: 9051)
        /// </summary>
        public int ListeningControlPort { get; set; } = 9051;

        #region Password

        /// <summary>
        /// Password that is required for the <see cref="TorController"/> when he wants to control the <see cref="TorProxy"/><br/>
        /// If the <see cref="HashedControlPassword"/> is set, <see cref="TorProxy"/> will ignore <see cref="ControlPassword"/><br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#HashedControlPassword">Documentation here</see><br/>
        /// (Default: null)
        /// </summary>
        public string ControlPassword { get; set; } = null;

        /// <summary>
        /// Password that is required for the <see cref="TorController"/> when he wants to control the <see cref="TorProxy"/><br/>
        /// If the <see cref="HashedControlPassword"/> is set, <see cref="TorProxy"/> will ignore <see cref="ControlPassword"/><br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#HashedControlPassword">Documentation here</see><br/>
        /// (Default: null)
        /// </summary>
        public string HashedControlPassword { get; set; } = null;

        #endregion

        #endregion

        #region Nodes filtering

        /// <summary>
        /// Determines if <see cref="TorProxy"/> should strictly respect the nodes filtering<br/>
        /// (Default: true)
        /// </summary>
        public bool StrictNodes { get; set; } = false;

        #region Whitelists

        /// <summary>
        /// List of accepted exit nodes<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#ExitNodes">Documentation here</see><br/>
        /// (Default: empty)
        /// </summary>
        public readonly List<string> ExitNodes = new List<string>();

        /// <summary>
        /// List of accepted middle nodes<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#MiddleNodes">Documentation here</see><br/>
        /// (Default: empty)
        /// </summary>
        public readonly List<string> MiddleNodes = new List<string>();

        /// <summary>
        /// List of accepted entry nodes<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#EntryNodes">Documentation here</see><br/>
        /// (Default: empty)
        /// </summary>
        public readonly List<string> EntryNodes = new List<string>();

        #endregion
        #region Blacklists

        /// <summary>
        /// List of non-accepted nodes<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#ExcludeNodes">Documentation here</see><br/>
        /// (Default: empty)
        /// </summary>
        public readonly List<string> ExcludeNodes = new List<string>();

        /// <summary>
        /// List of non-accepted exit nodes<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#ExcludeExitNodes">Documentation here</see><br/>
        /// (Default: empty)
        /// </summary>
        public readonly List<string> ExcludeExitNodes = new List<string>();

        #endregion

        #endregion
        #region Traffic

        /// <summary>
        /// Determines if the <see cref="TorProxy"/> shouldn't accept connections to .onion websites<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#NoOnionTraffic">Documentation here</see><br/>
        /// (Default: false)
        /// </summary>
        public bool NoOnionTraffic { get; set; } = false;

        /// <summary>
        /// Determines if the <see cref="TorProxy"/> should only accept connections to .onion websites<br/>
        /// -> <see href="https://www.torproject.org/docs/tor-manual.html.en#OnionTrafficOnly">Documentation here</see><br/>
        /// (Default: false)
        /// </summary>
        public bool OnionTrafficOnly { get; set; } = false;

        #endregion

        #endregion

        #endregion

    }

}