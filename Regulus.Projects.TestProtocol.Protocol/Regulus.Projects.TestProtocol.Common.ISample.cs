   
    using System;  
    
    using System.Collections.Generic;
    
    namespace Regulus.Projects.TestProtocol.Common.Ghost 
    { 
        public class CISample : Regulus.Projects.TestProtocol.Common.ISample , Regulus.Remote.IGhost
        {
            readonly bool _HaveReturn ;
            
            readonly long _GhostIdName;
            
            
            
            public CISample(long id, bool have_return )
            {                
                //Numbers
_Numbers = new Regulus.Remote.GhostNotifier<Regulus.Projects.TestProtocol.Common.INumber>((p) => _AddSupplyNoitfierEvent(typeof(ISample).GetProperty("Numbers"), p), (p) => _RemoveSupplyNoitfierEvent(typeof(ISample).GetProperty("Numbers"),p), (p) => _AddUnsupplyNoitfierEvent(typeof(ISample).GetProperty("Numbers"), p), (p) => _RemoveUnsupplyNoitfierEvent(typeof(ISample).GetProperty("Numbers"),p));
                _HaveReturn = have_return ;
                _GhostIdName = id; 
                
            }
            

            long Regulus.Remote.IGhost.GetID()
            {
                return _GhostIdName;
            }

            bool Regulus.Remote.IGhost.IsReturnType()
            {
                return _HaveReturn;
            }
            object Regulus.Remote.IGhost.GetInstance()
            {
                return this;
            }

            private event Regulus.Remote.CallMethodCallback _CallMethodEvent;

            event Regulus.Remote.CallMethodCallback Regulus.Remote.IGhost.CallMethodEvent
            {
                add { this._CallMethodEvent += value; }
                remove { this._CallMethodEvent -= value; }
            }

            private event Regulus.Remote.EventNotifyCallback _AddEventEvent;

            event Regulus.Remote.EventNotifyCallback Regulus.Remote.IGhost.AddEventEvent
            {
                add { this._AddEventEvent += value; }
                remove { this._AddEventEvent -= value; }
            }

            private event Regulus.Remote.EventNotifyCallback _RemoveEventEvent;

            event Regulus.Remote.EventNotifyCallback Regulus.Remote.IGhost.RemoveEventEvent
            {
                add { this._RemoveEventEvent += value; }
                remove { this._RemoveEventEvent -= value; }
            }
            event Regulus.Remote.PropertyNotifierCallback _AddSupplyNoitfierEvent;
            event Regulus.Remote.PropertyNotifierCallback Regulus.Remote.IGhost.AddSupplyNoitfierEvent
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

            event Regulus.Remote.PropertyNotifierCallback _RemoveSupplyNoitfierEvent;
            event Regulus.Remote.PropertyNotifierCallback Regulus.Remote.IGhost.RemoveSupplyNoitfierEvent
            {
                add
                {
                    _RemoveSupplyNoitfierEvent += value;
                }

                remove
                {
                    _RemoveSupplyNoitfierEvent -= value;
                }
            }

            event Regulus.Remote.PropertyNotifierCallback _AddUnsupplyNoitfierEvent;
            event Regulus.Remote.PropertyNotifierCallback Regulus.Remote.IGhost.AddUnsupplyNoitfierEvent
            {
                add
                {
                    _AddUnsupplyNoitfierEvent += value;
                }

                remove
                {
                    _AddUnsupplyNoitfierEvent -= value;
                }
            }

            event Regulus.Remote.PropertyNotifierCallback _RemoveUnsupplyNoitfierEvent;
            event Regulus.Remote.PropertyNotifierCallback Regulus.Remote.IGhost.RemoveUnsupplyNoitfierEvent
            {
                add
                {
                    _RemoveUnsupplyNoitfierEvent += value;
                }

                remove
                {
                    _RemoveUnsupplyNoitfierEvent -= value;
                }
            }
            
                Regulus.Remote.Value<System.Int32> Regulus.Projects.TestProtocol.Common.ISample.Add(System.Int32 _1,System.Int32 _2)
                {                    

                    
    var returnValue = new Regulus.Remote.Value<System.Int32>();
    

                    var info = typeof(Regulus.Projects.TestProtocol.Common.ISample).GetMethod("Add");
                    _CallMethodEvent(info , new object[] {_1,_2} , returnValue);                    
                    return returnValue;
                }

                


            readonly Regulus.Remote.GhostNotifier<Regulus.Projects.TestProtocol.Common.INumber> _Numbers;
            Regulus.Remote.INotifier<Regulus.Projects.TestProtocol.Common.INumber> Regulus.Projects.TestProtocol.Common.ISample.Numbers { get{ return _Numbers;} }

                public Regulus.Remote.GhostEventHandler _IntsEvent = new Regulus.Remote.GhostEventHandler();
                event System.Action<System.Int32> Regulus.Projects.TestProtocol.Common.ISample.IntsEvent
                {
                    add { 
                            var id = _IntsEvent.Add(value);
                            _AddEventEvent(typeof(Regulus.Projects.TestProtocol.Common.ISample).GetEvent("IntsEvent"),id);
                        }
                    remove { 
                                var id = _IntsEvent.Remove(value);
                                _RemoveEventEvent(typeof(Regulus.Projects.TestProtocol.Common.ISample).GetEvent("IntsEvent"),id);
                            }
                }
                
            
        }
    }
