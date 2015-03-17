using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VGame.Project.FishHunter.Formula
{
    class DummyInputView : Regulus.Utility.Console.IInput, Regulus.Utility.Console.IViewer
    {

        void Regulus.Utility.Console.IViewer.WriteLine(string message)
        {
            
        }

        void Regulus.Utility.Console.IViewer.Write(string message)
        {
            
        }

        event Regulus.Utility.Console.OnOutput Regulus.Utility.Console.IInput.OutputEvent
        {
            add {  }
            remove {  }
        }
    }
    public class RemotingClient : Client
    {
        public delegate void UserCallback(IUser user);
        public event UserCallback UserEvent;
        RemotingClient(Regulus.Utility.Console.IInput input, Regulus.Utility.Console.IViewer view)
            : base(view, input)
        {
            ModeSelectorEvent += RemotingClient_ModeSelectorEvent;
        }
        void RemotingClient_ModeSelectorEvent(Regulus.Framework.GameModeSelector<IUser> selector)
        {            
            selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.RemotingUserFactory());
            
            var provider = selector.CreateUserProvider("remoting");

            var user = provider.Spawn("1");
            provider.Select("1");
            if (UserEvent != null)
                UserEvent(user);
        }


        static public RemotingClient Create()
        {
            var dummpy = new DummyInputView();
            var client = new RemotingClient(dummpy ,dummpy);
            return client;
        }
    }
}
