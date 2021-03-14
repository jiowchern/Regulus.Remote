# Regulus Remote
[![Maintainability](https://api.codeclimate.com/v1/badges/99cb5e1dc12cafbfe451/maintainability)](https://codeclimate.com/github/jiowchern/Regulus.Remote/maintainability)
[![Actions Status](https://github.com/jiowchern/Regulus.Remote/workflows/Build/badge.svg)](https://github.com/jiowchern/Regulus.Remote/actions)
[![Build status](https://ci.appveyor.com/api/projects/status/vu0t2mp45vgfka06/branch/release?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus-remote/branch/release)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus.Remote/badge.svg?branch=master)](https://coveralls.io/github/jiowchern/Regulus.Remote?branch=master)
![commit last date](https://img.shields.io/github/last-commit/jiowchern/Regulus.Remote)  
[![Discord](https://img.shields.io/discord/101557008930451456.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/uDF8NTp)

<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->
<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->


## Introduce
This is server-client connection framework, available for Unity development.

## Feature
* Support .Net Standard 2.0
* Remote method invocation
* Support Unity il2cpp
## Latest Version
Download the latest ![Latest Version](https://img.shields.io/github/v/tag/jiowchern/Regulus) .
## Architecture
<!-- 
@startuml
!include <c4/C4_Context.puml> 

LAYOUT_LEFT_RIGHT()

System(Regulus,"Regulus Remote")
System(Server,"Server","Implement a interface IFoo.")
System(Client,"Client","Communicate with the server through this interface IFoo.")

Rel(Regulus,Client,"notice")
Rel(Server,Regulus,"bind")
@enduml
-->
![Architecture](http://www.plantuml.com/plantuml/svg/RO_DQiCm48JlUegjJWcOz99ZAIccQGs1WkCUUZAkF4q4qYf8g_wyVUNcfvUSPjZP7RvT9HcYPE_KZMNZyWYwCylRUjdNWGNViZYKy9wKsZoylRns_UPntVLjy_JSpUPARN-ImCzQxBBBgT8dGory4EZvUM6B-8bOwQFgIZV-uE31GWDP5iIFmO2QTGYB_wlQMR1K-gYwcE1zPci60NrPsPFiGUclizWOycuQsNIbKhLm5yvpCGUnPiASmXFZvPTtosE9Lk0nU_SB)

## Communication   
Instead of client communicating with server in packets, server send object to client through interface.  
Here are the steps to set up the communication.  
**A. Define the interface IGreeter.**  
```csharp
namespace Common
{
	public class HelloRequest
	{
		public string Name;
	}
	public class HelloReply
	{
		public string Message;
	}
	public interface IGreeter
	{
		Regulus.Remote.Value<HelloReply> SayHello(HelloRequest request);
	}
}
```
**B. Implement the greeter class.**  
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
**C. Use binder to send Greeter to the client.**  
```csharp
namespace Server
{
	public class Entry	
	{
		readonly Greeter _Greeter;
		readonly Regulus.Remote.IBinder _Binder;
		public Entry(Regulus.Remote.IBinder binder)
		{
			_Greeter = new Greeter();
			_Binder = binder;
			binder.Bind<IBinder>(_Greeter);
		}
		public void Dispose()
		{
			// you can call Unbind to notify the client to cancel the greeter.  
			_Binder.Unbind<IBinder>(_Greeter);
		}
	}
}
```
**D. Use an Agent to receive greeter from the server.**
```csharp
namespace Client
{
	class Entry
	{
		public Entry(Regulus.Remote.IAgent agent)
		{
			agent.QueryNotifier<Common.IGreeter>().Supply += _AddGreeter;
			agent.QueryNotifier<Common.IGreeter>().Unsupply += _RemoveGreeter;
		}
		void _AddGreeter(Common.IGreeter greeter)
		{
			// todo: Having received the greeter from the server, 			 
			//       begin to implement the following code.
		}
		void _RemoveGreeter(Common.IGreeter greeter)
		{
			// todo: The server has canceled the greeter.
		}
	}
}
```
---
In this way, the server and the client can communicate through the interface and achieve object-oriented development as much as possible.  
In addition, bind and unbind are used to switch the objects of the server, so as to control the access rights of the client conveniently.  
The current communication capabilities of the interface are as follows...  
* [Method](document/communications-method.md)
* [Event](document/communications-event.md)
* [Property](document/communications-property.md)
* [Notifier](document/communications-notifier.md)
> Serialization supports the following types...  
> ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, System.Guid, char, byte, enum, string``` and array of the types.
          

## Getting Start
This is a server-client framework, so it requires at least four projects: **Common**, **Protocol**, **Server** and **Client**.
### Dependency
* Visual Studio 2019.
* .NET Core Sdk 2.0 or above.

### Common And Protocol 
Create a protocol component to handle the communication requirements between client and server.  

**Create Common Project.**
```powershell
Sample/Common>dotnet new classlib 
```
Add references to **Common.csproj**.
```xml
<ItemGroup>
	<ProjectReference Include="Regulus\Regulus.Remote\Regulus.Remote.csproj" />
</ItemGroup>
```
Add a sample file,**IFoo.cs**.
```csharp
namespace Common
{
	public interface IFoo	
	{
	}
}
```
**Create Protocol Project.**
```powershell
Sample/Protocol>dotnet new classlib 
```
Add references to **Protocol.csproj**.
```xml
<ItemGroup>
	<ProjectReference Include="Regulus\Regulus.Remote\Regulus.Remote.csproj" />
	<ProjectReference Include="Regulus\Regulus.Serialization\Regulus.Serialization.csproj" />
	<ProjectReference Include="Common\Common.csproj" />
</ItemGroup>
```
**Generation the protocol code.**  
Use ```Regulus.Application.Protocol.CodeWriter.dll``` to generate code to **Protocol** project.
```powershell
Sample>dotnet run --project Regulus/Regulus.Application.Protocol.CodeWriter  --common Common.dll --output Protocol
```
At this point, there are two projects, **Common.csproj** and **Protocol.csproj**.
	
#### Server  
The following example sets up a server in console.  
```powershell
Sample/Server>dotnet new console 
```
Add references to **Server.csproj**.  
```xml
<ItemGroup>
	<ProjectReference Include="Regulus\Regulus.Remote.Server\Regulus.Remote.Server.csproj" />
	<ProjectReference Include="..\Common\Common.csproj" />	
</ItemGroup>
```
The server needs an entry point for the startup environment.  
Create a entry class that inherits from the ```Regulus.Remote.IEntry```.
```csharp
namespace Server
{
	public class Entry : Regulus.Remote.IEntry
	{
		void IBinderProvider.AssignBinder(IBinder binder)
		{
			// IBinder is what you get when your client completes the connection.
		}		
	}
}
```
Create service.
```csharp
namespace Server
{
	static void Main(string[] args)
	{
		var protocolAsm = Assembly.LoadFrom("Protocol.dll");
		// Create protocol.
		var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
		// your server entry.
		var entry = new Entry();
		
		// Create service.
		var service = Regulus.Remote.Server.Provider.CreateService(entry, protocol);
		
		entry.Run();
	
		service.Dispose();
	}
}
```
#### Client
The following example sets up a client in console.  
```powershell
Sample/Client>dotnet new console 
```
Add references to **Client.csproj**.  
```xml
<ItemGroup>
	<ProjectReference Include="Regulus\Regulus.Remote.Server\Regulus.Remote.Client.csproj" />
	<ProjectReference Include="..\Common\Common.csproj" />
</ItemGroup>
```
Create a ```Regulus.Remote.IAgent``` to handle the connection and receive objects from the server.
```csharp
var agent = Regulus.Remote.Client.Provider.CreateAgent(protocol);
// The agent uses single-thread continuations to process server requests and responses, so it needs to keep calling this method to stay operational. 
agent.Update(); 
```

Receive objects from the server side.
```csharp
var notifier = agent.QueryNotifier<Common.IFoo>();
notifier.Supply += _AddFoo; // The Supply is the Bind for the corresponding server.
notifier.Unsupply += _RemoveFoo;// The Unsupply is the Unbind for the corresponding server.
```

## Connection
By default, Tcp connectivity is provided.  
**Listener**  
```csharp
var listener = Regulus.Remote.Server.CreateTcp(service);
listener.Bind(port);
listener.Close()
```
**Connecter**
```csharp
var connecter = Regulus.Remote.Client.CreateTcp(agent);
var online = connecter.Connect(ipaddress);
if(online != null)
	// connect success.
else
	// connect failed.
```
## Extension
If you want to customize. Simply provide ```IService``` and ```IAgent``` a **data stream**.  
Implement ```Regulus.Network.IStreamable```.  
```csharp
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
        System.Threading.Tasks.Task<int> Receive(byte[] buffer, int offset, int count);
        /// <summary>
        ///     Send data streams.
        /// </summary>
        /// <param name="buffer">Stream instance.</param>
        /// <param name="offset">Start send position.</param>
        /// <param name="count">Count of byte send.</param>
        /// <returns>Actual count of byte send.</returns>
        System.Threading.Tasks.Task<int> Send(byte[] buffer, int offset, int count);
    }
}
```
**Server**
```csharp
class Server
{
	Regulus.Remote.Soul.IService _Service;
	// When listening on a connection.
	void Acctpt(Regulus.Network.IStreamable stream)
	{
		_Service.Join(stream);
	}
}
```
**Client**
```csharp
class Client
{
	void Start()
	{
		// custom connecter
		var connecter = CreateFromCustom();
		var agent = Regulus.Remote.Client.provider.CreateAgent(protocol);
		agent.Start(connecter);
		// begin your connect.
		connecter.Connect("127.0.0.1:21861");
	}
}
```
## Future Features
* Standalone serialization system.
* Add remote container.
* Integrate rx.
## Recommend
* [Regulus.Remote.CodeAnalysis](https://github.com/jiowchern/Regulus.Remote.CodeAnalysis) - Protocol syntax checker.
## Sample 
**[Regulus.Samples](https://github.com/jiowchern/Regulus.Samples)** ,This repository shows applications such as chat rooms.  




[![HitCount](http://hits.dwyl.com/jiowchern/Regulus.svg)](http://hits.dwyl.com/jiowchern/Regulus.Remote) 