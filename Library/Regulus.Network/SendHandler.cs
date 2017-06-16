using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Serialization;
using System.Linq;
namespace Regulus.Network.RUDP
{
    public class SendHandler
    {
        
        private readonly Serializer _Serializer;

        private readonly BufferDispenser _Dispenser;
        private readonly List<byte[]> _Packages;

        public SendHandler(int package_size , Serializer serializer)
        {
            _Dispenser = new BufferDispenser(package_size);
            _Serializer = serializer;
            _Packages = new List<byte[]>();
        }

        public void PushData(byte[] buffer)
        {
            var packages = _Dispenser.Packing(buffer, 0, 0);
            var buffers = from package in packages select _Serializer.ObjectToBuffer(package);
            _Packages.AddRange(buffers);
        }

        public void PushAck(uint package_serial_number)
        {
            var package = new AckPackage();
            package.SerialNumber = package_serial_number;
            _Packages.Add(_Serializer.ObjectToBuffer(package));
        }

        public void PopPackages(List<byte[]> packages)
        {
            packages.AddRange(_Packages);
            _Packages.Clear();
        }


        
    }
}