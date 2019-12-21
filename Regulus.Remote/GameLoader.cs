using System.Reflection;

namespace Regulus.Remote
{
	public class Loader
	{
		public static IEntry Load(byte[] assembly_stream, string class_name)
		{
			var assembly = Assembly.Load(assembly_stream);
			var instance = assembly.CreateInstance(class_name);            
            return instance as IEntry;
		}

	    

    }
}
