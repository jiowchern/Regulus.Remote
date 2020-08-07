namespace Regulus.Network
{
    public interface IObjectPool<TShell, TObject>
    {
        void New(out TShell Shell, out TObject Fillings);
        void Reset(out TShell shell, TObject Fillings);
    }
}