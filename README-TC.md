# Regulus Remote
[![Maintainability](https://api.codeclimate.com/v1/badges/99cb5e1dc12cafbfe451/maintainability)](https://codeclimate.com/github/jiowchern/Regulus.Remote/maintainability)
[![Actions Status](https://github.com/jiowchern/Regulus.Remote/workflows/Build/badge.svg)](https://github.com/jiowchern/Regulus.Remote/actions)
[![Build status](https://ci.appveyor.com/api/projects/status/fv1owwit4utddawv/branch/release?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus-remote/branch/release)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus.Remote/badge.svg?branch=master)](https://coveralls.io/github/jiowchern/Regulus.Remote?branch=master)
![commit last date](https://img.shields.io/github/last-commit/jiowchern/Regulus.Remote)    
<!-- [![Discord](https://img.shields.io/discord/101557008930451456.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/uDF8NTp) -->

<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->
<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->
![Latest Version](https://img.shields.io/github/v/tag/jiowchern/Regulus.Remote)

## 簡介
這是一個使用 .Net Standard2.0 開發的主從式連線框架，可使用在 Unity 遊戲引擎和其他符合 .Net Standard2.0 的遊戲引擎。

## 特色
服務端與客戶端透過接口做傳輸, 減少協定的維護成本.
<!-- 
@startuml
package Protocol <<Rectangle>>{
    interface IGreeter {
        +SayHello()
    }
}


package Server <<Rectangle>> {
    class Greeter {
        +SayHello()
    }
}

package Client <<Rectangle>>{
    class SomeClass{
        {field} IGreeter greeter 
    }
}

IGreeter --* SomeClass::greeter 
IGreeter <|.. Greeter  

note left of Greeter  
    Implement IGreeter 
end note

note right of SomeClass::greeter 
    Use object from server.
end note

@enduml
-->
![plantUML](http://www.plantuml.com/plantuml/svg/ZP31JiCm38RlUGeVGMXzWAcg9kq0ko4g7Y1aVql1IIR7GqAZxqvRLGdiD5zgsVw_ViekgHKzUpOdwpvj3tgMgD55fhf-WLCRUaRJN0nDDGI5TDQ13ey2A8IcnLeFhVr-0dEykrzcencDoTWMyWNv3rt3ZcrAT1EmyFOy8EYrPC6rqMC_TuLtwGRmSIpk_VejzBpQR9g2s6xpPJweVwegEvCn8Ig8qId5himNyi6V67wspMc3SAGviWPbwD_dvDK_Yzrh0iMt3pYbJgAdj3ndzOUpczgpvry0)


<!-- * Remote Method Invocation
* .Net Standard 2.0 base
* Compatible with Unity il2cpp
* Compatible with Unity WebGL
* Customizable connection
* Stand-alone mode  -->



## 範例
**1. 定義接口 IGreeter .**  
```csharp
namespace Protocol
{
	public struct HelloRequest
	{
		public string Name;
	}
	public struct HelloReply
	{
		public string Message;
	}
	public interface IGreeter
	{
		Regulus.Remote.Value<HelloReply> SayHello(HelloRequest request);
	}
}
```
**2. 服務端實作 IGreeter.**  
```csharp
namespace Server
{	
	class Greeter : IGreeter
	{				
		Regulus.Remote.Value<HelloReply> SayHello(HelloRequest request)
		{
			return new HelloReply() { Message = $"Hello {request.Name}." };
		}
	}
}
```
**3. 透過 ```IBinder.Bind``` 將 ```IGreeter``` 傳送到客戶端.**  
```csharp
namespace Server
{
	public class Entry	
	{
		readonly Greeter _Greeter;
		readonly Regulus.Remote.IBinder _Binder;
		readonly Regulus.Remote.ISoul _GreeterSoul;
		public Entry(Regulus.Remote.IBinder binder)
		{
			_Greeter = new Greeter();
			_Binder = binder;
			// bind to client.
			_GreeterSoul = binder.Bind<IGreeter>(_Greeter);
		}
		public void Dispose()
		{
			// 透過 Unbind 解除與客戶端的聯繫.  
			_Binder.Unbind(_GreeterSoul);
		}
	}
}
```
**4. 客戶端用 ```IAgent.QueryNotifier``` 取得 ```IGreeter``` .**
```csharp
namespace Client
{
	class Entry
	{
		public Entry(Regulus.Remote.IAgent agent)
		{
			agent.QueryNotifier<IGreeter>().Supply += _AddGreeter;
			agent.QueryNotifier<IGreeter>().Unsupply += _RemoveGreeter;
		}
		async void  _AddGreeter(IGreeter greeter)
		{						
			// Having received the greeter from the server, 			 
			// begin to implement the following code.
			var reply = await greeter.SayHello(new HelloRequest() {Name = "my"});
		}
		void _RemoveGreeter(IGreeter greeter)
		{
			// todo: The server has canceled the greeter.
		}
	}
}
```
---
完成以上步驟服务端和客户端就可以通过接口进行通信, 尽可能的实现面向对象的开发.
#### 支持規範
**接口**  
除了上述範例 ```IGreeter.SayHello``` 外還有提供以下總共四種方式 ...

<!-- In addition, bind and unbind are used to switch the objects of the server, so as to control the access rights of the client conveniently.  -->
* [Method](document/communications-method.md) <-- ```IGreeter.SayHello``` 的方式
* [Event](document/communications-event.md)
* [Property](document/communications-property.md)
* [Notifier](document/communications-notifier.md)

**序列化**  
可以序列化的類型請參閱 [Regulus.Serialization](Regulus.Serialization/README-CN.md) 說明.
<!-- > Serialization supports the following types...  
> ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, char, byte, enum, string``` and array of the types. -->
          
---
## 開始
這是一個主從式框架, 所以需要建立三個項目 : **Protocol**, **Server** and **Client**.

#### 環境需求
* Visual Studio 2022  17.0.5 以上.
* .NET Sdk 5 以上. 

#### Protocol Project
建立共用接口項目 **Protocol.csproj** .
```powershell
Sample/Protocol>dotnet new classlib 
```
<!-- Add references to **Protocol.csproj**. -->
1. 加入參考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote" Version="0.1.11.0" />
	<PackageReference Include="Regulus.Serialization" Version="0.1.11.0" />
	<PackageReference Include="Regulus.Remote.Tools.Protocol.Sources" Version="0.0.0.7">
		<PrivateAssets>all</PrivateAssets>
		<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
	</PackageReference>	
</ItemGroup>
```
2. 新增接口, **IGreeter.cs**
```csharp
namespace Protocol
{
	public interface IGreeter
	{
		Regulus.Remote.Value<string> SayHello(string request);
	}
}
```
3. 新增 **ProtocolCreater.cs**.
```csharp
namespace Protocol
{
    public static partial class ProtocolCreater
    {
        public static Regulus.Remote.IProtocol Create()
        {
            Regulus.Remote.IProtocol protocol = null;
            _Create(ref protocol);
            return protocol;
        }

        /*
			Create a partial method as follows.
        */
        [Regulus.Remote.Protocol.Creater]
        static partial void _Create(ref Regulus.Remote.IProtocol protocol);
    }
}
```  
此步驟是為了產生 ```IProtocol``` 的產生器,```IProtocol``` 是框架的重要元件,服務端與客戶端需要的通訊元件.  
**_注意_**  
如以上代碼所示, 添加 ```Regulus.Remote.Protocol.Creater``` 屬性到想要取得 ```IProtocol``` 的方法上, 方法的規範必須是 ```static partial void Method(ref Regulus.Remote.IProtocol)```, 否則會無法通過編譯.

	
#### Server Project
建立服務端. **Serrver.csproj**
```powershell
Sample/Server>dotnet new console 
```
1. 加入參考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Server" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />	
</ItemGroup>
```
2. 實作 ```IGreeter```
```csharp
namespace Server
{
	public class Greeter : Protocol.IGreeter
	{
		Regulus.Remote.Value<string> SayHello(string request)
		{
			// 回傳收到的信息
			return $"echo:{request}";
		}
	}
}
```
3. 服务器需要一个启动环境的入口点 , 创建一个继承自 ```Regulus.Remote.IEntry``` 的入口. **Entry.cs**
```csharp
namespace Server
{
	public class Entry : Regulus.Remote.IEntry
	{
		void IBinderProvider.AssignBinder(IBinder binder)
		{					
			binder.Binder<IGreeter>(new Greeter());
		}		
	}
}
```
4. 建立 Tcp 服務
```csharp
namespace Server
{	
	static void Main(string[] args)
	{		
		// 透過 ProtocolCreater 取得 IProtocol
		var protocol = Protocol.ProtocolCreater.Create();

		// 服務端的進入點
		var entry = new Entry();
		
		// 帶入 Entry , 建立服務
		var set = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
		int yourPort = 0;
		set.Listener.Bind(yourPort);
				
		// 關閉服務
		set.Listener.Close();
		set.Service.Disoprt();
	}
}
```
#### Client Project
建立客戶端. **Client.csproj**.  
```powershell
Sample/Client>dotnet new console 
```
1. 加入參考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Client" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
</ItemGroup>
```
2. 建立 Tcp 客戶端
```csharp
namespace Client
{	
	static void Main(string[] args)
	{		
		// 透過 ProtocolCreater 取得 IProtocol
		var protocol = Protocol.ProtocolCreater.Create();
		
		// 建立客戶端
		var set = Regulus.Remote.Client.Provider.CreateTcpAgent(protocol);

		// 呼叫 Agent.Update 保持資料傳輸, 在此產生一個簡易 thread 使其保持運作.
		bool stop = false;
		var task = System.Threading.Tasks.Task.Run(() => 
		{
			while (!stop)
			{
				set.Agent.Update();
			}
                
		});
		// 開始連線
		EndPoint yourEndPoint = null;
		set.Connecter.Connect(yourEndPoint ).Wait();

		// SupplyEvent 接收服務端綁定的 IGreeter
		set.Agent.QueryNotifier<Protocol.IGreeter>().Supply += greeter => 
		{
			// 呼叫物件
			greeter.SayHello("hello");
		};

		// UnsupplyEvent 接收服務端解綁的 IGreeter
		set.Agent.QueryNotifier<Protocol.IGreeter>().Unsupply += greeter => 
		{
			
		};

		// 關閉
		stop = true;
		task.Wait();
		set.Connecter.Disconnect();
		set.Agent.Dispose();

	}
}
```
---
## 單機模式
為了方便開發與除錯, 提供了單機模式, 可以在不經過連線的情況下運行系統.
```powershell
Sample/Standalone>dotnet new console 
```
1. 加入參考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Standalone" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
	<ProjectReference Include="..\Server\Server.csproj" />
</ItemGroup>
```
2.  建立單機服務
```csharp
namespace Standalone
{	
	static void Main(string[] args)
	{		
		// 透過 ProtocolCreater 取得 IProtocol
		var protocol = Protocol.ProtocolCreater.Create();
		
		// 建立服務
		var entry = new Entry();
		var service = Regulus.Remote.Standalone.Provider.CreateService(entry , protocol);
		var agent = service.Create();

		// 呼叫 Agent.Update 保持資料傳輸, 在此產生一個簡易 thread 使其保持運作.
		bool stop = false;
		var task = System.Threading.Tasks.Task.Run(() => 
		{
			while (!stop)
			{
				agent.Update();
			}
                
		});
		
		// SupplyEvent 接收服務端綁定的 IGreeter
		agent.QueryNotifier<Protocol.IGreeter>().Supply += greeter => 
		{
			// 呼叫物件
			greeter.SayHello("hello");
		};

		// UnsupplyEvent 接收服務端解綁的 IGreeter
		agent.QueryNotifier<Protocol.IGreeter>().Unsupply += greeter => 
		{
			
		};

		// 關閉
		stop = true;
		task.Wait();
		
		agent.Dispose();
		service.Dispose();		

	}
}
```
---
## 客製連線
如果想自定義連線系統可以通過以下方式.
#### 客戶端
建立連線從 ```CreateTcpAgent``` 改為 ```CreateAgent``` 並實現接口 ```IStreamable```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
IStreamable stream = null ;// todo: 實現接口 IStreamable
var service = Regulus.Remote.Client.CreateAgent(protocol , stream) ;
```
實現類型 IStreamable
```csharp
using Regulus.Remote;
namespace Regulus.Network
{
    public interface IStreamable
    {
        /// <summary>
        ///     Receive data streams.
        /// </summary>
        /// <param name="buffer">Stream instance.</param>
        /// <param name="offset">Start receiving position.</param>
        /// <param name="count">Count of byte received.</param>
        /// <returns>Actual count of byte received.</returns>
        IWaitableValue<int> Receive(byte[] buffer, int offset, int count);
        /// <summary>
        ///     Send data streams.
        /// </summary>
        /// <param name="buffer">Stream instance.</param>
        /// <param name="offset">Start send position.</param>
        /// <param name="count">Count of byte send.</param>
        /// <returns>Actual count of byte send.</returns>
        IWaitableValue<int> Send(byte[] buffer, int offset, int count);
    }
}
```

#### 服務端
建立服務從 ```CreateTcpService``` 改為 ```CreateService``` 並實現接口 ```IListenable```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
IListenable listener = null; // todo: 實現接口 IListenable
var service = Regulus.Remote.Server.CreateService(entry , protocol , listener) ;
```
實現類型 IListenable
```csharp
namespace Regulus.Remote.Soul
{
    public interface IListenable
    {
		// 連線時提供 IStreamable
        event System.Action<Network.IStreamable> StreamableEnterEvent;
		// 斷線時提供 IStreamable
        event System.Action<Network.IStreamable> StreamableLeaveEvent;
    }
}
```
---
## 客製序列化
實現類型 ```ISerializable``` 
```csharp
namespace Regulus.Remote
{
    public interface ISerializable
    {
        byte[] Serialize(System.Type type, object instance);
        object Deserialize(System.Type type, byte[] buffer);
    }
}
```
並帶入到服務端 ```CreateTcpService```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
ISerializable serializer = null; // todo: 實現接口 ISerializable
var service = Regulus.Remote.Server.CreateTcpService(entry , protocol , serializer) ;
```

並帶入到客戶端 ```CreateTcpAgent```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
ISerializable serializer = null ;// todo: 實現接口 ISerializable
var service = Regulus.Remote.Client.CreateTcpAgent(protocol , serializer) ;
```


