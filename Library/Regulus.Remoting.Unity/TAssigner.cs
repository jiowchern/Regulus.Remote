using System.Collections.Generic;
using System.Linq;


namespace Regulus.Remote.Unity
{
    public class Assigner<T> : Regulus.Remote.Unity.Assigner
    {
        private readonly INotifier<T> _Notifier;


        private readonly List<T> _UnusedGpis;


        private readonly List<Adsorber<T>> _UnusedAdsorbers;


        public class Binded
        {
            public readonly Adsorber<T> Adsorber;

            public readonly T Gpi;


            public Binded(Adsorber<T> adsorber, T gpi)
            {
                Adsorber = adsorber;
                Gpi = gpi;
            }
        }

        private readonly List<Binded> _Bindeds;

        public Assigner(INotifier<T> notifier)
        {
            _Bindeds = new List<Binded>();
            _UnusedAdsorbers = new List<Adsorber<T>>();
            _UnusedGpis = new List<T>();

            _Notifier = notifier;
            _Notifier.Supply += _Add;
            _Notifier.Unsupply += _Remove;

        }

        private void _Remove(T obj)
        {

            _UnusedGpis.RemoveAll(g => g.GetHashCode() == obj.GetHashCode());
            _Unbind(obj);
        }



        private void _Add(T obj)
        {
            _Attach(obj);
        }

        private void _Attach(T obj)
        {
            if (_Bind(obj) == false)
            {
                _UnusedGpis.Add(obj);
            }
        }

        public override void Register<T1>(Adsorber<T1> adsorber)
        {
            var asadsorber = adsorber as Adsorber<T>;

            _Attach(asadsorber);
        }

        private void _Attach(Adsorber<T> asadsorber)
        {
            if (_Bind(asadsorber) == false)
            {
                _UnusedAdsorbers.Add(asadsorber);
            }
        }

        private bool _Bind(Adsorber<T> adsorber)
        {
            var gpi = _UnusedGpis.FirstOrDefault();
            if (gpi != null)
            {
                _Bind(adsorber, gpi);
                _UnusedGpis.Remove(gpi);
                return true;
            }
            return false;
        }

        private void _Bind(Adsorber<T> adsorber, T gpi)
        {
            adsorber.Supply(gpi);
            _Bindeds.Add(new Binded(adsorber, gpi));
        }

        private void _Unbind(T gpi)
        {
            var binded = _Bindeds.FirstOrDefault(b => b.Gpi.GetHashCode() == gpi.GetHashCode());

            if (binded == null)
                return;


            _Bindeds.Remove(binded);
            binded.Adsorber.Unsupply(gpi);            

            _Attach(binded.Adsorber);
        }

        private bool _Bind(T obj)
        {
            var adsorber = _UnusedAdsorbers.FirstOrDefault();
            if (adsorber != null)
            {
                _Bind(adsorber, obj);
                _UnusedAdsorbers.Remove(adsorber);
                return true;
            }
            return false;
        }

        public override void Unregister<T1>(Adsorber<T1> adsorber)
        {
            _UnusedAdsorbers.RemoveAll(a => a.GetHashCode() == adsorber.GetHashCode());
            _Unbind(adsorber as Adsorber<T>);
        }

        private void _Unbind(Adsorber<T> adsorber)
        {
            var binded = _Bindeds.FirstOrDefault(b => b.Adsorber.GetHashCode() == adsorber.GetHashCode());

            if(binded == null)
                return;

            binded.Adsorber.Unsupply(binded.Gpi);
            _Bindeds.Remove(binded);            
            _Attach(binded.Gpi);
        }
    }
}
