using System;

namespace Regulus.Remote.Server
{
    public class DriverService : Soul.IService
    {
        readonly Soul.IService _Service;
        
        readonly System.IDisposable _Driver;
        public DriverService(Soul.Service service)
        {
            _Service = service;
            _Driver = new Soul.Driver(service);
        }
        void IDisposable.Dispose()
        {
            _Driver.Dispose();
            _Service.Dispose();
        }
    }
}
