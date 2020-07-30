using NSubstitute;
using System;
using System.Linq;
using System.Reflection;

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
        
        public class GhostA : IGpiA , IGhost
        {



            readonly GhostNotifier<IGpiB> _GpiBs ;
            INotifier<IGpiB> IGpiA.GpiBs => _GpiBs;

            public GhostA()
            {
                var IGptA_GpiBs_Property = typeof(IGpiA).GetProperty(nameof(IGpiA.GpiBs));
                _GpiBs = new GhostNotifier<IGpiB>((p) => _AddSupplyNoitfierEvent(IGptA_GpiBs_Property, p), (p) => _AddSupplyNoitfierEvent(IGptA_GpiBs_Property,p), (p) => _AddSupplyNoitfierEvent(IGptA_GpiBs_Property, p), (p) => _AddSupplyNoitfierEvent(IGptA_GpiBs_Property,p));
            }

            event CallMethodCallback IGhost.CallMethodEvent
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

            event EventNotifyCallback IGhost.AddEventEvent
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

            event EventNotifyCallback IGhost.RemoveEventEvent
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

            event PropertyNotifierCallback _AddSupplyNoitfierEvent;
            event PropertyNotifierCallback IGhost.AddSupplyNoitfierEvent
            {
                add
                {
                    _AddSupplyNoitfierEvent += value;
                }

                remove
                {
                    _AddSupplyNoitfierEvent -= value;
                }
            }

            event PropertyNotifierCallback _RemoveSupplyNoitfierEvent;
            event PropertyNotifierCallback IGhost.RemoveSupplyNoitfierEvent
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

            event PropertyNotifierCallback IGhost.AddUnsupplyNoitfierEvent
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

            event PropertyNotifierCallback IGhost.RemoveUnsupplyNoitfierEvent
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

            long IGhost.GetID()
            {
                throw new NotImplementedException();
            }

            object IGhost.GetInstance()
            {
                throw new NotImplementedException();
            }

            bool IGhost.IsReturnType()
            {
                throw new NotImplementedException();
            }
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
                var supplyBinder = NotifierEventBinder.Create(soulA, gpiBsProperty ,"Supply" , (instance) => supplyB = instance as IGpiB);
                

                var unsupplyBinder = NotifierEventBinder.Create(soulA, gpiBsProperty, "Unsupply", (instance) => unsupplyB = instance as IGpiB);
                

                soul.AttachSupply(1,supplyBinder);
                soul.AttachUnsupply(1, unsupplyBinder);

                soulA.Add();
                soulA.Remove();

                soul.DetachSupply(1);
                soul.DetachUnsupply(1);

                NUnit.Framework.Assert.AreEqual(soulA.SoulB , supplyB);
                NUnit.Framework.Assert.AreEqual(soulA.SoulB, unsupplyB);
            }

            [NUnit.Framework.Test]
            public void GhostNotifierTest()
            {
                PassageCallback p1 = null;
                PassageCallback p2 = null;
                PassageCallback p3 = null;
                PassageCallback p4 = null;
                Action<PassageCallback> addSupply = p=>  p1 = p ;
                Action<PassageCallback> removeSupply = p => p2 = p;
                Action<PassageCallback> addUnsupply = p => p3 = p;
                Action<PassageCallback> removeUnsupply = p => p4 = p;
                var notifier = new Regulus.Remote.GhostNotifier<IGpiA>(addSupply, removeSupply, addUnsupply, removeUnsupply);
                var gpiNotifier = notifier as INotifier<IGpiA>;
                Action<IGpiA> empty = (gpi) =>{ };
                gpiNotifier.Supply += empty;
                gpiNotifier.Supply -= empty;
                gpiNotifier.Unsupply += empty;
                gpiNotifier.Unsupply -= empty;

                var c1 = p1.GetHashCode();
                var c2 = p2.GetHashCode();

                NUnit.Framework.Assert.AreNotEqual(null, p1);
                NUnit.Framework.Assert.AreNotEqual(null, p2);
                NUnit.Framework.Assert.AreNotEqual(null, p3);
                NUnit.Framework.Assert.AreNotEqual(null, p4);

                NUnit.Framework.Assert.AreNotSame(c1,c2);
                NUnit.Framework.Assert.AreNotSame(p3, p4);
            }
            
            [NUnit.Framework.Test]
            public void GhostTest()
            {
                var ghostA = new GhostA();
                IGpiA gpiA = ghostA;
                IGhost ghost = ghostA;
                PropertyInfo addSupplyProperty = null;
                PassageCallback addSupplyPassage = null;
                ghost.AddSupplyNoitfierEvent += (property, passage) =>
                {
                    addSupplyProperty = property;
                    addSupplyPassage = passage;
                };
                IGpiB gPi = null; 
                gpiA.GpiBs.Supply += (gpi) => {
                    gPi = gpi;
                };

                var soulB = new SoulB();
                addSupplyPassage(soulB);


                NUnit.Framework.Assert.AreEqual(soulB , gPi);
            }
        }
    }
}
