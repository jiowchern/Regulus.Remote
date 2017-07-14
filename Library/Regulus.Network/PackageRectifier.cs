using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Regulus.Network.RUDP
{
    public class PackageRectifier
    {
        private readonly Dictionary<ushort, SegmentPackage> _DataPackages;

        private readonly Queue<SegmentPackage> _Packages;
        private ushort _Serial;
        private uint _SerialBitFields;

        public ushort Serial { get { return _Serial; } }
        public uint SerialBitFields { get { return _SerialBitFields; } }

        public PackageRectifier()
        {

            _Packages = new Queue<SegmentPackage>();
            _DataPackages = new Dictionary<ushort, SegmentPackage>();
            _Serial = 0;
        }
        public bool PushPackage(SegmentPackage segment_package)
        {
            var exist = _DataPackages.ContainsKey(segment_package.GetSeq()) ;
            
            if (exist == false && _SerialMoreRecent(segment_package.GetSeq()) )
            {
                _DataPackages.Add(segment_package.GetSeq(), segment_package);

                if (_Serial == segment_package.GetSeq())
                {
                    _Serial = _Rectify(_Serial);
                }


                uint mask = 1;
                for (uint i = 0; i < 32; i++)
                {
                    SegmentPackage pkg;
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

        public SegmentPackage PopPackage()
        {
            if(_Packages.Count > 0)
                return _Packages.Dequeue();
            return null;
        }

        private ushort _Rectify(ushort serial)
        {
            var removePackages = new List<ushort>();
            var index = serial;
            SegmentPackage package;
            while (_DataPackages.TryGetValue(index, out package))
            {
                _Packages.Enqueue(package);                
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