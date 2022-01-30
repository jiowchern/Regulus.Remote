//using Regulus.Projects.TestProtocol.Common;
using Regulus.Utility;
using System;
using System.Threading;

namespace Regulus.Remote.Standalone.Test
{
   /* public class StatusEntryUserAddItemsStage : Regulus.Utility.IBootable , INext
    {
        readonly Regulus.Projects.TestProtocol.Common.Sample _Sample;
        private readonly IBinder _Binder;
        private ISoul _SampleProxy;
        private ISoul _INextProxy;

        public event System.Action DoneEvent;
        public StatusEntryUserAddItemsStage(IBinder binder)
        {
            _Sample = new Projects.TestProtocol.Common.Sample();
            this._Binder = binder;
        }
        void IBootable.Launch()
        {

            _SampleProxy = _Binder.Bind<ISample>(_Sample);
            _INextProxy = _Binder.Bind<INext>(this);
            _Sample.Numbers.Items.Add(new Number(1));
            _Sample.Numbers.Items.Add(new Number(2));
            _Sample.Numbers.Items.Add(new Number(3));
        }

        Regulus.Remote.Value<bool> INext.Next()
        {
            DoneEvent();
            return true;
        }

        void IBootable.Shutdown()
        {
            _Binder.Unbind(_INextProxy);
            _Sample.Numbers.Items.Clear();            
            _Binder.Unbind(_SampleProxy);
        }
    }

    public class StatusEntryUser : Regulus.Utility.IUpdatable 
    {
        
        private readonly IBinder _Binder;
        readonly Regulus.Utility.StageMachine _Machine;

        public StatusEntryUser(IBinder binder)
        {
            _Machine = new StageMachine();
            this._Binder = binder;
        }

        void IBootable.Launch()
        {
            _ToAddItems();
        }

        private void _ToAddItems()
        {
            var stage = new StatusEntryUserAddItemsStage(_Binder);
            stage.DoneEvent += _ToEmpry;
            _Machine.Push(stage);

        }

        private void _ToEmpry()
        {
            _Machine.Clean();
        }

        void IBootable.Shutdown()
        {
            _Machine.Clean();
        }

        bool IUpdatable.Update()
        {
            return true;
        }
    }

    public class Entry<T> : IBinderProvider, System.IDisposable 
    {
        readonly T _Entry;
        public Entry(T entry)
        {
            _Entry = entry;
        }
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {
            binder.Bind(_Entry);
        }

        void IDisposable.Dispose()
        {            
        }
    }
    public class SampleEntry : IBinderProvider , System.IDisposable
    {
        public readonly Regulus.Projects.TestProtocol.Common.Sample Sample;

        readonly System.IDisposable _Dispose;

        public SampleEntry()
        {
            Sample = new Projects.TestProtocol.Common.Sample();
            _Dispose = Sample;
        }
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {
            binder.Bind<ISample>(Sample);
        }

        void IDisposable.Dispose()
        {
            _Dispose.Dispose();
        }
    }*/
}
