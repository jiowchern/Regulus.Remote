# Regulus Library
[![Maintainability](https://api.codeclimate.com/v1/badges/78946edf1189eb49dfbd/maintainability)](https://codeclimate.com/github/jiowchern/Regulus/maintainability)
[![Build status](https://ci.appveyor.com/api/projects/status/2wtsf61u87qg62cc?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus/badge.svg?branch=)](https://coveralls.io/github/jiowchern/Regulus?branch=)
![commit last date](https://img.shields.io/github/last-commit/jiowchern/regulus)
[![Discord](https://img.shields.io/discord/101557008930451456)](https://discord.gg/uDF8NTp)
<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->
<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->


### What is this doing?
This is server-client connection framework, available for Unity development.

### Feature
* .Net Standard 2.0.
* Serialization.
* Remote method invocation.
* Unity il2cpp.
* Simulate a stand-alone environment.
* Customize the stream transport.

### Architecture
<!-- 
@startuml
package "Project" {
rectangle "Common(dll)" as Common
rectangle "Server" as Server
rectangle "Client" as Client

}

package "Regulus Library" {
rectangle "Regulus.Remote.Client.dll" as Regulus.Remote.Client
rectangle "Regulus.Remote.Server.dll" as Regulus.Remote.Server
}

[Regulus.Remote.Server] <--- [Server]
[Regulus.Remote.Client] <--- [Client]
[Common] <.. [Client]
[Common] <.. [Server]

note left   of [Client]
 Unity or other compatible c#
 NetStandard2.0 project.
end note

note left   of [Server]
   Server-side game logic.
end note

note left   of [Common] 
   A common defined by the server and the client.
end note
@enduml
-->
![PlantUML model](http://www.plantuml.com/plantuml/svg/VL6xJiGm4Epz5QFGG14BKLCSeaK8tOdgHE7Ocs3m8t8Sf0ZnxpWsKJZTeyhEpcCdoMQ88iJH6jOB-IawGlKI_0V9ME6RXVGKhZDf--YjzUvQ6NDJGGme-BzYH-6BGYRBU60tcbpCP1aP-s7hpIrrena7FEacY30TtbvOlYNh8_4Im5ELd7UIlM0lvSxP2pkNsvzatd1VrpNsV-X8LSulgeAIgdokjERyt7P9P2xbm50R0VXsbUFLwJZ11_ZuJW7Isrv4tHY2l69ufhYBmYaHr1s_HLz-8sVa5ER8u-3bOe9bh0Uj29smIUOxBI-Pb-wp-m4o8oXgjIE5PaAgY26d8fNAKEONMJCtQHgj-GK0)  

### Remote method invocation.
Defining a common interface.
```csharp
// Common
namespace Common
{
	public interface IChatable 
	{				
		void Send(string name , string message);
	}
}	
```
Implementation a method .
```csharp
// Server
namespace Server
{
	public class Chatter : IChatable
	{
		void IChatable.Send(string message)
		{
			//TODO: 
		}
	}
}
```
Receives an object from the server and invoke it.
```csharp
// Client
namespace Client
{		
	public void AMethod(Regulus.Remote.IAgent agent)
	{
		// receive
		agent.QueryNotifier<IChatable>().Supply += (chatter) =>{
			// invoke
			chatter.Send("hello!");				
		};		
	}
}
```




### Sample
Description|Tcp|Rudp|Console Client|Unity|Standalone
-|:-|:-|:-|:-|:-
[ChatRoom](https://github.com/jiowchern/Regulus.Samples/tree/master/Chat1)|✔|❌|✔|✔|❌


