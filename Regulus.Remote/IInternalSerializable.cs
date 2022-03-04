namespace Regulus.Remote
{
    public interface IInternalSerializable
    {
        byte[] Serialize(object instance);
        object Deserialize(byte[] buffer);
    }
    
    


}
