namespace Regulus.Remote
{
    public interface ISerializable
    {
        Regulus.Memorys.Buffer Serialize(System.Type type, object instance);
        object Deserialize(System.Type type, Regulus.Memorys.Buffer buffer);
    }
}