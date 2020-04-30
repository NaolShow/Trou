# <img src="https://github.com/NaolShow/Trou/blob/master/logo.png?raw=true" width="250"/> 
---

> Join **ToWolf** server to get latest news about **Trou** - https://discord.gg/m7CZ6md

> Tor + Privoxy for the best anonymous HTTP proxy implementation on C#

Trou is a complete Tor (and Tor Controller) and Privoxy implementation on C#
You can use every services separately or combined to have a local anonymous proxy !

**You could use Trou with**:
- HttpClients
- WebClients
- WebBrowsers
- (and everything that support HTTP or Socks5 proxy..)

## Compatibility

**Trou is made using .NET Core 3.1 and it's working currently working on:**
- Windows 8 and higher (you could go up to windows 7, just check the project wiki)

/!\ **Linux and Mac OS compatibility is planned** /!\

# :rocket: Quick example
---

Here's a quick example on how to use Trou.
This example is very minimalist, and it doesn't even care about errors/warnings/exceptions..

You can also get this [example project](https://github.com/NaolShow/Trou/blob/master/TrouExample)

```cs

// - Instantiate Trou proxy

TrouProxy proxy = new TrouProxy(new TorProxySettings() {
    TorBundlePath = @"C:\AnyFolder\TorProxy"
}, new TorControllerSettings() {

}, new PrivoxyProxySettings() {
    PrivoxyBundlePath = @"C:\AnyFolder\PrivoxyProxy"
});

// - Start

proxy.Start();

// - Check IP

// Create client connected to Tor using Trou
WebClient client = new WebClient() {
    Proxy = new WebProxy("127.0.0.1:8118")
};

// Write TOR IP address
Console.WriteLine(client.DownloadString("http://api.ipify.org"));

// - Stop
Console.ReadLine();
client.Dispose();
proxy.Dispose();

```

# :books: Documentation
---

**The complete Trou documentation [can be found here](https://github.com/NaolShow/Trou/wiki)**

# :question: Help
---

If you need help, or if you want to contact me in general, just make a github issue ticket !
You can also contact me on my discord server or in private messages: [NaolShow#7243](#)

# :wrench: Installation
---

You have two ways to install Trou, the first one is by far the most simplest one:
```
// With the package manager (Nuget)
PM> Install-Package Trou

// With .NET CLI
dotnet add package Trou
```
You can also go in your project top bar menu in visual studio > Manage Nuget packages > Search for "Trou" > Install

The second way is to go in the [release](https://github.com/NaolShow/Trou/releases) tab in the github project,
and download the last .dll, and then just reference it in your project!

# :newspaper: Licence
---

Distributed under the GNU General Public Licence v3.0. See LICENSE for more information.