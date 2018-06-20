# 可靠性UDP
[位置](https://github.com/jiowchern/Regulus/tree/master/Library/Regulus.Network)
[測試](https://github.com/jiowchern/Regulus/tree/master/Test/Regulus.Network.Tests)
[測試工具](https://github.com/jiowchern/Regulus/tree/master/Test/Regulus.Network.Tests.TestTool)

```csharp
void CreateServer()
{
    var server = new Regulus.Network.Rudp.Server(new UdpSocket()) as Regulus.Network.IServer ;
    server.AcceptEvent += PeerConnected; // 連線成功
    server.Bind(12345); // 監聽的連接埠
    server.Close(); // 關閉監聽
}

void PeerConnected(IPeer peer)
{
    // 參考https://github.com/jiowchern/Regulus/blob/master/Library/Regulus.Network/IPeer.cs
}

void CreateClient()
{
    var rudpClient = new Regulus.Network.Rudp.Client(new UdpSocket());
    rudpClient.Launch(); // 啟動
    SpawnConnecter(rudpClient);
    rudpClient.Shutdown();//關閉
}
void SpawnConnecter(ICLient client)
{
    var connecter = client.Spawn();
    connecter.Connect( new IPEndPoint(IPAddress.Parse("127.0.0.1"),port ) , ConnectResult);
}

void ConnectResult(bool result)
{
    // result = 是否連線成功
}
```