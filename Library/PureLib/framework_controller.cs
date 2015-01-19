using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    internal class Controller<T> where T : class , Regulus.Utility.IUpdatable
    {
        
        T _User;
        public string _Name;
        public Controller(string name , T user )
        {            
            _User = user;
            _Name = name;            
        }
        public T User { get { return _User; } }
        public string Name { get { return _Name; } }

        public Regulus.Framework.ICommandParsable<T> Parser { get; set; }

        public Remoting.GPIBinderFactory Builder { get; set; }
    }
}
