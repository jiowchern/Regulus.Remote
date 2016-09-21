using System.Reflection;

namespace Regulus.Remoting
{
	public class Loader
	{
		public static ICore Load(byte[] assembly_stream, string class_name)
		{
			var assembly = Assembly.Load(assembly_stream);
			var instance = assembly.CreateInstance(class_name);            
            return instance as ICore;
		}

	    

    }
}
