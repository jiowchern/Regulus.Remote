using System;
using System.Collections.Generic;


using ProtoBuf;

namespace Regulus.Remoting
{

    [ProtoContract]
    public class Package
    {
        [ProtoMember(2)]
        public byte[] Data;

        [ProtoMember(1)]
        public ClientToServerOpCode Code;
    }


    [ProtoContract]	
	public class RequestPackage
	{
		[ProtoMember(2)]
		public byte[] Data;

		[ProtoMember(1)]
		public ClientToServerOpCode Code;
	}

    [ProtoContract]
    public class ResponsePackage
    {
        [ProtoMember(2)]
        public byte[] Data;

        [ProtoMember(1)]
        public ServerToClientOpCode Code;
    }

    public class TPackageData<TData> where TData : class
    {
        public byte[] ToBuffer()
        {
            return Regulus.TypeHelper.Serializer<TData>(this as TData);
        }
    }
    


    public static class PackageHelper
    {
        public static TData ToPackageData<TData>(this byte[] buffer) where TData : TPackageData<TData>
        {
            return Regulus.TypeHelper.Deserialize<TData>(buffer);
        }
    }


    [ProtoContract]
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

    [ProtoContract]
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

    [ProtoContract]
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

    [ProtoContract]
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

    [ProtoContract]
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

    [ProtoContract]
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
    [ProtoContract]
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


    [ProtoContract]
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

    [ProtoContract]
    public class PackageRelease : TPackageData<PackageRelease>
    {
        [ProtoMember(1)]
         public Guid EntityId;
    }
    
}
