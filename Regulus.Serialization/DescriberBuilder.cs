using Regulus.Serialization.Dynamic;
using System;

namespace Regulus.Serialization
{
    public class DescriberBuilder
    {
        public readonly DescriberProvider Describers;
        public DescriberBuilder(params Type[] types)
        {
            Describers = _BuildDescribers(types);
        }

        public DescriberBuilder(ITypeFinder type_finder)
        {
            Describers = _BuildDescribers(type_finder);
        }

        DescriberProvider _BuildDescribers(ITypeFinder type_finder)
        {
            Dynamic.DescribersFinder describersFinder = new Dynamic.DescribersFinder(type_finder);
            return new DescriberProvider(describersFinder);
        }
        DescriberProvider _BuildDescribers(params Type[] types)
        {

            DescribersFinder finder = new DescribersFinder(types);
            return new DescriberProvider(finder.KeyDescriber, finder);
        }

    }
}
