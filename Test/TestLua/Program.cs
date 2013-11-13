using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLua
{
    public class Test 
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    class Program
    {
        

        static void Main(string[] args)
        {

            Regulus.Script.Lua.VirtualMachineProvider vmp = new Regulus.Script.Lua.VirtualMachineProvider();

            vmp.Initialize(_OpenPlugin("ScriptLuaDotNet.dll"));
            var vm = vmp.Create();

            // 測試呼叫lua code
            vm.Execute("function Add(a,b) return a-b end");

            object[] results = vm.Call("Add" , 1 , 2);
            
            vmp.Finialize();
            
        }

        private static byte[] _OpenPlugin(string path)
        {
            using (var file = System.IO.File.Open(path, System.IO.FileMode.Open))
            { 
                
                byte[] buffer = new byte[file.Length];
                if (file.Length < int.MaxValue)
                {
                    file.Read(buffer, 0, (int)file.Length);
                }
                else
                    throw new SystemException("read file source too big");

                return buffer;
            }
            return null;
        }

        
    }
}
