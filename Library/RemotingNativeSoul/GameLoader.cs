using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Game
{
    public interface ICore : Regulus.Utility.IUpdatable
    {
        void ObtainController(Regulus.Remoting.ISoulBinder binder);
    }

    public class Loader
    {
        static public Regulus.Game.ICore Load(byte[] assembly_stream, string class_name)
        {
            var assembly = System.Reflection.Assembly.Load(assembly_stream);
            var instance = assembly.CreateInstance(class_name);
            return instance as Regulus.Game.ICore;
        }
    }
}


