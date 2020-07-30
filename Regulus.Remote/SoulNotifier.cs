using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Regulus.Remote
{
    internal class SoulNotifier
    {
        readonly IdLandlord _SupplyIdLandlord;
        readonly IdLandlord _UnsupplyIdLandlord;
        readonly List<SoulPassage> _SupplyPassages;
        readonly List<SoulPassage> _UnsupplyPassages;
        public SoulNotifier()
        {
            _SupplyPassages = new List<SoulPassage>();
            _SupplyIdLandlord = new IdLandlord();

            _UnsupplyPassages = new List<SoulPassage>();
            _UnsupplyIdLandlord = new IdLandlord();
        }
        

        private long _UnregisterSupply(PassageCallback handler)
        {
            var passage = _SupplyPassages.FirstOrDefault(p => p.Handler == handler);
            if (passage == null)
                return 0;
            _SupplyPassages.Remove(passage);
            return passage.Id;
        }
        
        private long _RegisterSupply(PassageCallback passage)
        {
            var id = _SupplyIdLandlord.Rent();
            _SupplyPassages.Add(new SoulPassage(id, passage));
            return id;
        }

        private long _UnregisterUnsupply(PassageCallback handler)
        {
            var passage = _UnsupplyPassages.FirstOrDefault(p => p.Handler == handler);
            if (passage == null)
                return 0;
            _UnsupplyPassages.Remove(passage);
            return passage.Id;
        }

        private long _RegisterUnsupply(PassageCallback passage)
        {
            var id = _UnsupplyIdLandlord.Rent();
            _UnsupplyPassages.Add(new SoulPassage(id , passage));
            return id;
        }

        public long UnregisterSupply(PassageCallback passage)
        {
            return _UnregisterSupply(passage);
        }

        public long RegisterSupply(PassageCallback passage)
        {
            return _RegisterSupply(passage);
        }

        public long UnregisterUnsupply(PassageCallback passage)
        {
            return _UnregisterUnsupply(passage);
        }

        public long RegisterUnsupply(PassageCallback passage)
        {
            return _RegisterUnsupply(passage);
        }

        internal void Supply(IGhost ghost, long notifier_id)
        {
            var passage = _FindSupply(notifier_id);
            if(passage != null)
                passage(ghost);
        }

        private PassageCallback _FindSupply(long notifier_id)
        {
            var passage = _SupplyPassages.FirstOrDefault(p=>p.Id == notifier_id);
            return passage?.Handler;
        }

        private PassageCallback _FindUnsupply(long notifier_id)
        {
            var passage = _UnsupplyPassages.FirstOrDefault(p => p.Id == notifier_id);
            return passage?.Handler;
        }
        

        internal void Unsupply(IGhost ghost, long notifier_id)
        {
            var passage = _FindUnsupply(notifier_id);
            if (passage != null)
                passage(ghost);
        }

        
    }
}