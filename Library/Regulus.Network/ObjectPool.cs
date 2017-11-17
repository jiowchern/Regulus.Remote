namespace Regulus.Network
{
    public class  ObjectPool<TShell, TObject> : IObjectPool<TShell, TObject> 
        where TShell : IRecycleable<TObject> , new () 

    {
        private readonly IObjectProvider<TObject> _Provider;

        public ObjectPool(IObjectProvider<TObject> provider)
        {
            _Provider = provider;
        }

        void IObjectPool<TShell, TObject>.New(out TShell Shell, out TObject Fillings)
        {
            Shell = new TShell();
            Fillings = _Provider.Spawn();
            Shell.Reset(Fillings);
        }

        void IObjectPool<TShell, TObject>.Reset(out TShell shell, TObject Fillings)
        {
            shell = new TShell();
            shell.Reset(Fillings);            
        }
    }
}