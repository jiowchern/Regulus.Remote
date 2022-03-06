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

## 简介
这是一个使用 .Net Standard2.0 开发的主从式连线框架，可使用在 Unity 游戏引擎和其他符合 .Net Standard2.0 的游戏引擎。

## 特色
服务端与客户端透过接口做传输, 减少协定的维护成本.
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



## 范例
**1. 定义接口 IGreeter .**  
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
**2. 服务端实作 IGreeter.**  
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
**3. 透过 ```IBinder.Bind``` 将 ```IGreeter``` 传送到客户端.**  
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
			// 透过 Unbind 解除与客户端的联系.
			_Binder.Unbind(_GreeterSoul);
		}
	}
}
```
**4. 客户端用 ```IAgent.QueryNotifier``` 取得 ```IGreeter``` .**
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

完成以上步骤服务端和客户端就可以通过接口进行通信, 尽可能的实现面向对象的开发.
#### 支持规范
**接口**  
除了上述范例 ```IGreeter.SayHello``` 外还有提供以下总共四种方式 ...
<!-- In addition, bind and unbind are used to switch the objects of the server, so as to control the access rights of the client conveniently.  -->
* [Method](document/communications-method.md) <-- ```IGreeter.SayHello``` 的方式
* [Event](document/communications-event.md)
* [Property](document/communications-property.md)
* [Notifier](document/communications-notifier.md)

**序列化**  
可以序列化的类型请参阅 [Regulus.Serialization](Regulus.Serialization/README-CN.md) 说明.
<!-- > Serialization supports the following types...  
> ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, char, byte, enum, string``` and array of the types. -->
          
---
## 开始
这是一个主从式框架, 所以需要建立三个项目 : **Protocol**, **Server** and **Client**.

#### 环境需求
* Visual Studio 2022  17.0.5 以上.
* .NET Sdk 5 以上. 

