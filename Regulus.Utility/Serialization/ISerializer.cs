namespace Regulus.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize(object instance);
        object Deserialize(byte[] buffer);
    }
}