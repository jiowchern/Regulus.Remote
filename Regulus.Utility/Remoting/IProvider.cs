using System;

namespace Regulus.Remote
{
    public interface IProvider
	{
		IGhost[] Ghosts { get; }

		void Add(IGhost entiry);

		void Remove(Guid id);

		IGhost Ready(Guid id);

		void ClearGhosts();
	}
}
