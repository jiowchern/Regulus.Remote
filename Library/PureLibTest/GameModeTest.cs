using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Regulus
{
    public interface IUser
    {

    }

    class User : IUser
    {

    }
    
    [TestClass]
    public class GameModeTest
    {

        class UserFactoty : Framework.IUserFactoty<IUser>
        {            
            IUser Framework.IUserFactoty<IUser>.Spawn()
            {
                return new User();
            }
        }
        
        [TestMethod]
        public void RootTestSpawnUser()
        {
            var root = new Framework.Root<IUser>(new UserFactoty());

            var user = root.Spawn();
            root.Unspawn(user);

            Assert.AreNotEqual(null , user);
            Assert.AreNotEqual(null , root);
        }
        [TestMethod]
        public void GameModeSelectorInitTest()
        {
            var view = NSubstitute.Substitute.For<Regulus.Utility.Console.IViewer>();
            var parser = NSubstitute.Substitute.For<Regulus.Framework.ICommandParsable<IUser>>();
            
            var input = new Regulus.Utility.ConsoleInput(view);
            var console = new Regulus.Utility.Console(input, view);

            var gameModeSelector = new Regulus.Framework.GameModeSelector<IUser>(console.Command, view, parser);

            var factoryA = NSubstitute.Substitute.For<Framework.IUserFactoty<IUser>>();
            var factoryB = NSubstitute.Substitute.For<Framework.IUserFactoty<IUser>>();

            gameModeSelector.AddFactoty("a", factoryA);
            view.Received().WriteLine( NSubstitute.Arg.Is("Added a factory.") ); 
            gameModeSelector.AddFactoty("b", factoryB);
            view.Received().WriteLine( NSubstitute.Arg.Is("Added b factory.") ); 
            
            Regulus.Framework.GameConsole<IUser> gameConsole = gameModeSelector.CreateGameConsole("a");
            view.Received().WriteLine(NSubstitute.Arg.Is("Create Game Console Factory:a.")); 

            Assert.AreNotEqual(null, gameConsole);
        }

        [TestMethod]
        public void GameConsoleInitTest()
        {
           
            var view = NSubstitute.Substitute.For<Regulus.Utility.Console.IViewer>();
            var input = new Regulus.Utility.ConsoleInput(view);
            var console = new Regulus.Utility.Console(input, view);
            var parser = NSubstitute.Substitute.For<Regulus.Framework.ICommandParsable<IUser>>();

            var factoryA = NSubstitute.Substitute.For<Framework.IUserFactoty<IUser>>();
            Regulus.Framework.GameConsole<IUser> gameConsole = new Framework.GameConsole<IUser>(parser , factoryA, view, console.Command);

            var user = gameConsole.Spawn("jc1");

            
            var result = gameConsole.Select("jc1");
            parser.Received().Register(Arg.Is(user), Arg.Any<Regulus.Utility.Command>());

            Assert.AreNotEqual(null, user);
            Assert.AreEqual(true, result);

            gameConsole.Unspawn("jc1");
            parser.Received().Unregister(Arg.Is(user), Arg.Any<Regulus.Utility.Command>());
            result = gameConsole.Select("jc1");
            Assert.AreEqual(false, result);
        }
        

    }
}
