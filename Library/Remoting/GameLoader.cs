using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
            try
            {
                var instance = assembly.CreateInstance(class_name);
                return instance as Regulus.Game.ICore;
            }
            catch (System.MissingMethodException mme)
            {

            }
            catch (System.IO.FileNotFoundException fne)
            {

            }
            catch (System.IO.FileLoadException fe)
            {

            }
            catch (System.BadImageFormatException bfe)
            {

            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                var sss = tie.ToString();
            }
            return null;
        }
    }
}


