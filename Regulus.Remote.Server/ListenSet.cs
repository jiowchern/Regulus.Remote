namespace Regulus.Remote.Server
{
    public class ListenSet<TListener>
    {
        public ListenSet(TListener listener, Soul.IService service)
        {
            Listener = listener;
            Service = service;
        }

        public readonly TListener Listener;
        public readonly Soul.IService Service;

    }
}
