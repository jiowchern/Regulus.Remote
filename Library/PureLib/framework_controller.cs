using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    internal struct Controller<T> where T : class
    {
        T _User;
        public string _Name;
        public Controller(string name , T user)
        {
            _User = user;
            _Name = name;
        }
        public T User { get { return _User; } }
        public string Name { get { return _Name; } }
    }
}
