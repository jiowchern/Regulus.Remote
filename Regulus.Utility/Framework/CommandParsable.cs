using Regulus.Remote;

namespace Regulus.Framework
{
	public interface ICommandParsable<T>
	{
		void Setup(IGPIBinderFactory build);

		void Clear();
	}
}
