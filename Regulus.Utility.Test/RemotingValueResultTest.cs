using Regulus.Remote;
using System.Timers;

namespace RegulusLibraryTest
{

    public class RemotingValueResultTest
    {
        [NUnit.Framework.Test()]
        [NUnit.Framework.MaxTime(5000)]

        public void TestRemotingValueResult()
        {
            Value<bool> val = new Value<bool>();
            Timer timer = new Timer(1);
            timer.Start();
            timer.Elapsed += (object sender, ElapsedEventArgs e) => { val.SetValue(true); };

            val.Result();
        }


    }
}
