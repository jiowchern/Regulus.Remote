using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Utility
{
    

    public class Loader
    {
        static public Regulus.Remoting.ICore Load(byte[] assembly_stream, string class_name)
        {
            var assembly = System.Reflection.Assembly.Load(assembly_stream);
            var instance = assembly.CreateInstance(class_name);
            return instance as Regulus.Remoting.ICore;
        }
    }
}


