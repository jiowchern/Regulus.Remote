using System;
using System.Collections.Generic;
using Regulus.Serialization;

using ProtoBuf;

namespace Regulus.Remoting
{

    /*[ProtoContract][Serializable]
    public class Package
    {
        [ProtoMember(2)]
        public byte[] Data;

        [ProtoMember(1)]
        public ClientToServerOpCode Code;
    }*/


    [ProtoContract][Serializable]	
	public class RequestPackage
	{
		[ProtoMember(2)]
		public byte[] Data;

		[ProtoMember(1)]
		public ClientToServerOpCode Code;
	}

    [ProtoContract][Serializable]
    public class ResponsePackage
    {
        [ProtoMember(2)]
        public byte[] Data;

        [ProtoMember(1)]
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


    [ProtoContract][Serializable]
    public class PackageUpdateProperty : TPackageData<PackageUpdateProperty>
    {
        public PackageUpdateProperty()
        {
            EventName = String.Empty;
            Args = new byte[0];
        }

        [ProtoMember(1)]
        public Guid EntityId { get; set; }
        [ProtoMember(2)]
        public string EventName { get; set; }
        [ProtoMember(3)]
        public byte[] Args { get; set; }
    }

    [ProtoContract][Serializable]
    public class PackageInvokeEvent : TPackageData<PackageInvokeEvent>
    {
        public PackageInvokeEvent()
        {
            EventParams = new byte[0][];
            EventName = String.Empty;
        }
        [ProtoMember(1)]
        public Guid EntityId;

        [ProtoMember(2)]
        public string EventName;

        [ProtoMember(3)]
        public byte[][] EventParams;
    }

    [ProtoContract][Serializable]
    public class PackageErrorMethod : TPackageData<PackageErrorMethod>
    {
        public PackageErrorMethod()
        {
            Method= String.Empty;
            Message = String.Empty;
        }
        [ProtoMember(1)]
        public Guid ReturnTarget;
        [ProtoMember(2)]
        public string Method;
        [ProtoMember(3)]
        public string Message;
    }

    [ProtoContract][Serializable]
    public class PackageReturnValue : TPackageData<PackageReturnValue>
    {
        public PackageReturnValue()
        {
            ReturnValue = new byte[0];
        }
        [ProtoMember(1)]
        public Guid ReturnTarget    ;
        [ProtoMember(2)]
        public byte[] ReturnValue   ;
    }

    [ProtoContract][Serializable]
    public class PackageLoadSoulCompile : TPackageData<PackageLoadSoulCompile>
    {
        public PackageLoadSoulCompile ()
        {
            TypeName = String.Empty;
        }
        [ProtoMember(1)]
        public string TypeName;
        [ProtoMember(2)]
        public Guid EntityId;
        [ProtoMember(3)]
        public Guid ReturnId;

    }

    [ProtoContract][Serializable]
    public class PackageLoadSoul : TPackageData<PackageLoadSoul>
    {
        public PackageLoadSoul()
        {
            TypeName = String.Empty;
        }
        [ProtoMember(1)]
        public string TypeName;
        [ProtoMember(2)]
        public Guid EntityId;
        [ProtoMember(3)]
        public bool ReturnType;
    }
    [ProtoContract][Serializable]
    public class PackageUnloadSoul : TPackageData<PackageUnloadSoul>
    {
        public PackageUnloadSoul()
        {
            TypeName = String.Empty;
        }
        [ProtoMember(1)]
        public string TypeName;
        [ProtoMember(2)]
        public Guid EntityId;
    }


    [ProtoContract][Serializable]
    public class PackageCallMethod : TPackageData<PackageCallMethod>
    {

        public PackageCallMethod()
        {
            MethodName = String.Empty;
            MethodParams = new byte[0][];
        }
        [ProtoMember(1)]

        public Guid EntityId;
        [ProtoMember(2)]

        public string MethodName;
        [ProtoMember(3)]

        public Guid ReturnId;
        [ProtoMember(4)]

        public byte[][] MethodParams;
    }

    [ProtoContract][Serializable]
    public class PackageRelease : TPackageData<PackageRelease>
    {
        [ProtoMember(1)]
         public Guid EntityId;
    }
    
}
