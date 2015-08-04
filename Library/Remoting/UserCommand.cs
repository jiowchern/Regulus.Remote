// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserCommand.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IUserCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Regulus.Remoting
{
	public interface IUserCommand
	{
		void Register();

		void Unregister();
	}
}