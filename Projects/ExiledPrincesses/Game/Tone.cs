using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game.Stage
{

    class Tone : Regulus.Game.IStage
    {

        public delegate void OnToMap(string name);
        public event OnToMap ToMapEvent;
        void Regulus.Game.IStage.Enter()
        {
            throw new NotImplementedException();
        }

        void Regulus.Game.IStage.Leave()
        {
            throw new NotImplementedException();
        }

        void Regulus.Game.IStage.Update()
        {
            throw new NotImplementedException();
        }
    }
}
