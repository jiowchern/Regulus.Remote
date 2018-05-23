namespace Regulus.Serialization
{
    public interface ITypeDescriberProvider<TKey>
    {
        ITypeDescriberFinder<TKey> GetKeyFinder();
        ITypeDescriberFinder<System.Type> GetTypeFinder();
    }
}