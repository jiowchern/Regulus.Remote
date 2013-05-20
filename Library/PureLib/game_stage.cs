using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Game
{
	public interface IStage
	{
		void	Enter	();
		void	Leave	();
		void	Update	();
	}

	public interface IStage<T> 
	{
		void Enter(T obj);
		void Leave(T obj);
		void Update(T obj);
	}
}
