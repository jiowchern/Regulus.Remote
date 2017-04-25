namespace Regulus.Remoting
{
    public interface ISerializer
    {
        byte[] Serialize   (object instance);
        object Deserialize(byte[] buffer);
    }
}