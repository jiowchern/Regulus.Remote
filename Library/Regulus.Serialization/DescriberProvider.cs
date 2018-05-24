using System.Linq;

namespace Regulus.Serialization
{
    public class DescriberProvider
    {
        

        public readonly IntDescribers _KeyDescribers;
        public readonly IDescribersFinder _TypeDescribers;

        public DescriberProvider(params ITypeDescriber[] describers)
        {            
            _KeyDescribers = new IntDescribers(describers);
            _TypeDescribers = new DescribersFinder(describers);

            foreach (var typeDescriber in describers)
                typeDescriber.SetFinder(_TypeDescribers);
        }

    }
}