﻿# Regulus Remote
[![Maintainability](https://api.codeclimate.com/v1/badges/99cb5e1dc12cafbfe451/maintainability)](https://codeclimate.com/github/jiowchern/Regulus.Remote/maintainability)
[![Actions Status](https://github.com/jiowchern/Regulus.Remote/workflows/Build/badge.svg)](https://github.com/jiowchern/Regulus.Remote/actions)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus.Remote/badge.svg?branch=master)](https://coveralls.io/github/jiowchern/Regulus.Remote?branch=master)
![commit last date](https://img.shields.io/github/last-commit/jiowchern/Regulus.Remote)    
<!-- [![Discord](https://img.shields.io/discord/101557008930451456.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/uDF8NTp) -->
<!-- [![Build status](https://ci.appveyor.com/api/projects/status/fv1owwit4utddawv/branch/release?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus-remote/branch/release) -->
<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->
<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->

![NuGet Downloads](https://img.shields.io/nuget/dt/Regulus.Remote)


## Introduction
Regulus Remote is a powerful and flexible server-client communication framework developed in C#. Designed to work seamlessly with the Unity game engine and any other .NET Standard 2.0 compliant environments, it simplifies network communication by enabling servers and clients to interact through interfaces. This object-oriented approach reduces the maintenance cost of protocols and enhances code readability and maintainability.  

Key features of Regulus Remote include support for IL2CPP and AOT, making it compatible with various platforms, including Unity WebGL. It provides default TCP connection and serialization mechanisms but also allows for customization to suit specific project needs. The framework supports methods, events, properties, and notifiers, giving developers comprehensive tools to build robust networked applications.  

With its stand-alone mode, developers can simulate server-client interactions without a network connection, facilitating development and debugging. Regulus Remote aims to streamline network communication in game development and other applications, enabling developers to focus more on implementing business logic rather than dealing with the complexities of network protocols.  



<!-- * Remote Method Invocation
* .Net Standard 2.0 base
* Compatible with Unity il2cpp
* Compatible with Unity WebGL
* Customizable connection
* Stand-alone mode  -->

## Feature
Server and client transfer through the interface, reducing the maintenance cost of the protocol.
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
## Supports
* Support **IL2CPP & AOT**.  
* Compatible with **.Net Standard2.0** or above development environment.
* **Tcp** connection is provided by default, and any connection can be customized according to your needs.
* **Serialization** is provided by default, and can be customized.
* Support **Unity3D WebGL**, provide server-side Websocket, client-side need to implement their own.
 
## Usage
1. Definition Interface ```IGreeter``` .
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
2. Server implemente ```IGreeter```.
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

3. Use ```IBinder.Bind``` to send the ```IGreeter``` to the client.
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

4. Client uses ```IAgent.QueryNotifier``` to obtain ```IGreeter```.
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
After completing the above steps, the server and client can communicate through the interface to achieve object-oriented development as much as possible.
#### Specification
**Interface**  
In addition to the above example ``IGreeter.SayHello``, there are a total of four ways to ...

<!-- In addition, bind and unbind are used to switch the objects of the server, so as to control the access rights of the client conveniently.  -->
* [```Method```](document/communications-method.md) <-- ```IGreeter.SayHello``` 
* [```Event```](document/communications-event.md)
* [```Property```](document/communications-property.md)
* [```Notifier```](document/communications-notifier.md)

**Serialization**  
For the types that can be serialized, see [```Regulus.Serialization```](Regulus.Serialization/README.md) instructions.
<!-- > Serialization supports the following types...  
> ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, char, byte, enum, string``` and array of the types. -->
          
---
## Getting Started
This is a server-client framework, so you need to create three projects : ```Protocol```, ```Server``` and ```Client```.

#### Requirements
* Visual Studio 2022  17.0.5 above.
* .NET Sdk 5 above. 

#### Protocol Project
Create common interface project ```Protocol.csproj```.
```powershell
Sample/Protocol>dotnet new classlib 
```
<!-- Add references to **Protocol.csproj**. -->
1. Add References
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote" Version="0.1.12.15" />
	<PackageReference Include="Regulus.Serialization" Version="0.1.12.12" />
	<PackageReference Include="Regulus.Remote.Tools.Protocol.Sources" Version="0.0.0.25">
		<PrivateAssets>all</PrivateAssets>
		<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
	</PackageReference>	
</ItemGroup>
```
2. Add interface, ```IGreeter.cs```
```csharp
namespace Protocol
{
	public interface IGreeter
	{
		Regulus.Remote.Value<string> SayHello(string request);
	}
}
```
3. Add ```ProtocolCreater.cs```.
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
This step is to generate the generator for the ``IProtocol``, which is an important component of the framework and is needed for communication between the server and the client.  
**_Note_**  
>> As shown in the code above, Add ```Regulus.Remote.Protocol``` attribute to the method you want to get ```IProtocol```, the method specification must be ```static partial void Method(ref Regulus.Remote.IProtocol)```, otherwise it will not pass compilation.

---
	
#### Server Project
Create the server. ```Server.csproj```
```powershell
Sample/Server>dotnet new console 
```
1. Add References
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Server" Version="0.1.12.13" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />	
</ItemGroup>
```
2. Instantiate ```IGreeter```
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
3. The server needs an entry point to start the environment , creating an entry point that inherits from ``Regulus.Remote.IEntry``. ```Entry.cs```
```csharp
namespace Server
{
	public class Entry : Regulus.Remote.IEntry
	{		
		void IBinderProvider.RegisterClientBinder(IBinder binder)
		{					
			binder.Binder<IGreeter>(new Greeter());
		}		
		void IBinderProvider.UnregisterClientBinder(IBinder binder)
		{
			// when client disconnect.
		}

		void IEntry.Update()
		{
			// Update
		}
	}
}
```
4. Create Tcp service
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
---
#### Client Project
Create Client. ```Client.csproj```.  
```powershell
Sample/Client>dotnet new console 
```
1. Add References
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Client" Version="0.1.12.12" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
</ItemGroup>
```
2. Create Tcp client
```csharp
namespace Client
{	
	static async Task Main(string[] args)
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
		var peer = await set.Connector.Connect(yourEndPoint );

		set.Agent.Enable(peer);

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
		set.Connector.Disconnect();
		set.Agent.Disable();

	}
}
```
---
## Standalone mode
In order to facilitate development and debugging, a standalone mode is provided to run the system without a connection.
```powershell
Sample/Standalone>dotnet new console 
```
1. Add References
```xml
<ItemGroup>
	<PackageReference Include="Regulus.Remote.Standalone" Version="0.1.12.14" />
	<ProjectReference Include="..\Protocol\Protocol.csproj" />
	<ProjectReference Include="..\Server\Server.csproj" />
</ItemGroup>
```
2.  Create standlone service
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
---
## Custom Connection
If you want to customize the connection system you can do so in the following way.
#### Client
Create a connection use ```CreateAgent``` and implement the interface ```IStreamable```.
```csharp
var protocol = Protocol.ProtocolCreater.Create();
IStreamable stream = null ;// todo: Implementation Interface IStreamable
var service = Regulus.Remote.Client.CreateAgent(protocol , stream) ;
```
implement ```IStreamable```.
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

#### Server

Create a service use ```CreateService``` and implement the interface ```IListenable```.
```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
IListenable listener = null; // todo: Implementation Interface IListenable
var service = Regulus.Remote.Server.CreateService(entry , protocol , listener) ;
```
implement ```IListenable```.
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
---
## Custom Serialization
implement ```ISerializable```.
```csharp
namespace Regulus.Remote
{
    public interface ISerializable
    {
        Regulus.Memorys.Buffer Serialize(System.Type type, object instance);
        object Deserialize(System.Type type, Regulus.Memorys.Buffer buffer);
    }
}
```
and bring it to the server ```CreateTcpService```.
```csharp
var protocol = Protocol.ProtocolCreater.Create();
var entry = new Entry();
ISerializable yourSerializer = null; 
var service = Regulus.Remote.Server.CreateTcpService(entry , protocol , yourSerializer) ;
```

and bring it to the client ```CreateTcpAgent```.
```csharp
var protocol = Protocol.ProtocolCreater.Create();
ISerializable yourSerializer = null ;
var service = Regulus.Remote.Client.CreateTcpAgent(protocol , yourSerializer) ;
```  

If need to know what types need to be serialized can refer ```Regulus.Remote.IProtocol.SerializeTypes```.  
```csharp
namespace Regulus.Remote
{
	public interface IProtocol
	{
		// What types need to be serialized.
		System.Type[] SerializeTypes { get; }
				
		System.Reflection.Assembly Base { get; }
		EventProvider GetEventProvider();
		InterfaceProvider GetInterfaceProvider();
		MemberMap GetMemberMap();
		byte[] VerificationCode { get; }
	}
}
```

---
## Example 
**Chat Room**  

A full example of the application can be found here, including the Unity and Stride3D version. [Link](https://github.com/jiowchern/Regulus.Samples/tree/readme/Chat1)
