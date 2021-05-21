using System;
using System.Net;
using Trou;
using Trou.ServicesSettings;

namespace TrouExample {

    /// <summary>
    /// Program class
    /// </summary>
    public class Program {

        /// <summary>
        /// Entry point
        /// </summary>
        private static void Main() {

            // - Instantiate Trou proxy

            TrouProxy proxy = new TrouProxy(new TorProxySettings() {

                // Relative or full path to the tor bundle
                TorBundlePath = @"TorProxy"

            }, new TorControllerSettings() {

            }, new PrivoxyProxySettings() {

                // Relative or full path to the privoxy bundle
                PrivoxyBundlePath = @"PrivoxyProxy"

            });

            // - Start

            proxy.Start();

            // - Check ip

            // Create client connected to Tor using Trou
            WebClient client = new WebClient() {
                Proxy = new WebProxy("127.0.0.1:8118")
            };

            // Write TOR Ip address
            Console.WriteLine(client.DownloadString("http://api.ipify.org"));

            // - Stop
            Console.ReadLine();
            client.Dispose();
            proxy.Dispose();

        }

    }

}
