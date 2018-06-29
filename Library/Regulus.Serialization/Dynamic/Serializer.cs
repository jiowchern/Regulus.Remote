using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Serialization.Dynamic
{
    public class Serializer
    {
        private readonly DescribersFinder _KeyFinder;

        public Serializer() : this(new StandardFinder())
        {
            
        }
        public Serializer(ITypeFinder finder)
        {
            _KeyFinder = new DescribersFinder(finder);            
        }

        

        public byte[] ObjectToBuffer(object instance)
        {

            if (instance == null)
            {
                return _Null();
            }
            var id = instance.GetType().FullName;
            var des =  _KeyFinder.Get(id);
            var idDes = _KeyFinder.Get(typeof(string).FullName);
            var idByteLen = idDes.GetByteCount(id);
            var instanceByteLen = des.GetByteCount(instance);
            var buffer = new byte[idByteLen + instanceByteLen];
            var readed = idDes.ToBuffer(id, buffer, 0);
            readed  += des.ToBuffer(instance, buffer, readed);
            return buffer;
        }

        private byte[] _Null()
        {
            var idDes = _KeyFinder.Get(typeof(string).FullName);
            var idByteLen = idDes.GetByteCount("");
            var buffer = new byte[idByteLen];
            idDes.ToBuffer("" , buffer, 0);
            return buffer;
        }

        public object BufferToObject(byte[] buffer)
        {
            var idDes = _KeyFinder.Get(typeof(string).FullName);
            object idObj;
            var readed = idDes.ToObject(buffer, 0, out idObj);
            string id = idObj as string;

            var instanceDes =  _KeyFinder.Get(id);
            if (instanceDes == null)
                return null;
            object instanceObject;
            readed += instanceDes.ToObject(buffer, readed, out instanceObject);
            return instanceObject;
        }
    }

    public class StandardFinder : ITypeFinder
    {
        Type ITypeFinder.Find(string type_name)
        {
            var retType   = Type.GetType(type_name);
            if (retType != null)
                return retType;


            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                let type = (from t in asm.GetTypes() where t.FullName == type_name select t).FirstOrDefault()
                where type != null
                select type).FirstOrDefault();
        }
    }
}
