// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameLoader.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Loader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Reflection;

#endregion

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