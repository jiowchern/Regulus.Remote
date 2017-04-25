using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Remoting;
using Regulus.Serialization;

namespace Regulus.Protocol
{
    class Serializer : Regulus.Remoting.ISerializer
    {
        private Serialization.Serializer _Core;

        public Serializer(params Type[] types)
        {
            _Core = new Regulus.Serialization.Serializer(new DescriberBuilder(types));
        }
        byte[] ISerializer.Serialize(object instance)
        {
            return _Core.ObjectToBuffer(instance);
        }

        object ISerializer.Deserialize(byte[] buffer)
        {
            return _Core.BufferToObject(buffer);
        }
    }
}
