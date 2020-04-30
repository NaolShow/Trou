using Trou.Services;

namespace Trou.ServicesSettings {

    /// <summary>
    /// Allows to configure a <see cref="TorController"/>
    /// </summary>
    public class TorControllerSettings {

        #region Network

        /// <summary>
        /// Address that the <see cref="TorController"/> will connect to<br/>
        /// (Default: 127.0.0.1)
        /// </summary>
        public string TorAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port that the <see cref="TorController"/> will connect to<br/>
        /// (Default: 9051)
        /// </summary>
        public int TorPort { get; set; } = 9051;

        /// <summary>
        /// Password that the <see cref="TorController"/> will use to authenticate to the <see cref="TorProxy"/><br/>
        /// (Default: null)
        /// </summary>
        public string ControlPassword { get; set; } = null;

        #endregion

    }

}
