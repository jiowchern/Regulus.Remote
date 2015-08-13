using Regulus.Remoting;

namespace Regulus.Framework
{
	public interface ICommandParsable<T>
	{
		void Setup(IGPIBinderFactory build);

		void Clear();
	}
}
