
    using System;  
    using System.Collections.Generic;
    
    namespace Regulus.Projects.TestProtocol.Common.Invoker.ISample 
    { 
        public class IntsEvent : Regulus.Remote.IEventProxyCreator
        {

            Type _Type;
            string _Name;
            
            public IntsEvent()
            {
                _Name = "IntsEvent";
                _Type = typeof(Regulus.Projects.TestProtocol.Common.ISample);                   
            
            }
            Delegate Regulus.Remote.IEventProxyCreator.Create(long soul_id,int event_id,long handler_id, Regulus.Remote.InvokeEventCallabck invoke_Event)
            {                
                var closure = new Regulus.Remote.GenericEventClosure<System.Int32>(soul_id , event_id ,handler_id, invoke_Event);                
                return new Action<System.Int32>(closure.Run);
            }
        

            Type Regulus.Remote.IEventProxyCreator.GetType()
            {
                return _Type;
            }            

            string Regulus.Remote.IEventProxyCreator.GetName()
            {
                return _Name;
            }            
        }
    }
                