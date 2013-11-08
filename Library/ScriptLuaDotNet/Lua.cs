using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Script.Lua
{
    public class VirtualMachine : IVirtualMachine , Regulus.Framework.ILaunched
    {
        LuaInterface.Lua _Lua;


        void Framework.ILaunched.Launch()
        {
            _Lua = new LuaInterface.Lua();
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
