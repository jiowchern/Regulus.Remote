namespace Regulus.Remote
{
    public interface ISerializable
    {
        byte[] Serialize(System.Type type, object instance);
        object Deserialize(System.Type type, byte[] buffer);
    }
}