#### Protocol Project
建立共用接口项目 **Protocol.csproj** .
```powershell
Sample/Protocol>dotnet new classlib 
```
<!-- Add references to **Protocol.csproj**. -->
1. 加入参考
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
此步骤是为了产生 ```IProtocol``` 的产生器,```IProtocol``` 是框架的重要元件,服务端与客户端需要的通讯元件.  
**_注意_**  
如以上代码所示, 添加 ```Regulus.Remote.Protocol.Creater``` 属性到想要取得 ```IProtocol``` 的方法上, 方法的规范必须是 ```static partial void Method(ref Regulus.Remote.IProtocol)```, 否则会无法通过编译.

	
#### Server Project
建立服务端. **Serrver.csproj**
```powershell
Sample/Server>dotnet new console 
```
1. 加入参考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Server" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />	
</ItemGroup>
```
2. 实作 ```IGreeter```
```csharp
namespace Server
{
	public class Greeter : Protocol.IGreeter
	{
		Regulus.Remote.Value<string> SayHello(string request)
		{
			// 回传收到的信息
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
4. 建立 Tcp 服务
```csharp
namespace Server
{	
	static void Main(string[] args)
	{		
		// 透过 ProtocolCreater 取得 IProtocol
		var protocol = Protocol.ProtocolCreater.Create();

		// 服务端的进入点
		var entry = new Entry();
		
		// 带入 Entry , 建立服务
		var set = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
		int yourPort = 0;
		set.Listener.Bind(yourPort);
				
		// 关闭服务
		set.Listener.Close();
		set.Service.Disoprt();
	}
}
```
#### Client Project
建立客户端. **Client.csproj**.  
```powershell
Sample/Client>dotnet new console 
```
1. 加入参考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Client" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
</ItemGroup>
```
2. 建立 Tcp 客户端
```csharp
namespace Client
{	
	static void Main(string[] args)
	{		
		// 透过 ProtocolCreater 取得 IProtocol
		var protocol = Protocol.ProtocolCreater.Create();
		
		// 建立客户端
		var set = Regulus.Remote.Client.Provider.CreateTcpAgent(protocol);

		// 呼叫 Agent.Update 保持资料传输, 在此产生一个简易 thread 使其保持运作.
		bool stop = false;
		var task = System.Threading.Tasks.Task.Run(() => 
		{
			while (!stop)
			{
				set.Agent.Update();
			}
                
		});
		// 开始连线
		EndPoint yourEndPoint = null;
		set.Connecter.Connect(yourEndPoint ).Wait();

		// SupplyEvent 接收服务端绑定的 IGreeter
		set.Agent.QueryNotifier<Protocol.IGreeter>().Supply += greeter => 
		{
			// 呼叫物件
			greeter.SayHello("hello");
		};

		// UnsupplyEvent 接收服务端解绑的 IGreeter
		set.Agent.QueryNotifier<Protocol.IGreeter>().Unsupply += greeter => 
		{
			
		};

		// 关闭
		stop = true;
		task.Wait();
		set.Connecter.Disconnect();
		set.Agent.Dispose();

	}
}
```
---
## 单机模式
为了方便开发与除错, 提供了单机模式, 可以在不经过连线的情况下运行系统.
```powershell
Sample/Standalone>dotnet new console 
```
1. 加入参考
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Standalone" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
	<ProjectReference Include="..\Server\Server.csproj" />
</ItemGroup>
```
2.  建立单机服务
```csharp
namespace Standalone
{	
	static void Main(string[] args)
	{		
		// 透过 ProtocolCreater 取得 IProtocol
		var protocol = Protocol.ProtocolCreater.Create();
		
		// 建立服务
		var entry = new Entry();
		var service = Regulus.Remote.Standalone.Provider.CreateService(entry , protocol);
		var agent = service.Create();

		// 呼叫 Agent.Update 保持资料传输, 在此产生一个简易 thread 使其保持运作.
		bool stop = false;
		var task = System.Threading.Tasks.Task.Run(() => 
		{
			while (!stop)
			{
				agent.Update();
			}
                
		});
		
		// SupplyEvent 接收服务端绑定的 IGreeter
		agent.QueryNotifier<Protocol.IGreeter>().Supply += greeter => 
		{
			// 呼叫物件
			greeter.SayHello("hello");
		};

		// UnsupplyEvent 接收服务端解绑的 IGreeter
		agent.QueryNotifier<Protocol.IGreeter>().Unsupply += greeter => 
		{
			
		};

		// 关闭
		stop = true;
		task.Wait();
		
		agent.Dispose();
		service.Dispose();		

	}
}
```
---
## 客制连线
如果想自定义连线系统可以通过以下方式.
#### 客户端
建立连线从 ```CreateTcpAgent``` 改为 ```CreateAgent``` 并实现接口 ```IStreamable```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
IStreamable stream = null ;// todo: 实现接口 IStreamable
var service = Regulus.Remote.Client.CreateAgent(protocol , stream) ;
```
实现类型 IStreamable
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

#### 服务端
建立服务从 ```CreateTcpService``` 改为 ```CreateService``` 并实现接口 ```IListenable```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
IListenable listener = null; // todo: 实现接口 IListenable
var service = Regulus.Remote.Server.CreateService(entry , protocol , listener) ;
```
实现类型 IListenable
```csharp
namespace Regulus.Remote.Soul
{
    public interface IListenable
    {
		// 连线时提供 IStreamable
        event System.Action<Network.IStreamable> StreamableEnterEvent;
		// 断线时提供 IStreamable
        event System.Action<Network.IStreamable> StreamableLeaveEvent;
    }
}
```
---
## 客制序列化
实现类型 ```ISerializable``` 
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
并带入到服务端 ```CreateTcpService```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
ISerializable serializer = null; // todo: 实现接口 ISerializable
var service = Regulus.Remote.Server.CreateTcpService(entry , protocol , serializer) ;
```

并带入到客户端 ```CreateTcpAgent```
```csharp
var protocol = Protocol.ProtocolCreater.Create();
ISerializable serializer = null ;// todo: 实现接口 ISerializable
var service = Regulus.Remote.Client.CreateTcpAgent(protocol , serializer) ;
```
