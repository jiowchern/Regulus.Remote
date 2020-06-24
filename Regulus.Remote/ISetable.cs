namespace Regulus.Remote
{
    internal interface IAccessable
    {
        void Set(object value);
        object Get();
    }
}