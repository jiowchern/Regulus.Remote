namespace Regulus.Remote
{
    public interface IInternalSerializable
    {
        Regulus.Memorys.Buffer Serialize(object instance);
        object Deserialize(byte[] buffer);
    }
    
    


}
