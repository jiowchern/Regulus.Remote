# Regulus Library
<!-- [![Build status](https://ci.appveyor.com/api/projects/status/2wtsf61u87qg62cc?svg=true)](https://ci.appveyor.com/project/jiowchern/regulus)[![GitHub release](https://img.shields.io/github/release/jiowchern/regulus.svg?style=flat-square)](https://github.com/jiowchern/Regulus/releases)[![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library)   -->
[![Gitter](https://badges.gitter.im/JoinChat.svg)](https://gitter.im/Regulus-Library)
> The following instructions are written in native language, please mail to me if you need the English version.



### 簡介
這是一個基於 **Unity** 遊戲引擎開發的 **.Net Standard 2.0** 連線程式庫。雖然是說基於 Unity 開發的程式庫但是並**未使用**到 Unity 相關 API 因此也可以應用於任何支援 .Net Standard 2.0 的工具程式。

### 規格
* .Net Standard 2.0
* 主從式架構

### 關係
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
 Unity 或是其他相容 c# 
.NetStandard2.0 的環境
end note

note left   of [Server]
   服務端的遊戲邏輯
end note

note left   of [Common] 
   服務端與客戶端定義的共用物件
end note
@enduml
-->
![PlantUML model](http://www.plantuml.com/plantuml/svg/VP3FIWCn4CRlUOgXNlJG89uz5BoAI8kduY5sqrBDPYND2aKy-AUuAYA85AsUUh88hNeeIiMdQJRjMvXkNDR5fQVCp3VVRpvIQ4WYfEyoj4ygUwH68RSfl5rQaJauHCAyXDUOcQvvhklnHOUnfHoG1jZ-xqQ9YWCg8j6MAJkhKouZqPO87Q7aPf7MVEOtOBs-8uXefc_7AYvIrvCKMm0sKI9UfZh7RiDbssDr5gwSgMp3QZwVX_9lXygPv-Cjsvaj-rrcZ77sD24YRZZ0Q5K2W5TDrc6BrsKJmg0TtWzKQpWUjVNZX-f_GcK14DwWiYG9TuXmLl0owCwPldrLtLD4kGCpvdzoVDAquTErkdhGBmFZcnvVXi7xzEHcyZMOuuxJf-oJu5prks4mDBNgT_Htpm1LwqtVFUkBtdWqJ805K_ak-m40)
P.S1 虛線為內部繼承關係，實線為外部引用。  
P.S2 此圖為主要引用物件，在建立專案時需要依需求自行引入參考。
### 建立
1. 建立公用邏輯專案。  
  > dotnet new classlib -n Common 

2. 實作服務端業務邏輯      
   ```csharp
    namespace Server
    {
        // 此物件是服務器業務邏輯實作
        // 繼承 Regulus.Remote.IEntry
        public class Logic : Regulus.Remote.IEntry
        {
            void IBootable.Launch()
            {
                // Todo : 服務啟動時候 ...
            }
            void IBootable.Shutdown()
            {
                // Todo : 服務關閉時候 ...
            }
            void IBinderProvider.AssignBinder(IBinder binder)
            {
                // Todo : 當有連線發生時 ...
            }
        }
    }
    ```
3. 建立服務  
    ```csharp
    namespace Server
    {
        public void CreateService()
        {
            var service = Regulus.Remote.Server.ServiceProvider.CreateTcp(/*Port*/, new Server.Logic() ,Essential.CreateFromDomain(/* Common Assembly */) );
            service.Launch(); // 啟動服務
            service.Shutdown(); // 關閉服務
        }
    }
    ```
4. 建立客戶端元件  
    ```csharp
    namespace Client
    {
        public void CreateAgent()
        {
            var agent = Regulus.Remote.Client.JIT.AgentProivder.CreateTcp(Essential.CreateFromDomain(/* Common Assembly */));
            agent.Launch(); // 啟動
            agent.Update(); // 每幀調用
            var result = agent.Connect(/*System.Net.IPEndPoint*/);
            result.OnValue += (connect_result) =>{
                // connect_result 連線結果
            };
            agent.Shutdown(); // 關閉
        }
    }
    ```

### 傳輸

1. 定義要給**客戶端**的介面。  
	```csharp
	// Common
    namespace Common
    {
        public interface IChatable 
	    {
		    // 接收聊天訊息
		    event Action<string,string> MessageEvent;
		    // 傳送聊天訊息
		    void Send(string name , string message);
	    }
    }
	
	```
2. 實作**服務端**內容， Chatter 繼承自 IChatable ，實現聊天功能。
	```csharp
	// Server
	namespace Server
	{
		public class Chatter : IChatable
		{
			// todo:實作聊天功能邏輯...			
		}
	}
	```
3. 透過 Regulus.Remote.**IBinder** 將 Chatter 傳送到**客戶端**。
	```csharp
	// Server
	namespace Server
	{
        public class Logic : Regulus.Remote.IEntry
        {
            // 某個類別的方法 ...
		    public void Bind(Regulus.Remote.IBinder binder , Chatter chatter)
		    {
			    // 把 Chatter 送給客戶端
			    binder.Bind<IChatable>(chatter);
		    }
        }
		    
	}
	```
4. **客戶端**接收 Chatter 物件。
	```csharp
	// Client
	namespace Client
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


### 範例
Link|Description|Socket|Client|Mode
-|:-|:-|:-|:-
[Chat1](https://github.com/jiowchern/Regulus.Samples/tree/master/Chat1)|聊天室|Tcp|JIT|Console


