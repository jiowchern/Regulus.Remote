using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Regulus.Network.RUDP
{
    public class PackageRectifier
    {
        private readonly Dictionary<ushort, SocketMessage> _DataPackages;

        private readonly Queue<SocketMessage> _Packages;
        private ushort _Serial;
        private uint _SerialBitFields;

        public ushort Serial { get { return _Serial; } }
        public uint SerialBitFields { get { return _SerialBitFields; } }

        public PackageRectifier()
        {

            _Packages = new Queue<SocketMessage>();
            _DataPackages = new Dictionary<ushort, SocketMessage>();
            _Serial = 0;
        }
        public bool PushPackage(SocketMessage segment_message)
        {
            var exist = _DataPackages.ContainsKey(segment_message.GetSeq()) ;
            
            if (exist == false && _SerialMoreRecent(segment_message.GetSeq()) )
            {
                _DataPackages.Add(segment_message.GetSeq(), segment_message);

                if (_Serial == segment_message.GetSeq())
                {
                    _Serial = _Rectify(_Serial);
                }


                uint mask = 1;
                for (uint i = 0; i < 32; i++)
                {
                    SocketMessage pkg;
                    if (_DataPackages.TryGetValue((ushort)(_Serial + i + 1), out pkg))
                    {
                        _SerialBitFields = _SerialBitFields | mask;
                    }

                    mask <<= 1;
                }

                return true;
            }
            return false;
        }

        private bool _SerialMoreRecent(uint serial)
        {
            return (serial >= _Serial) &&
                   (serial - _Serial <= 0xFFFFFFFF / 2)
                   ||
                   (_Serial >= serial) &&
                   (_Serial - serial > 0xFFFFFFFF / 2);
        }

        public SocketMessage PopPackage()
        {
            if(_Packages.Count > 0)
                return _Packages.Dequeue();
            return null;
        }

        private ushort _Rectify(ushort serial)
        {
            var removePackages = new List<ushort>();
            var index = serial;
            SocketMessage message;
            while (_DataPackages.TryGetValue(index, out message))
            {
                _Packages.Enqueue(message);                
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