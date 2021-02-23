   
    using System;  
    
    using System.Collections.Generic;
    
    namespace Regulus.Projects.TestProtocol.Common.Ghost 
    { 
        public class CINext : Regulus.Projects.TestProtocol.Common.INext , Regulus.Remote.IGhost
        {
            readonly bool _HaveReturn ;
            
            readonly long _GhostIdName;
            
            
            
            public CINext(long id, bool have_return )
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
            
            
                Regulus.Remote.Value<System.Boolean> Regulus.Projects.TestProtocol.Common.INext.Next()
                {                    

                    
    var returnValue = new Regulus.Remote.Value<System.Boolean>();
    

                    var info = typeof(Regulus.Projects.TestProtocol.Common.INext).GetMethod("Next");
                    _CallMethodEvent(info , new object[] {} , returnValue);                    
                    return returnValue;
                }

                



            
        }
    }
