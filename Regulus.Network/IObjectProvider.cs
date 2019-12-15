namespace Regulus.Network
{
    public interface IObjectProvider<out T>
    {
        T Spawn();
    }
}