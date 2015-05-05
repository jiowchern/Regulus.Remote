using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLibTest
{
    class EnqueueHelper
    {
        private Regulus.Collection.Queue<int> ints;
        private int i;

        public EnqueueHelper(Regulus.Collection.Queue<int> ints, int i)
        {
            // TODO: Complete member initialization
            this.ints = ints;
            this.i = i;
        }
        internal void Run()
        {
            ints.Enqueue(i);
        }
    }
}
