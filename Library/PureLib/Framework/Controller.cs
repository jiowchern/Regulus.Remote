using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Framework
{
	internal class Controller<T> where T : class, IUpdatable
	{
		public string _Name;

		public T User { get; private set; }

		public string Name
		{
			get { return _Name; }
		}

		public ICommandParsable<T> Parser { get; set; }

		public GPIBinderFactory Builder { get; set; }

		public Controller(string name, T user)
		{
			User = user;
			_Name = name;
		}
	}
}
