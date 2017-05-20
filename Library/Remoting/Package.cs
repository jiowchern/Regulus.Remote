using System;
using System.Collections.Generic;
using Regulus.Serialization;



namespace Regulus.Remoting
{




	[Serializable]	
	public class RequestPackage
	{
	
		public byte[] Data;


		public ClientToServerOpCode Code;
	}

	[Serializable]
	public class ResponsePackage
	{
	
		public byte[] Data;

	
		public ServerToClientOpCode Code;
	}
	[Serializable]
	public class TPackageData<TData> where TData : class
	{
		public byte[] ToBuffer(ISerializer serializer)
		{            
			return serializer.Serialize(this);
		}
	}
	


	public static class PackageHelper
	{
		public static TData ToPackageData<TData>(this byte[] buffer , ISerializer serializer) where TData : TPackageData<TData>
		{
			return serializer.Deserialize(buffer) as TData;
		}
	}


	[Serializable]
	public class PackageUpdateProperty : TPackageData<PackageUpdateProperty>
	{
		public PackageUpdateProperty()
		{
			EventName = String.Empty;
			Args = new byte[0];
		}


		public Guid EntityId;

		public string EventName;

		public byte[] Args;
	}

	[Serializable]
	public class PackageInvokeEvent : TPackageData<PackageInvokeEvent>
	{
		public PackageInvokeEvent()
		{
			EventParams = new byte[0][];
			EventName = String.Empty;
		}
	
		public Guid EntityId;

	
		public string EventName;

	
		public byte[][] EventParams;
	}

	[Serializable]
	public class PackageErrorMethod : TPackageData<PackageErrorMethod>
	{
		public PackageErrorMethod()
		{
			Method= String.Empty;
			Message = String.Empty;
		}
	
		public Guid ReturnTarget;
	
		public string Method;
	
		public string Message;
	}

	[Serializable]
	public class PackageReturnValue : TPackageData<PackageReturnValue>
	{
		public PackageReturnValue()
		{
			ReturnValue = new byte[0];
		}
	
		public Guid ReturnTarget    ;
	
		public byte[] ReturnValue   ;
	}

	[Serializable]
	public class PackageLoadSoulCompile : TPackageData<PackageLoadSoulCompile>
	{
		public PackageLoadSoulCompile ()
		{
			TypeName = String.Empty;
		}
	
		public string TypeName;
	
		public Guid EntityId;
	
		public Guid ReturnId;

	}
	[Serializable]
	public class PackageLoadSoul : TPackageData<PackageLoadSoul>
	{
		public PackageLoadSoul()
		{
			TypeName = String.Empty;
		}
	
		public string TypeName;
	
		public Guid EntityId;
	
		public bool ReturnType;
	}
	[Serializable]
	public class PackageUnloadSoul : TPackageData<PackageUnloadSoul>
	{
		public PackageUnloadSoul()
		{
			TypeName = String.Empty;
		}
	
		public string TypeName;
		
		public Guid EntityId;
	}


	[Serializable]
	public class PackageCallMethod : TPackageData<PackageCallMethod>
	{

		public PackageCallMethod()
		{
			MethodName = String.Empty;
			MethodParams = new byte[0][];
		}
	

		public Guid EntityId;
	

		public string MethodName;
	

		public Guid ReturnId;
	

		public byte[][] MethodParams;
	}

	[Serializable]
	public class PackageRelease : TPackageData<PackageRelease>
	{
	
		 public Guid EntityId;
	}


    public class PackageProtocolSubmit : TPackageData<PackageProtocolSubmit>
    {

        public byte[] VerificationCode;
        
    }

}
