using System.Linq;

namespace Regulus.Serialization
{
    public class DescriberProvider
    {       
        public readonly IKeyDescriber KeyDescriber;
        public readonly IDescribersFinder TypeDescriberFinders;
        public DescriberProvider(IKeyDescriber key_describer, IDescribersFinder type_finder)
        {
            KeyDescriber = key_describer;
            TypeDescriberFinders = type_finder;            
        }

        public DescriberProvider(DescribersFinder finder) : this(finder.KeyDescriber , finder)
        {
            
        }
    }
}