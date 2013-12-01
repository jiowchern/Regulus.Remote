using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    class ChoiceResource : Resource<ChoiceResource ,int, ChoicePrototype>
    {
        public ChoiceResource() : base()
        { 
            _Resource.Add(1 , new ChoicePrototype() { Maps = new string[] { "Test1" } , Towns = new string[] {} });
            _Resource.Add(2, new ChoicePrototype() { Maps = new string[] { }, Towns = new string[] { "Credits" } });            
        }
    }
}
