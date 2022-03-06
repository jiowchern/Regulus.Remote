using Regulus.Serialization;
using System;



namespace Regulus.Remote
{


    [Serializable]
    public class RequestPackage
    {
        public RequestPackage()
        {

        }
        public byte[] Data;


        public ClientToServerOpCode Code;
    }

    [Serializable]
    public class ResponsePackage
    {
        public ResponsePackage()
        {

        }
        public byte[] Data;


        public ServerToClientOpCode Code;
    }
    [Serializable]
    public class TPackageData<TData> where TData : class
    {
        public byte[] ToBuffer(IInternalSerializable serializer)
        {
            return serializer.Serialize(this);
        }
    }



    public static class PackageHelper
    {
        public static TData ToPackageData<TData>(this byte[] buffer, IInternalSerializable serializer) where TData : TPackageData<TData>
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

    
    public class PackagePropertySoul : TPackageData<PackagePropertySoul>
    {
        public PackagePropertySoul()
        {

        }
        public long OwnerId;
        public long EntiryId;
        public int PropertyId;
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

        public long EntityId;        

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
        public PackageRelease()
        {

        }
        public long EntityId;
    }


    public class PackageProtocolSubmit : TPackageData<PackageProtocolSubmit>
    {
        public PackageProtocolSubmit()
        {

        }
        public byte[] VerificationCode;

    }

    public class PackageAddEvent : TPackageData<PackageAddEvent>
    {

        public PackageAddEvent()
        {

        }
        public long Entity;
        public int Event;
        public long Handler;

    }

    public class PackageRemoveEvent : TPackageData<PackageRemoveEvent>
    {
        public PackageRemoveEvent()
        {

        }
        public long Entity;
        public int Event;
        public long Handler;
    }
    
    


}
