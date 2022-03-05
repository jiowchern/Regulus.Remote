using Regulus.Network;
using Regulus.Network.Web;
using Regulus.Remote.Soul;
using System;

namespace Regulus.Remote.Server.Web
{
    public class Listener : Soul.IListenable
    {
        
        readonly Regulus.Network.Web.Listener _Listener;
        readonly Regulus.Remote.NotifiableCollection<IStreamable> _NotifiableCollection;
        public Listener()
        {
        
            _NotifiableCollection = new NotifiableCollection<IStreamable>();
            _Listener = new Network.Web.Listener();

            _Listener.AcceptEvent += _Join;

        }

        event Action<IStreamable> Soul.IListenable.StreamableEnterEvent
        {
            add
            {
                _NotifiableCollection.Notifier.Supply += value;
            }

            remove
            {
                _NotifiableCollection.Notifier.Supply -= value;
            }
        }

        event Action<IStreamable> Soul.IListenable.StreamableLeaveEvent
        {
            add
            {
                _NotifiableCollection.Notifier.Unsupply += value;
            }

            remove
            {
                _NotifiableCollection.Notifier.Unsupply -= value;
            }
        }

        public void Bind(string  address)
        {
            _Listener.Bind(address);
        }

        public void Close()
        {
            _Listener.Close();
        }

        private void _Join(Peer peer)
        {
            peer.ErrorEvent += (status) => {
                lock (_NotifiableCollection)
                    _NotifiableCollection.Items.Remove(peer);
            };
            lock (_NotifiableCollection)
                _NotifiableCollection.Items.Add(peer);
        }
    }
}
