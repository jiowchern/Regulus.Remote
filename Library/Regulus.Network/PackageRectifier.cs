using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Regulus.Network.RUDP
{
    public class PackageRectifier
    {
        private readonly Dictionary<ushort, SocketMessage> _DataPackages;

        private readonly Queue<SocketMessage> _Packages;
        private ushort _Serial;
        private uint _SerialBitFields;

        public ushort Serial { get { return _Serial ; } }
        public uint SerialBitFields { get { return _SerialBitFields; } }
        public int Count { get { return _DataPackages.Count; } }

        public PackageRectifier()
        {

            _Packages = new Queue<SocketMessage>();
            _DataPackages = new Dictionary<ushort, SocketMessage>();
            _Serial = 0;
        }
        public bool PushPackage(SocketMessage segment_message)
        {
            var seq = segment_message.GetSeq();
            var exist = _DataPackages.ContainsKey(seq) ;
            
            if (exist == false && _SerialMoreRecent(seq) )
            {
                _DataPackages.Add(seq, segment_message);

                if (_Serial == seq)
                {
                    _Serial = _Rectify(_Serial);
                }


                uint mask = 1;
                for (uint i = 0; i < 32; i++)
                {
                    SocketMessage pkg;
                    if (_DataPackages.TryGetValue((ushort)(_Serial + i ), out pkg))
                    {
                        _SerialBitFields = _SerialBitFields | mask;
                    }

                    mask <<= 1;
                }

                return true;
            }
            return false;
        }

        private bool _SerialMoreRecent(ushort serial)
        {
            return (serial >= _Serial) &&
                   (serial - _Serial <= 0xFFFF / 2)
                   ||
                   (_Serial >= serial) &&
                   (_Serial - serial > 0xFFFF / 2);
        }

        public SocketMessage PopPackage()
        {
            lock (_Packages)
            {
                if (_Packages.Count > 0)
                    return _Packages.Dequeue();
                return null;
            }

        }

        private ushort _Rectify(ushort serial)
        {
            var removePackages = new List<ushort>();
            var index = serial;
            SocketMessage message;
            while (_DataPackages.TryGetValue(index, out message))
            {
                lock (_Packages)
                {
                    _Packages.Enqueue(message);
                }
                
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