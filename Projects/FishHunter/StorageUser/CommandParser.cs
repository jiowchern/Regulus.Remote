using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Storage
{
	internal class CommandParser : ICommandParsable<IUser>
	{
		private Command command;

		private IUser user;

		private Console.IViewer view;

		public CommandParser(Command command, Console.IViewer view, IUser user)
		{
			// TODO: Complete member initialization
			this.command = command;
			this.view = view;
			this.user = user;
		}

		void ICommandParsable<IUser>.Setup(IGPIBinderFactory build)
		{
		}

		void ICommandParsable<IUser>.Clear()
		{
		}
	}
}
