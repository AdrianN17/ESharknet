# E_Sharknet
Wrapper of ENet-CSharp and others utilities to easy develop Lan multiplayer games

![alt text](https://img.shields.io/badge/.NET-4.0-brightgreen) ![alt_text](https://img.shields.io/badge/Version-0.1-blue) ![alt text](https://img.shields.io/badge/Status-In%20development-orange)

## Description:

Kit tool for easy develop Lan multiplayers games:

* Fin server using broadcasting
* Easy wrapp to EnetSharp, like [sock.lua](https://github.com/camchenry/sock.lua)

## Example:

### Server

```
    public ushort port;
    public int max_clients;
    public int timeout;
    public Server server;
    public string ip;

    void Start()
    {
        ip = new LocalIP().SetLocalIP();
        server = new Server(ip,port,max_clients,0,timeout);
    }

    // Update is called once per frame
    void Update()
    {
        server.update();
    }

    void OnDestroy()
    {
        server.Destroy();
        Debug.LogWarning("Destroy gameobject");
        server = null;
    }
```

### Client

```

```

## Documentation: 

## Dependences:

* [JSON.net](https://www.newtonsoft.com/json)
* [.NET 4.0 compatibility](https://docs.microsoft.com/en-us/visualstudio/cross-platform/unity-scripting-upgrade?view=vs-2019)
* [ENet C#](https://github.com/nxrighthere/ENet-CSharp)




