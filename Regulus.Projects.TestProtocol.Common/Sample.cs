using Regulus.Remote;
using System;
using System.Linq;

namespace Regulus.Projects.TestProtocol.Common
{
    public class Sample : ISample
    {

        public readonly NotifierCollection<INumber> Numbers;
        public readonly NotifierCollection<int> Ints;
        public Sample()
        {            
            Numbers = new NotifierCollection<INumber>();
            Ints = new NotifierCollection<int>();
        }
        

        INotifier<INumber> ISample.Numbers => Numbers;

        
        event Action<int> ISample.IntsEvent
        {
            add
            {
                Ints.Notifier.Supply += value;
            }

            remove
            {
                Ints.Notifier.Supply -= value;
            }
        }

        Value<int> ISample.Add(int num1, int num2)
        {
            return num1 + num2;
        }

        Value<bool> ISample.RemoveNumber(int val)
        {
            var number = Numbers.Items.First(n => n.Value == val);
            return Numbers.Items.Remove(number);
        }
    }
}
