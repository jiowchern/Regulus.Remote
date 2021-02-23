using Regulus.Remote;
using System;
using System.Linq;

namespace Regulus.Projects.TestProtocol.Common
{
    public class Sample : ISample , System.IDisposable
    {

        public readonly NotifiableCollection<INumber> Numbers;
        public readonly NotifiableCollection<int> Ints;

        readonly Notifier<INumber> _NumberNotifier;

        readonly Property<int> _LastValue;
        public Sample()
        {            
            Numbers = new NotifiableCollection<INumber>();
            _NumberNotifier = new Notifier<INumber>(Numbers);
            Ints = new NotifiableCollection<int>();
            _LastValue = new Property<int>();
        }

        Property<int> ISample.LastValue => _LastValue;

        Notifier<INumber> ISample.Numbers => _NumberNotifier;

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

        public void Dispose()
        {
            Numbers.Items.Clear();
            _NumberNotifier.Dispose();
        }

        Value<int> ISample.Add(int num1, int num2)
        {
            _LastValue.Value = num1 + num2;
            return _LastValue.Value;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        Value<bool> ISample.RemoveNumber(int val)
        {
            var number = Numbers.Items.First(n => n.Value == val);
            return Numbers.Items.Remove(number);
        }
    }
}
