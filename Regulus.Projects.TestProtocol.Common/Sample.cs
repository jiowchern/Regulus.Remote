using Regulus.Remote;
using System;
using System.Linq;

namespace Regulus.Projects.TestProtocol.Common
{
    public class Sample : ISample
    {

        public readonly NotifierCollection<INumber> Numbers;
        public readonly NotifierCollection<int> Ints;

        Property<int> _LastValue;
        public Sample()
        {            
            Numbers = new NotifierCollection<INumber>();
            Ints = new NotifierCollection<int>();
            _LastValue = new Property<int>();
        }

        Property<int> ISample.LastValue => _LastValue;

        

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
            _LastValue.Value = num1 + num2;
            return _LastValue.Value;
        }

        Value<bool> ISample.RemoveNumber(int val)
        {
            var number = Numbers.Items.First(n => n.Value == val);
            return Numbers.Items.Remove(number);
        }
    }
}
