using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VGame.Project.FishHunter.Formula
{
    public class Client :Regulus.Framework.Client<IUser>
    {

        public Client()
            : base(new Regulus.Utility.DummyView(), new Regulus.Utility.DummyInput())
        {

        }
        public Client(Regulus.Utility.Console.IViewer view, Regulus.Utility.Console.IInput input)
            : base(view , input)
        {
            
        }        
    }
}
