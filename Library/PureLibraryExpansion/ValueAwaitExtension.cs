using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regulus.Remoting.Native.Soul;

namespace Regulus.Extension
{
    public static class ValueAwaitExtension
    {
        public static Task<T> ToTask<T>(this Regulus.Remoting.Value<T> value)
        {
            Task<T> t = new Task<T>(() => 
            {
                return new ValueSpin<T>(value).Wait();
            });
            t.Start();
            return t;
        }

        public static T WaitResult<T>(this Regulus.Remoting.Value<T> value)
        {
            var t = value.ToTask();
            t.Wait();
            return t.Result;
        }


        
    }
}


