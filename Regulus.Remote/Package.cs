using Regulus.Serialization;
using System;



namespace Regulus.Remote
{


    
    public class RequestPackage
    {
        public RequestPackage()
        {

        }
        public byte[] Data;


        public ClientToServerOpCode Code;
    }

    
    public class ResponsePackage
    {
        public ResponsePackage()
        {

        }
        public byte[] Data;


        public ServerToClientOpCode Code;
    }


    public class PackageSetProperty 

    {
        public PackageSetProperty()
        {
            Value = new byte[0];
        }
        public long EntityId;
        public int Property;
        public byte[] Value;
    }


    
    public class PackageSetPropertyDone 
    {
        public PackageSetPropertyDone()
        {
        }
        public long EntityId;
        public int Property;
    }

    
    public class PackageInvokeEvent 
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

    
    public class PackageErrorMethod 
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

    
    public class PackageReturnValue 
    {
        public PackageReturnValue()
        {
            ReturnValue = new byte[0];
        }

        public long ReturnTarget;

        public byte[] ReturnValue;
    }

    
    public class PackagePropertySoul
    {
        public PackagePropertySoul()
        {

        }
        public long OwnerId;
        public long EntiryId;
        public int PropertyId;
    }
    
    public class PackageLoadSoulCompile 
    {
        public PackageLoadSoulCompile()
        {

        }
        public int TypeId;
        public long EntityId;
        public long ReturnId;        
    }
    
    public class PackageLoadSoul 
    {
        public PackageLoadSoul()
        {

        }

        public int TypeId;

        public long EntityId;

        public bool ReturnType;
    }
    
    public class PackageUnloadSoul 
    {
        public PackageUnloadSoul()
        {

        }

        public long EntityId;        

    }


    
    public class PackageCallMethod 
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

    
    public class PackageRelease 
    {
        public PackageRelease()
        {

        }
        public long EntityId;
    }


    public class PackageProtocolSubmit 
    {
        public PackageProtocolSubmit()
        {

        }
        public byte[] VerificationCode;

    }

    public class PackageAddEvent 
    {

        public PackageAddEvent()
        {

        }
        public long Entity;
        public int Event;
        public long Handler;

    }

    public class PackageRemoveEvent 
    {
        public PackageRemoveEvent()
        {

        }
        public long Entity;
        public int Event;
        public long Handler;
    }
    
    


}
