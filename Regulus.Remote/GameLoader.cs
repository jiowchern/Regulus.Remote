using System.Linq;
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

		public static IEntry Create(Assembly assembly)
		{

			var types = assembly.GetExportedTypes();
			var protocolType = types.Where(type => type.GetInterface(nameof(Regulus.Remote.IEntry)) != null).FirstOrDefault();
			if (protocolType == null)
				throw new System.Exception($"找不到{nameof(Regulus.Remote.IEntry)}的實作");
			return System.Activator.CreateInstance(protocolType) as Regulus.Remote.IEntry;
		}

		public static System.Collections.Generic.IEnumerable<System.Type> GetEntrys()
		{
			return System.AppDomain.CurrentDomain.GetAssemblies().Where(a => a.IsDynamic == false).SelectMany(assembly => assembly.GetExportedTypes().Where(type => typeof(Regulus.Remote.IProtocol).IsAssignableFrom(type) && typeof(Regulus.Remote.IEntry) != type).Select(t => t));
		}



	}
}
