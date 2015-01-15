using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    internal struct Controller<T> where T : class , Regulus.Utility.IUpdatable
    {
        Regulus.Framework.ICommandParsable<T> _Parser;        
        T _User;
        public string _Name;
        public Controller(string name , T user , Regulus.Framework.ICommandParsable<T> parser)
        {
            _Parser = parser;
            _User = user;
            _Name = name;
        }
        public T User { get { return _User; } }
        public string Name { get { return _Name; } }

        public Regulus.Framework.ICommandParsable<T> Parser { get { return _Parser; } }
    }
}
