namespace Regulus.Remote.Server
{
    public class Integral<TListener>
    {
        public Integral(TListener listener, Soul.IService service)
        {
            Listener = listener;
            Service = service;
        }

        public readonly TListener Listener;
        public readonly Soul.IService Service;

    }
}
