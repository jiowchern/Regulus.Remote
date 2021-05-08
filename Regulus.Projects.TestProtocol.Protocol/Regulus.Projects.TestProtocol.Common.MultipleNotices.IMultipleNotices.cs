   
    using System;  
    
    using System.Collections.Generic;
    
    namespace Regulus.Projects.TestProtocol.Common.MultipleNotices.Ghost 
    { 
        public class CIMultipleNotices : Regulus.Projects.TestProtocol.Common.MultipleNotices.IMultipleNotices , Regulus.Remote.IGhost
        {
            readonly bool _HaveReturn ;
            
            readonly long _GhostIdName;
            
            
            
            public CIMultipleNotices(long id, bool have_return )
            {                                
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
            
            

                    public Regulus.Remote.Notifier<Regulus.Projects.TestProtocol.Common.INumber> _Numbers1= new Regulus.Remote.Notifier<Regulus.Projects.TestProtocol.Common.INumber>();
                    Regulus.Remote.Notifier<Regulus.Projects.TestProtocol.Common.INumber> Regulus.Projects.TestProtocol.Common.MultipleNotices.IMultipleNotices.Numbers1 { get{ return _Numbers1;} }

                    public Regulus.Remote.Notifier<Regulus.Projects.TestProtocol.Common.INumber> _Numbers2= new Regulus.Remote.Notifier<Regulus.Projects.TestProtocol.Common.INumber>();
                    Regulus.Remote.Notifier<Regulus.Projects.TestProtocol.Common.INumber> Regulus.Projects.TestProtocol.Common.MultipleNotices.IMultipleNotices.Numbers2 { get{ return _Numbers2;} }

            
        }
    }
