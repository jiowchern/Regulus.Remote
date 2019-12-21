using System;

namespace Regulus.Serialization
{

    public class DescriberException : Exception
    {
        public readonly Type Describer;

        public readonly Type Instance;
        

        public readonly Exception Next;

        

        public DescriberException(Type describer, Type instance,  string message) : base(message)
        {
            Describer = describer;
            Instance = instance;
            
        }

        public DescriberException(Type describer, Type instance, string message , Exception next) : base(message)
        {
            Describer = describer;
            Instance = instance;            
            Next = next;
        }
    }
    
}