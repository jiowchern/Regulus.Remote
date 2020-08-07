namespace Regulus.Remote
{
    public interface IProvider
    {
        IGhost[] Ghosts { get; }

        void Add(IGhost entiry);

        void Remove(long id);

        IGhost Ready(long id);

        void ClearGhosts();
    }
}
