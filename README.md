# Regulus Library

<!-- [![Build status](https://ci.appveyor.com/api/projects/status/2wtsf61u87qg62cc?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus)[![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)[![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library)   -->

[![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library)

這是一個基於 **Unity** 遊戲引擎開發的 **.Net Standard 2.0** 連線程式庫。雖然是說基於 Unity 開發的程式庫但是並**未使用**到 Unity 相關 API 因此也可以應用於任何支援 .Net Standard 2.0 的工具程式。

### 規格
* .Net Standard 2.0
* 主從式架構


### 使用

1. 定義要給**客戶端**的介面。  
	```csharp
	// Common.csproj
	public interface IChatable 
	{
		// 接收聊天訊息
		event Action<string,string> MessageEvent;
		// 傳送聊天訊息
		void Send(string name , string message);
	}
	```
2. 實作**服務端**內容， Chatter 繼承自 IChatable ，實現聊天功能。
	```csharp
	// Server.csproj
	namespace Chat.Server
	{
		public class Chatter : IChatable
		{
			// todo:實作聊天功能邏輯...			
		}
	}
	```
3. 透過 Regulus.Remote.**IBinder** 將 Chatter 傳送到**客戶端**。
	```csharp
	// Server.csproj
	namespace Chat.Server
	{
		// 某個類別的方法 ...
		public void Bind(Regulus.Remote.IBinder binder , Chatter chatter)
		{
			// 把 Chatter 送給客戶端
			binder.Bind<IChatable>(chatter);
		}
	}
	```
4. **客戶端**接收 Chatter 物件。
	```csharp
	// Client.csproj
	namespace Chat.Client
	{
		// 某個類別的方法 ...
		public void Sample(Regulus.Remote.IAgent agent)
		{
			// 接收到來自服務端的物件 
			agent.QueryNotifier<IChatable>().Supply += (chatter) =>{
				// 操作物件
				chatter.Send("Regulus","hello!");
			}
		}
	}
	```
