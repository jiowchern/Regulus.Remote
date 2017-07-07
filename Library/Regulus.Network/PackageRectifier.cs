using System;
using System.Collections.Generic;
using System.IO;

namespace Regulus.Network.RUDP
{
    public class PackageRectifier
    {
        private readonly Dictionary<uint, SegmentPackage> _DataPackages;
        private readonly List<byte[]> _Packages;
        private uint _Serial;
        private uint _SerialBitFields;

        public uint Serial { get { return _Serial; } }
        public uint SerialBitFields { get { return _SerialBitFields; } }

        public PackageRectifier()
        {
            _Packages = new List<byte[]>();
            _DataPackages = new Dictionary<uint, SegmentPackage>();
            _Serial = 0;
        }
        public bool PushPackage(SegmentPackage segment_package)
        {
            var exist = _DataPackages.ContainsKey(segment_package.Serial) ;
            
            if (exist == false && _SerialMoreRecent(segment_package.Serial) )
            {
                _DataPackages.Add(segment_package.Serial, segment_package);

                if (_Serial == segment_package.Serial)
                {
                    _Serial = _Rectify(_Serial);
                }


                uint mask = 1;
                for (uint i = 0; i < 32; i++)
                {
                    SegmentPackage pkg;
                    if (_DataPackages.TryGetValue(_Serial + i + 1, out pkg))
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

        public void PopPackages(Queue<byte[]> packages)
        {
            for (int i = 0; i < _Packages.Count; i++)
            {
                packages.Enqueue(_Packages[i]);
            }
            _Packages.Clear();
        }

        private uint _Rectify(uint serial)
        {
            var removePackages = new List<uint>();
            var index = serial;
            SegmentPackage package;
            while (_DataPackages.TryGetValue(index, out package))
            {
                _Packages.Add(package.Data);
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