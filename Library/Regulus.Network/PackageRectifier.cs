using System;
using System.Collections.Generic;
using System.IO;

namespace Regulus.Network.RUDP
{
    public class PackageRectifier
    {
        private readonly Dictionary<uint, MessagePackage> _DataPackages;
        private readonly List<byte> _Stream;
        private uint _Serial;
        public PackageRectifier()
        {
            _Stream = new List<byte>();
            _DataPackages = new Dictionary<uint, MessagePackage>();
            _Serial = 0;
        }
        public void PushPackage(MessagePackage message_package)
        {
            _DataPackages.Add(message_package.Serial , message_package);

            if (_Serial == message_package.Serial)
            {
                _Serial = _Rectify(_Serial);
            }
        }

        public byte[] PopStream()
        {
            var stream = _Stream.ToArray();
            _Stream.Clear();            
            return stream;
        }

        private uint _Rectify(uint serial)
        {
            var removePackages = new List<uint>();
            var index = serial;
            MessagePackage package;
            while (_DataPackages.TryGetValue(index, out package))
            {
                _Stream.AddRange(package.Data);
                removePackages.Add(index);
                index++;
            }
            foreach (var removePackage in removePackages)
            {
                _DataPackages.Remove(removePackage);
            }

            return index;
        }
    }
}