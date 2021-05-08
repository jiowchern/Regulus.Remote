namespace Regulus.Remote
{
    public interface ISoul
    {
        object Instance { get; }
        long Id { get; }

        bool IsTypeObject(TypeObject obj);
    }
}

