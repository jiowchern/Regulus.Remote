using NSubstitute;
using System;
using System.Linq;

namespace Regulus.Remote.Tests
{
    namespace NotifierProperty
    {
        public interface IGpiB
        {

        }

        public class SoulB : IGpiB
        {

        }
        public interface IGpiA
        {
            INotifier<IGpiB> GpiBs { get; }
        }
        public class SoulA : IGpiA , INotifier<IGpiB>
        {
            internal readonly SoulB SoulB;

            INotifier<IGpiB> IGpiA.GpiBs => this;

            IGpiB[] INotifier<IGpiB>.Ghosts => new[] { SoulB };

            IGpiB[] INotifier<IGpiB>.Returns => throw new NotImplementedException();

            public SoulA()
            {
                this.SoulB = new SoulB();
            }

            event Action<IGpiB> INotifier<IGpiB>.Return
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event Action<IGpiB> _Supply;
            event Action<IGpiB> INotifier<IGpiB>.Supply
            {
                add
                {
                    _Supply += value;
                }

                remove
                {
                    _Supply -= value;
                }
            }

            event Action<IGpiB> _Unsupply;
            event Action<IGpiB> INotifier<IGpiB>.Unsupply
            {
                add
                {
                    _Unsupply += value;
                }

                remove
                {
                    _Unsupply -= value;
                }
            }

            internal void Add()
            {
                _Supply(SoulB);
            }

            internal void Remove()
            {
                _Unsupply(SoulB);
            }
        }
        
        public class NotifierPropertyTest
        {
            [NUnit.Framework.Test]
            public void SoulTest()
            {
                IGpiB supplyB = null;
                IGpiB unsupplyB = null;
                var soulA = new SoulA();
                var soul = new Regulus.Remote.SoulProvider.Soul(1, 1, typeof(IGpiA), soulA);
                var gpiBsProperty = typeof(IGpiA).GetProperties().FirstOrDefault(p => p.Name == nameof(IGpiA.GpiBs));                
                var binder = NotifierBinder.Create(soulA, gpiBsProperty);
                binder.SupplyEvent += (instance) => supplyB = instance as IGpiB;
                binder.UnsupplyEvent += (instance) => unsupplyB = instance as IGpiB;
                soul.AttachNotifier(binder);

                soulA.Add();
                soulA.Remove();

                soul.DetachNotifier(binder);

                NUnit.Framework.Assert.AreEqual(soulA.SoulB , supplyB);
                NUnit.Framework.Assert.AreEqual(soulA.SoulB, unsupplyB);
            }
        }
    }
}