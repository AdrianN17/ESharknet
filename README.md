# E_Sharknet
Wrapper of ENet-CSharp and others utilities to easy develop Lan multiplayer games

![alt text](https://img.shields.io/badge/.NET-4.0-brightgreen) ![alt_text](https://img.shields.io/badge/Version-0.2-blue) ![alt text](https://img.shields.io/badge/Status-In%20development-orange)

## Description:

Kit tool for easy develop Lan multiplayers games:

* Fin server using broadcasting
* Easy wrapp to EnetSharp, like [sock.lua](https://github.com/camchenry/sock.lua)


## Example:

### Server

```c#
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
        
        if (Input.GetKeyDown("space"))
        {
            server.SendToAll("Hi", "How are you?");
        }
    }

    void OnDestroy()
    {
        server.Destroy();
        Debug.LogWarning("Destroy gameobject");
        server = null;
    }
```

### Client

```c#
    public ushort port;
    public int timeout;
    public Client client;
    public string ip;

    void Start()
    {

        ip = new LocalIP().SetLocalIP();

        client = new Client(ip,port,0,timeout);

        client.AddTrigger("Hi", delegate (ENet.Event net_event) {
            var obj = client.JSONDecode(net_event.Packet);
            Debug.Log(obj.value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        client.update();
    }

    void OnDestroy()
    {
        client.Destroy();
        Debug.LogWarning("Destroy gameobject");
        client = null;
    }
```

### Broadcast Send

```c#

    public ushort port;
    public ushort port_send;
    private Broadcast_send broadcast;
    public int timedelay;

    void Start()
    {
        string ip = new LocalIP().SetLocalIP();


        var server_details = new Dictionary<string, dynamic>();
        server_details.Add("ip", "192.168.0.3");
        server_details.Add("port", 22122);
        server_details.Add("players", 1);
        server_details.Add("max_players", 8);
        server_details.Add("name_server", room1);

        var data = new Dictionary<string, dynamic>() {
            { "broadcast",server_details }
        };

        broadcast = new Broadcast_send(ip,port,port_send, timedelay, data);
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void OnDestroy()
    {
        broadcast.Destroy();
    }
    
```

### Broadcast Receive

```c#

    broadcast = new Broadcast_receive(ip,22124,timedelay);
```


## Documentation: 

## Dependences:

* [JSON.net](https://www.newtonsoft.com/json)
* [.NET 4.0 compatibility](https://docs.microsoft.com/en-us/visualstudio/cross-platform/unity-scripting-upgrade?view=vs-2019)
* [ENet C#](https://github.com/nxrighthere/ENet-CSharp)




