using Regulus.Remote;
using System;

namespace Regulus.Projects.TestProtocol.Common
{
    namespace MultipleNotices
    {
        public class MultipleNotices : IMultipleNotices 
        {

            public readonly NotifiableCollection<INumber> Numbers1;
            public readonly NotifiableCollection<INumber> Numbers2;
            Notifier<INumber> IMultipleNotices.Numbers1 => new Notifier<INumber>(Numbers1);

            Notifier<INumber> IMultipleNotices.Numbers2 => new Notifier<INumber>(Numbers2);

            

            public MultipleNotices()
            {
                
                Numbers1 = new NotifiableCollection<INumber>();
                Numbers2 = new NotifiableCollection<INumber>();
            }
        }
    }
}
