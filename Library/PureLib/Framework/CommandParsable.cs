// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandParsable.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ICommandParsable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;

#endregion

namespace Regulus.Framework
{
	public interface ICommandParsable<T>
	{
		void Setup(IGPIBinderFactory build);

		void Clear();
	}
}