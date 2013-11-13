using System;
using System.Collections.Generic;

using System.Text;




namespace Regulus.Script.Lua
{
    
    public interface IVirtualMachine 
    {
        void Execute(string chank);

        object[] Call(string function_fullname, params object[] args);
    }

    public class VirtualMachineProvider
    {
        System.Reflection.Assembly _Assembly;

        public VirtualMachineProvider()
        {
            
            
        }

        ~VirtualMachineProvider()
        { 

        }

        public void Initialize(byte[] dllstream)
        {
            var assembly = System.Reflection.Assembly.Load(dllstream);
            _Assembly = assembly;
        }

        public void Finialize()
        { 
            
        }

        public IVirtualMachine Create()
        {
            var instance = _Assembly.CreateInstance("Regulus.Script.Lua.VirtualMachine");
            ((Regulus.Framework.ILaunched)instance).Launch();
            return (IVirtualMachine)instance;
        }

        public void Destroy(IVirtualMachine vm)
        {
            ((Regulus.Framework.ILaunched)vm).Shutdown();
        }
        
    }
}
