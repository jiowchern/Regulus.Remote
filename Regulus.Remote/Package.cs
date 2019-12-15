using System;
using System.Collections.Generic;
using Regulus.Serialization;



namespace Regulus.Remote
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
			Args = new byte[0];
		}


		public Guid EntityId;

		public int Property;

		public byte[] Args;
	}

	[Serializable]
	public class PackageInvokeEvent : TPackageData<PackageInvokeEvent>
	{
		public PackageInvokeEvent()
		{
			EventParams = new byte[0][];
			
		}
	
		public Guid EntityId;

	
		public int Event;

	
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
			
		}
	
		public int TypeId;
	
		public Guid EntityId;
	
		public Guid ReturnId;

	}
	[Serializable]
	public class PackageLoadSoul : TPackageData<PackageLoadSoul>
	{
		public PackageLoadSoul()
		{
			
		}
	
		public int TypeId;
	
		public Guid EntityId;
	
		public bool ReturnType;
	}
	[Serializable]
	public class PackageUnloadSoul : TPackageData<PackageUnloadSoul>
	{
		public PackageUnloadSoul()
		{
			
		}
	
		public int TypeId;
		
		public Guid EntityId;
	}


	[Serializable]
	public class PackageCallMethod : TPackageData<PackageCallMethod>
	{

		public PackageCallMethod()
		{
			
			MethodParams = new byte[0][];
		}
	

		public Guid EntityId;
	

		public int MethodId;
	

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
