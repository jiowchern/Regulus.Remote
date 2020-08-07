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
    }
}
