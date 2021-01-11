using Regulus.Remote;
using System.Timers;

namespace RegulusLibraryTest
{

    public class RemotingValueResultTest
    {
        [Xunit.Fact(Timeout = 5000)]
        

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
