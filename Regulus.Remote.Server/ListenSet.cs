namespace Regulus.Remote.Server
{

    
    public class ListenSet<TListener , TService>
    {
        public ListenSet(TListener listener, TService service)
        {
            Listener = listener;
            Service = service;            
        }

        public readonly TListener Listener;
        public readonly TService Service;
        
    }
}
