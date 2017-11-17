namespace Regulus.Network
{
    public interface IRecycleable<T> 
    {
        void Reset(T Instance);
    }
}