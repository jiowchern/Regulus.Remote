using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{
    public class DescriberBuilder
    {
        public readonly ITypeDescriber[] Describers;
        public DescriberBuilder(params Type[] types)
        {
            Describers = _BuildDescribers(types);
        }
        ITypeDescriber[] _BuildDescribers(params Type[] types)
        {
            int id = 0;
            var describers = new List<ITypeDescriber>();
            foreach (var type in types)
            {
                var identifier = new TypeIdentifier(type);

                if (identifier.Type == TypeIdentifier.TYPE.STRING  )
                {
                    describers.Add(new StringDescriber(++id));
                }
                else if (identifier.Type == TypeIdentifier.TYPE.NUMBER)
                {
                    describers.Add(new NumberDescriber(++id, type));
                }
                else if (identifier.Type == TypeIdentifier.TYPE.ARRAY)
                {
                    describers.Add(new ArrayDescriber(++id, type));
                }
                else if (identifier.Type == TypeIdentifier.TYPE.CLASS)
                {
                    describers.Add(new ClassDescriber(++id, type));
                }
                else if (identifier.Type == TypeIdentifier.TYPE.ENUM)
                {
                    describers.Add(new EnumDescriber(++id, type));
                }
                else if(identifier.Type == TypeIdentifier.TYPE.BITTABLE)
                {
                    describers.Add(new StructDescriber(++id, type));
                }
                else 
                    throw new Exception("Can not describe the type. " + type.FullName);
            }

            return describers.ToArray();
        }
      
    }
}
