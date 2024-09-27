using System.Reflection;

namespace Regulus.Serialization.Dynamic
{
    public class Serializer : Regulus.Serialization.Serializer
    {
        public Serializer() : this(new StandardFinder())
        {

        }
        public Serializer(ITypeFinder finder) : base(new DescriberBuilder(finder).Describers)
        {
        }

        public Serializer(Regulus.Memorys.IPool pool) : base(new DescriberBuilder(new StandardFinder()).Describers , pool)
        {

        }
    }
}
