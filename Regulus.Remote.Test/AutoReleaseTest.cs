using NSubstitute;
using Regulus.Remote;
using Regulus.Serialization;
using System;
using System.Linq;

namespace Regulus.Remote.Test
{
    public class AutoReleaseTests
    {
        [NUnit.Framework.Test]
        public void Test()
        {
            var ar = new AutoRelease<long, IGhost>();
            var ar2 = new AutoRelease<long, IGhost>();

            AddGhost(ar, ar2);            

            GC.Collect(2, GCCollectionMode.Forced,true);

            NUnit.Framework.Assert.AreEqual(1, ar.NoExist().Count());
            NUnit.Framework.Assert.AreEqual(1, ar2.NoExist().Count());
        }

        private static void AddGhost(AutoRelease<long, IGhost> ar , AutoRelease<long, IGhost>  ar2)
        {
            IGhost ghost = new Ghost();
            ar.Push(ghost.GetID(), ghost);
            ar2.Push(ghost.GetID(), ghost);
        }

    }
}
