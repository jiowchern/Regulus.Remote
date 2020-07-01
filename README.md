# Regulus Library
[![Maintainability](https://api.codeclimate.com/v1/badges/78946edf1189eb49dfbd/maintainability)](https://codeclimate.com/github/jiowchern/Regulus/maintainability)
[![Build status](https://ci.appveyor.com/api/projects/status/2wtsf61u87qg62cc?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/Regulus/badge.svg?branch=)](https://coveralls.io/github/jiowchern/Regulus?branch=)
![commit last date](https://img.shields.io/github/last-commit/jiowchern/regulus)
[![Discord](https://img.shields.io/discord/101557008930451456)](https://discord.gg/uDF8NTp)
![Latest Version](https://img.shields.io/github/v/tag/jiowchern/Regulus)
<!-- [![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)![pre-release](https://img.shields.io/github/v/release/jiowchern/Regulus?include_prereleases) -->
<!-- [![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library) -->


### What is this doing?
This is server-client connection framework, available for Unity development.
### Feature
* Simple server-client framework.
* Support .Net Standard 2.0.
* Serialization.
* Remote method invocation.
* Support Unity il2cpp.
* Customizable packet transmission.
### Latest Version
Download the latest tag 0.1.7.1
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

### Remote Method Invocation.
Defining a common interface.
```csharp
// Common
namespace Common
{
	public interface IAdder 
	{				
		Regulus.Remote.Value<int> Add(int num1,int num2);
	}
}	
```
Implementation a method.
```csharp
// Server
namespace Server
{
	public class Adder : IAdder
	{
		Regulus.Remote.Value<int> IAdder.Add(int num1,int num2)
		{
			return num1+num2;
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
		agent.QueryNotifier<IAdder>().Supply += (adder) =>{
			// invoke
			var val = adder.Add(1,2);			
			// return value
			val.OnValue += (num)=> System.Console.WriteLine;
		};		
	}
}
```
### Remote Event 
Define events like this.  
The client can receive events from the server.
```csharp
// Common
namespace Common
{
	public interface ISampleInterface
	{				
		event System.Action<string> BroadcastEvent;
	}
}	
```

### Property Synchronize
Define a property.
```csharp
// Common
namespace Common
{
	public interface ISampleInterface
	{				
		Regulus.Remote.Property<int> Property {get;}
	}
}	
```
Implement property.
```csharp
namespace Server
{
	public class Sample : ISampleInterface
	{				
		readonly Regulus.Remote.Property<int> _Property;_
		public Sample()
		{
			_Property = new Regulus.Remote.Property<int>();
		}
		Regulus.Remote.Property<int> ISampleInterface.Property => _Property;
	}
}	
```
Set property value.
```csharp
namespace Server
{
	public class Sample : ISampleInterface
	{						
		public void ChangeProperty()
		{
			_Property.Value = 1;
		}
	}
}	
```
If invoke "ChangeProperty" you can get Property value is 1.
```csharp
// Client
namespace Client
{		
	public void Update(ISampleInterface sample)
	{
		if(sample.Property == 1)
		{
			// Console.WriteLine($"Property value is 1");
		}
	}
}
```
### Sample
Name|Description|Tcp|Rudp|Console|Unity
|-|-|-|-|-|-|
[Hello World](https://github.com/jiowchern/Regulus.Samples/tree/master/Helloworld)|Simple message response.|✔|❌|✔|❌
[Chat](https://github.com/jiowchern/Regulus.Samples/tree/master/Chat1)|Chat message broadcast.|✔|❌|✔|✔



