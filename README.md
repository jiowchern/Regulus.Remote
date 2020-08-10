# Regulus Library
[![Maintainability](https://api.codeclimate.com/v1/badges/78946edf1189eb49dfbd/maintainability)](https://codeclimate.com/github/jiowchern/Regulus/maintainability)
[![Build status](https://ci.appveyor.com/api/projects/status/2wtsf61u87qg62cc?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus/badge.svg?branch=)](https://coveralls.io/github/jiowchern/Regulus?branch=)
![commit last date](https://img.shields.io/github/last-commit/jiowchern/regulus)
[![Discord](https://img.shields.io/discord/101557008930451456)](https://discord.gg/uDF8NTp)
[![HitCount](http://hits.dwyl.com/jiowchern/Regulus.svg)](http://hits.dwyl.com/jiowchern/Regulus)
<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->
<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->


## What is this doing?
This is server-client connection framework, available for Unity development.
## Feature
* Support .Net Standard 2.0
* Remote method invocation
* Support Unity il2cpp
* Customizable data stream
## Latest Version
Download the latest ![Latest Version](https://img.shields.io/github/v/tag/jiowchern/Regulus) .
## Architecture
<!-- 
@startuml 
!include <c4/C4_Component.puml> 
LAYOUT_WITH_LEGEND()

title System Landscape diagram for Regulus Library

System_Boundary(Regulus,"Regulus Library") {
    Container_Boundary(Remote,"Remote"){
        Component(RegulusClient,"Regulus.Remote.Client","Connect","Receive Ghost from the server.")
        Component(RegulusServer,"Regulus.Remote.Server","Listen","Create Regulus.Remote.IBinder for each socket.")
    }
    Container_Boundary(RegulusProtocol,"Protocol"){
        Component(RegulusCodeWriter,"Regulus.Application.Protocol.CodeWriter","Generate Tool","Refer to Common to generate Protocol code.")
    }
    
}

System_Boundary(Projects,"Projects") {
    Component(Client,"Client","Application")
    Component(Server,"Server","Application")
    Component(Common,"Common","Library","Define the interface between the server and the client.")
    Component(Protocol,"Protocol","A blank libaray project","Holds the generated protocol code.")
}

Rel_U(Server,RegulusServer,"bind","server object")
Rel_U(RegulusClient,Client,"notice","server object's ghost")
Rel_U(RegulusCodeWriter,Common,"read","parse define interfaces")
Rel_U(RegulusCodeWriter,Protocol,"write","build ghost code")
Rel_U(Protocol,Common,"ref","implementation")
Rel_U(Server,Common,"ref","implementation")
Rel_U(Client,Common,"ref","using")

Lay_D(Common,Protocol)
Lay_D(Common,Server)
Lay_D(Common,Client)
@enduml

-->
![PlantUML model](https://plantuml-server.kkeisuke.app/svg/ZLJ1Rjim3BtxApXVLWE1T-bn6DecHLh0s8QqHR6J8SjqebMM34bk4Gpzzr5MvCJ96imNo_JZu-CZxIlhc75zAo7v91INVOtmbLz-cL-MSzrsMg5oUKVllq5INF_wyVHOFjy_tfN5xVBs--8YIn8dd4Hus5g7BHHCrPQp3g4MR6rO2uqsiC9rBtiBXQWCCzia2UZoHlUgfi353Z1BZv1f1dyIe6kkbMD2eJdCQRL3d-BlQHQ00RmJ7dddKj3Jo9w7b3o4qrbAx0gvFwsGetX5M6wqTT0OtOBR85WqhsZoDFkuniC0EQaHmiHS26fP-M86cKCumjtV25MZ6Un2nZTWDNz15qk-V-p2GFFJQAUvbhCqdiuRecjyDi8T2hxkEYauSqAhF9BaUo0fNoALzjeVDV7xp8OKE-tvMwtyQHqXaG4uCKoxIDvF5u3Wlsa2Tj0_d0v67yN7COvpEv2ygx07ntcC0pW73WtuHV3tOLfX_sRf0XjoVDW2eSY7Xd642jqReZhO3Q357nxv82u_AV6F2P4cg2HJBo15nGpRGXUQfpTtMjPs88oMrlxjr5CoSeMoV8hD7grYHIj5L3k1kXgeirtAzDE8rYhj1CVZfCyMrlw3E4dUhr5qa9RRr-oOiVGF2DwDljap17j_tdo0EAfUo3eK7ZeUaqVelcX3UD5s4bjg8yvuuir_OQCR4snlXLeJ92dOjbp4NOaYicauL3iA1jeikK9Lqw_qBm00.svg)  

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
namesapce Server
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
          

## Set up
This is a server-client framework, so it requires at least four projects: **Common**, **Protocol**, **Server** and **Client**.
#### Prepare
Download the library.
```powershell
Sample>git submodule add https://github.com/jiowchern/Regulus.git Regulus
```
#### Protocol
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
		void IBootable.Launch()
		{
			Console.WriteLine("Server launch.");
		}

		void IBootable.Shutdown()
		{
			Console.WriteLine("Server shutdown.");
		}
	}
}
```
Set up the server.  
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
		int port = 1130;
		// Create service.
		var service = Regulus.Remote.Server.ServiceProvider.CreateTcp(entry, port, protocol);
		service.Launch();
		// You need to call Shutdown to release the resource when service ends.
		service.Shutdown();
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
var agent = Regulus.Remote.Client.AgentProvider.CreateTcp(protocolAsm);
```
<!-- Three methods that ```Regulus.Remote.IAgent``` must invoke.  
1.```Regulus.Remote.IAgent.Launch()``` and 2.```Regulus.Remote.IAgent.Shutdown()``` , Initialization and release of agents.  
3.```Regulus.Remote.IAgent.Update()``` , The agent uses single-thread continuations to process server requests and responses, so it needs to keep calling this method to stay operational.  -->
Receive objects from the server side.
```csharp
var notifier = agent.QueryNotifier<Common.IFoo>();
notifier.Supply += _AddFoo; // The Supply is the Bind for the corresponding server.
notifier.Unsupply += _RemoveFoo;// The Unsupply is the Unbind for the corresponding server.
```
Connect also get Regulus.Remote.IConnect using notifier.
```csharp
var notifier = agent.QueryNotifier<Regulus.Remote.IConnect>();
notifier.Supply += (c)=>c.Connect( ipaddress , port );
```







	



## Extension
#### Customizable data stream
If you need to customize the communication mode of the data stream, you can use the following method to expand.  
**Server**
```csharp
class CustomListener : Regulus.Network.IListenable
{
	// todo : Implement methods.
}
```
```csharp
var customListener = new CustomListener();
Regulus.Remote.Server.ServiceProvider.Create(entry , port , protocol , customListener );
```
**Client**
```csharp
class CustomConnectProvider : Regulus.Network.IConnectProvidable
{
	// todo : Implement methods.
}
```
```csharp
var customConnectProvider = new CustomConnectProvider();
Regulus.Remote.Client.AgentProvider.Create(protocol , customConnectProvider );
```

## Sample 
**[Regulus.Samples](https://github.com/jiowchern/Regulus.Samples)** ,This repository shows applications such as chat rooms.  




