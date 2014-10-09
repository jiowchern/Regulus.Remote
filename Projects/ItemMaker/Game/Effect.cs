using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItemMaker
{
    public struct Effect
    {
        public enum TYPE
        {
            ATTACKUP

        }
        public TYPE Property;
        public int Value;
    }
}
