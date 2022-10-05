using System;

namespace Regulus.Remote.Soul
{
    public class Driver : System.IDisposable
    {
        readonly System.Collections.Generic.HashSet<Advanceable> _Advanceables;
        readonly ThreadUpdater _ThreadUpdater;
        readonly AdvanceProviable _Service;
        public Driver(AdvanceProviable service)
        {
            _Service = service;
            _Service.JoinEvent += Join;
            _Service.LeaveEvent += Join;
            
            _Advanceables = new System.Collections.Generic.HashSet<Advanceable>();
            _ThreadUpdater = new ThreadUpdater(_Drive);
            _ThreadUpdater.Start();
        }

        private void _Drive()
        {
            lock(_Advanceables)
                System.Threading.Tasks.Parallel.ForEach(_Advanceables, d =>  d.Advance() );
        }

        public void Join(Advanceable driveable)
        {
            lock(_Advanceables)
                _Advanceables.Add(driveable);
        }
        public void Leave(Advanceable driveable)
        {
            lock (_Advanceables)
                _Advanceables.Remove(driveable);
        }

        void IDisposable.Dispose()
        {
            _Service.JoinEvent -= Join;
            _Service.LeaveEvent -= Leave;
            _ThreadUpdater.Stop();
        }
    }
}
