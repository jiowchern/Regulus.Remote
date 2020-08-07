using Regulus.Serialization;
using System;



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
        public static TData ToPackageData<TData>(this byte[] buffer, ISerializer serializer) where TData : TPackageData<TData>
        {
            return serializer.Deserialize(buffer) as TData;
        }
    }

    public class PackageSetProperty : TPackageData<PackageSetProperty>

    {
        public PackageSetProperty()
        {
            Value = new byte[0];
        }
        public long EntityId;
        public int Property;
        public byte[] Value;
    }


    [Serializable]
    public class PackageSetPropertyDone : TPackageData<PackageSetPropertyDone>
    {
        public PackageSetPropertyDone()
        {
        }
        public long EntityId;
        public int Property;
    }

    [Serializable]
    public class PackageInvokeEvent : TPackageData<PackageInvokeEvent>
    {
        public PackageInvokeEvent()
        {
            EventParams = new byte[0][];

        }

        public long EntityId;


        public int Event;
        public long HandlerId;


        public byte[][] EventParams;
    }

    [Serializable]
    public class PackageErrorMethod : TPackageData<PackageErrorMethod>
    {
        public PackageErrorMethod()
        {
            Method = String.Empty;
            Message = String.Empty;
        }

        public long ReturnTarget;

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

        public long ReturnTarget;

        public byte[] ReturnValue;
    }

    [Serializable]
    public class PackageLoadSoulCompile : TPackageData<PackageLoadSoulCompile>
    {
        public PackageLoadSoulCompile()
        {

        }

        public int TypeId;

        public long EntityId;

        public long ReturnId;
        public long PassageId;

    }
    [Serializable]
    public class PackageLoadSoul : TPackageData<PackageLoadSoul>
    {
        public PackageLoadSoul()
        {

        }

        public int TypeId;

        public long EntityId;

        public bool ReturnType;
    }
    [Serializable]
    public class PackageUnloadSoul : TPackageData<PackageUnloadSoul>
    {
        public PackageUnloadSoul()
        {

        }

        public int TypeId;

        public long EntityId;

        public long PassageId;

    }


    [Serializable]
    public class PackageCallMethod : TPackageData<PackageCallMethod>
    {

        public PackageCallMethod()
        {

            MethodParams = new byte[0][];
        }


        public long EntityId;


        public int MethodId;


        public long ReturnId;


        public byte[][] MethodParams;
    }

    [Serializable]
    public class PackageRelease : TPackageData<PackageRelease>
    {

        public long EntityId;
    }


    public class PackageProtocolSubmit : TPackageData<PackageProtocolSubmit>
    {

        public byte[] VerificationCode;

    }

    public class PackageAddEvent : TPackageData<PackageAddEvent>
    {
        public long Entity;
        public int Event;
        public long Handler;

    }

    public class PackageRemoveEvent : TPackageData<PackageRemoveEvent>
    {
        public long Entity;
        public int Event;
        public long Handler;
    }
    public class PackageNotifierEvent : TPackageData<PackageNotifierEvent>
    {
        public long Entity;
        public int Property;
        public long Passage;

    }
}
