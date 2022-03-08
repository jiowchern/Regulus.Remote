# 轩辕远程

[![Maintainability](https://api.codeclimate.com/v1/badges/99cb5e1dc12cafbfe451/maintainability)](https://codeclimate.com/github/jiowchern/Regulus.Remote/maintainability)[![Actions Status](https://github.com/jiowchern/Regulus.Remote/workflows/Build/badge.svg)](https://github.com/jiowchern/Regulus.Remote/actions)[![Build status](https://ci.appveyor.com/api/projects/status/fv1owwit4utddawv/branch/release?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus-remote/branch/release)[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus.Remote/badge.svg?branch=master)](https://coveralls.io/github/jiowchern/Regulus.Remote?branch=master)![commit last date](https://img.shields.io/github/last-commit/jiowchern/Regulus.Remote)

<!-- [![Discord](https://img.shields.io/discord/101557008930451456.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/uDF8NTp) -->

<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->

<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->

![Latest Version](https://img.shields.io/github/v/tag/jiowchern/Regulus.Remote)

## 介绍

它是使用.Net Standard 2.0开发的服务器-客户端连接框架，可用于Unity游戏引擎和其他兼容.Net标准的游戏引擎。

## 特征

服务器和客户端通过接口进行传输，降低了协议的维护成本。

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

## 例子

1.  定义接口`IGreeter`.

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

2.  实现服务器`IGreeter`.

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

3.  采用`IBinder.Bind`发送`IGreeter`给客户。

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
			_Binder.Unbind(_GreeterSoul);
		}
	}
}
```

4.  客户使用`IAgent.QueryNotifier`获得`IGreeter`.

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

* * *

完成以上步骤后，服务端和客户端就可以通过接口进行通信，尽可能实现面向对象的开发。

#### 规格

**界面**  
除了上面的例子`IGreeter.SayHello`, 一共有四种方法...

<!-- In addition, bind and unbind are used to switch the objects of the server, so as to control the access rights of the client conveniently.  -->

-   [`Method`](document/communications-method.md)&lt;--`IGreeter.SayHello`
-   [`Event`](document/communications-event.md)
-   [`Property`](document/communications-property.md)
-   [`Notifier`](document/communications-notifier.md)

**序列化**  
对于可以序列化的类型，请参见[`Regulus.Serialization`](Regulus.Serialization/README.md)指示。

<!-- > Serialization supports the following types...  
> ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, char, byte, enum, string``` and array of the types. -->

* * *

## 入门

这是一个服务器-客户端框架，因此您需要创建三个项目：`Protocol`,`Server`和`Client`.

#### 要求

-   Visual Studio 2022 17.0.5 以上。
-   .NET SDK 5 以上。

#### 协议项目

创建通用接口项目`Protocol.csproj`.

```powershell
Sample/Protocol>dotnet new classlib 
```

<!-- Add references to **Protocol.csproj**. -->

1.  添加参考

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

2.  添加接口，`IGreeter.cs`

```csharp
namespace Protocol
{
	public interface IGreeter
	{
		Regulus.Remote.Value<string> SayHello(string request);
	}
}
```

3.  添加`ProtocolCreater.cs`.

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

这一步是生成生成器`IProtocol`，它是框架的重要组成部分，是服务器和客户端之间通信所需要的。  
**_笔记_**

> > 如上代码所示，添加`Regulus.Remote.Protocol`属性到你想得到的方法`IProtocol`，方法规范必须是`static partial void Method(ref Regulus.Remote.IProtocol)`，否则编译不通过。

* * *

#### 服务器项目

创建服务器。`Server.csproj`

```powershell
Sample/Server>dotnet new console 
```

1.  添加参考

```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Server" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />	
</ItemGroup>
```

2.  实例化`IGreeter`

```csharp
namespace Server
{
	public class Greeter : Protocol.IGreeter
	{
		Regulus.Remote.Value<string> SayHello(string request)
		{
			// Return the received message
			return $"echo:{request}";
		}
	}
}
```

3.  服务器需要一个入口点来启动环境，创建一个继承自`Regulus.Remote.IEntry`.`Entry.cs`

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

4.  创建 TCP 服务

```csharp

namespace Server
{	
	static void Main(string[] args)
	{		
		// Get IProtocol with ProtocolCreater
		var protocol = Protocol.ProtocolCreater.Create();
		
		// Create Service
		var entry = new Entry();		
		
		var set = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
		int yourPort = 0;
		set.Listener.Bind(yourPort);
				
		//  Close service
		set.Listener.Close();
		set.Service.Dispose();
	}
}


```

* * *

#### 客户项目

创建客户端。`Client.csproj`.

```powershell
Sample/Client>dotnet new console 
```

1.  添加参考

```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Client" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
</ItemGroup>
```

2.  Create Tcp client

```csharp
namespace Client
{	
	static void Main(string[] args)
	{		
		// Get IProtocol with ProtocolCreater
		var protocol = Protocol.ProtocolCreater.Create();
				
		var set = Regulus.Remote.Client.Provider.CreateTcpAgent(protocol);
		
		bool stop = false;
		var task = System.Threading.Tasks.Task.Run(() => 
		{
			while (!stop)
			{
				set.Agent.Update();
			}
                
		});
		// Start Connecting
		EndPoint yourEndPoint = null;
		set.Connecter.Connect(yourEndPoint ).Wait();

		// SupplyEvent ,Receive add IGreeter.
		set.Agent.QueryNotifier<Protocol.IGreeter>().Supply += greeter => 
		{			
			greeter.SayHello("hello");
		};

		// SupplyEvent ,Receive remove IGreeter.
		set.Agent.QueryNotifier<Protocol.IGreeter>().Unsupply += greeter => 
		{
			
		};

		// Close
		stop = true;
		task.Wait();
		set.Connecter.Disconnect();
		set.Agent.Dispose();

	}
}
```

* * *

## 独立模式

为了方便开发和调试，提供了单机模式，无需连接即可运行系统。

```powershell
Sample/Standalone>dotnet new console 
```

1.  添加参考

```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Standalone" Version="0.1.11.0" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
	<ProjectReference Include="..\Server\Server.csproj" />
</ItemGroup>
```

2.  创建独立服务

```csharp
namespace Standalone
{	
	static void Main(string[] args)
	{		
		// Get IProtocol with ProtocolCreater
		var protocol = Protocol.ProtocolCreater.Create();
		
		// Create service
		var entry = new Entry();
		var service = Regulus.Remote.Standalone.Provider.CreateService(entry , protocol);
		var agent = service.Create();
		
		bool stop = false;
		var task = System.Threading.Tasks.Task.Run(() => 
		{
			while (!stop)
			{
				agent.Update();
			}
                
		});		
		
		agent.QueryNotifier<Protocol.IGreeter>().Supply += greeter => 
		{
		
			greeter.SayHello("hello");
		};
		
		agent.QueryNotifier<Protocol.IGreeter>().Unsupply += greeter => 
		{
			
		};

		// Close
		stop = true;
		task.Wait();
		
		agent.Dispose();
		service.Dispose();		

	}
}
```

* * *

## 自定义连接

如果要自定义连接系统，可以通过以下方式进行。

#### 客户

创建连接使用`CreateAgent`并实现接口`IStreamable`.

```csharp
var protocol = Protocol.ProtocolCreater.Create();
IStreamable stream = null ;// todo: Implementation Interface IStreamable
var service = Regulus.Remote.Client.CreateAgent(protocol , stream) ;
```

工具`IStreamable`

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

#### 服务器

创建服务使用`CreateService`并实现接口`IListenable`.

```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
IListenable listener = null; // todo: Implementation Interface IListenable
var service = Regulus.Remote.Server.CreateService(entry , protocol , listener) ;
```

实施`IListenable`

```csharp
namespace Regulus.Remote.Soul
{
    public interface IListenable
    {
		// When connected
        event System.Action<Network.IStreamable> StreamableEnterEvent;
		// When disconnected
        event System.Action<Network.IStreamable> StreamableLeaveEvent;
    }
}
```

* * *

## 自定义序列化

实施`ISerializable`

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

并将其带到服务器`CreateTcpService`

```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
ISerializable serializer = null; 
var service = Regulus.Remote.Server.CreateTcpService(entry , protocol , serializer) ;
```

并将其带给客户`CreateTcpAgent`

```csharp
var protocol = Protocol.ProtocolCreater.Create();
ISerializable serializer = null ;
var service = Regulus.Remote.Client.CreateTcpAgent(protocol , serializer) ;
```

* * *

## 样品

聊天室  
[关联](https://github.com/jiowchern/Regulus.Samples/tree/readme/Chat1)
