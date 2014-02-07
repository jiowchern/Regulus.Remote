using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNativeGameCore
{
    public class GameCore : Regulus.Game.ICore, TestNativeGameCore.IMessager
    {
        
        void Regulus.Game.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            
            binder.Bind<TestNativeGameCore.IMessager>(this);
            binder.BreakEvent += () =>
            {
                binder.Unbind<TestNativeGameCore.IMessager>(this);
            };
        }

        public void Launch()
        {
            
        }

        public void Shutdown()
        {
            
        }

        public bool Update()
        {
            return true;
        }

        Regulus.Remoting.Value<string> IMessager.Send(string message)
        {
            return "response : " + message;
        }
    }
}
