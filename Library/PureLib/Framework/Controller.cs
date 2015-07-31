// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Controller.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Controller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace Regulus.Framework
{
	internal class Controller<T> where T : class, IUpdatable
	{
		public string _Name;

		public T User { get; private set; }

		public string Name
		{
			get { return this._Name; }
		}

		public ICommandParsable<T> Parser { get; set; }

		public GPIBinderFactory Builder { get; set; }

		public Controller(string name, T user)
		{
			this.User = user;
			this._Name = name;
		}
	}
}