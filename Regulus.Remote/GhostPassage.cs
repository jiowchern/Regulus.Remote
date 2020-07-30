using System;

namespace Regulus.Remote
{
    public class GhostPassage<T>
    {
        public readonly Action<T> Owner;
        public event System.Action<T> ThroughEvent;
        public GhostPassage(Action<T> owner)
        {
            this.Owner = owner;
        }

        public void Through(object gpi)
        {
            var tgpi = (T)gpi;
            ThroughEvent(tgpi);
            Owner(tgpi);
        }
    }
}