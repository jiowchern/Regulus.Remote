using Regulus.Remote;
using Regulus.Remote.Reactive;
using System.Linq;
using System.Reactive.Linq;
using Regulus.Utility;
using System.Diagnostics;
using System;

namespace Regulus.Integration.Tests
{
    namespace SimulateReals
    {

        namespace Client
        {
            class StayStatus : Regulus.Utility.IStatus
            {
                readonly Stopwatch _Stopwatch;
                private readonly int _MilliSeconds;
                private readonly INotifierQueryable _QueryNotifier;
                private IDisposable _Dispose;

                public event System.Action DoneEvent;
                public StayStatus(int seconds , Regulus.Remote.INotifierQueryable agent)
                {
                    _Stopwatch = new Stopwatch();
                    _Stopwatch.Restart();
                    this._MilliSeconds = seconds * 1000;
                    this._QueryNotifier = agent;

                    var obs = from gpi in _QueryNotifier.QueryNotifier<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>().SupplyEvent()
                              from v1 in gpi.SayHello(new Remote.Tools.Protocol.Sources.TestCommon.HelloRequest() {Name ="1" }).RemoteValue()
                              select v1;
                    _Dispose = obs.Subscribe(_GetResult);
                }

                private void _GetResult(Remote.Tools.Protocol.Sources.TestCommon.HelloReply reply)
                {
                    var obs = from gpi in _QueryNotifier.QueryNotifier<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>().SupplyEvent()
                              from v1 in gpi.SayHello(new Remote.Tools.Protocol.Sources.TestCommon.HelloRequest() { Name = "1" }).RemoteValue()
                              select v1;
                    _Dispose = obs.Subscribe(_GetResult);
                }

                public void Enter()
                {
                }

                public void Leave()
                {
                    _Dispose.Dispose();
                }

                void IStatus.Update()
                {
                    if(_Stopwatch.ElapsedMilliseconds >= _MilliSeconds )
                    {
                        DoneEvent();
                    }
                }
            }
        }
    }
    
}