namespace Regulus.Remote
{
    public interface IProvider
    {

        System.Collections.Generic.IReadOnlyCollection<IGhost> Ghosts { get; }

        void Add(IGhost entiry);

        void Remove(long id);

        IGhost Ready(long id);

        void ClearGhosts();
    }
}
