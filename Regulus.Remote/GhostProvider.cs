

using Regulus.Extension;
using Regulus.Serialization;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Regulus.Remote
{
    public class GhostProvider
    {
        private readonly AutoRelease _AutoRelease;

        private readonly Dictionary<Type, IProvider> _Providers;

        private readonly ReturnValueQueue _ReturnValueQueue;

        private readonly object _Sync = new object();

        private TimeCounter _PingTimeCounter = new TimeCounter();

        private Timer _PingTimer;

        private readonly IGhostRequest _Requester;

        private readonly InterfaceProvider _InterfaceProvider;
        private readonly ISerializer _Serializer;        
       
        private readonly IProtocol _Protocol;

        bool _Active;
        public bool Active => _Active;

        public long Ping { get; private set; }

        

        

        public GhostProvider(IProtocol protocol, IGhostRequest req)
        {
            
            _Active = false;
            _Requester = req;
            
            
            _ReturnValueQueue = new ReturnValueQueue();
            _Protocol = protocol;
            _InterfaceProvider = _Protocol.GetInterfaceProvider();
            _Serializer = _Protocol.GetSerialize();
            _Providers = new Dictionary<Type, IProvider>();
            _AutoRelease = new AutoRelease(_Requester, _Serializer);


        }
        public void Start()
        {
            _Requester.ResponseEvent += OnResponse;
            _StartPing();
            
        }
        public void Stop()
        {
            _Requester.ResponseEvent -= OnResponse;
            
            lock (_Providers)
            {
                foreach (KeyValuePair<Type, IProvider> providerPair in _Providers)
                {
                    providerPair.Value.ClearGhosts();
                }
            }

            _EndPing();
        }

        public void OnResponse(ServerToClientOpCode id, byte[] args)
        {
            _OnResponse(id, args);
            _AutoRelease.Update();
        }

        protected void _OnResponse(ServerToClientOpCode id, byte[] args)
        {
            if (id == ServerToClientOpCode.Ping)
            {                
                Ping = _PingTimeCounter.Ticks;
                _StartPing();
            }
            else if (id == ServerToClientOpCode.SetProperty)
            {
                PackageSetProperty data = args.ToPackageData<PackageSetProperty>(_Serializer);
                _UpdateSetProperty(data.EntityId, data.Property, data.Value);
            }

            else if (id == ServerToClientOpCode.InvokeEvent)
            {
                PackageInvokeEvent data = args.ToPackageData<PackageInvokeEvent>(_Serializer);
                _InvokeEvent(data.EntityId, data.Event, data.HandlerId, data.EventParams);
            }
            else if (id == ServerToClientOpCode.ErrorMethod)
            {
                PackageErrorMethod data = args.ToPackageData<PackageErrorMethod>(_Serializer);

                _ErrorReturnValue(data.ReturnTarget, data.Method, data.Message);
            }
            else if (id == ServerToClientOpCode.ReturnValue)
            {

                PackageReturnValue data = args.ToPackageData<PackageReturnValue>(_Serializer);
                _SetReturnValue(data.ReturnTarget, data.ReturnValue);
            }
            else if (id == ServerToClientOpCode.LoadSoulCompile)
            {
                
                PackageLoadSoulCompile data = args.ToPackageData<PackageLoadSoulCompile>(_Serializer);
            
                _LoadSoulCompile(data.TypeId, data.EntityId, data.ReturnId);
                
            }            
            else if (id == ServerToClientOpCode.LoadSoul)
            {
                
                PackageLoadSoul data = args.ToPackageData<PackageLoadSoul>(_Serializer);
            
                _LoadSoul(data.TypeId, data.EntityId, data.ReturnType);
                
            }
            else if (id == ServerToClientOpCode.UnloadSoul)
            {
                PackageUnloadSoul data = args.ToPackageData<PackageUnloadSoul>(_Serializer);
                
                _UnloadSoul(data.EntityId);
            }
            else if (id == ServerToClientOpCode.AddPropertySoul)
            {
                PackagePropertySoul data = args.ToPackageData<PackagePropertySoul>(_Serializer);

                _AddPropertySoul(data);
            }
            else if (id == ServerToClientOpCode.RemovePropertySoul)
            {
                PackagePropertySoul data = args.ToPackageData<PackagePropertySoul>(_Serializer);

                _RemovePropertySoul(data);
            }
            else if (id == ServerToClientOpCode.ProtocolSubmit)
            {
                PackageProtocolSubmit data = args.ToPackageData<PackageProtocolSubmit>(_Serializer);
            
                _ProtocolSubmit(data);
            }

        }

        

        private void _ProtocolSubmit(PackageProtocolSubmit data)
        {
            _Active = _Comparison(_Protocol.VerificationCode, data.VerificationCode);

        }

        private bool _Comparison(byte[] code1, byte[] code2)
        {
            return new Regulus.Utility.Comparison<byte>(code1, code2, (arg1, arg2) => arg1 == arg2).Same;
        }

        private void _ErrorReturnValue(long return_target, string method, string message)
        {
            _ReturnValueQueue.PopReturnValue(return_target);

            if (ErrorMethodEvent != null)
            {
                ErrorMethodEvent(method, message);
            }
        }

        private void _SetReturnValue(long returnTarget, byte[] returnValue)
        {
            IValue value = _ReturnValueQueue.PopReturnValue(returnTarget);
            if (value != null)
            {
                object returnInstance = _Serializer.Deserialize(returnValue);
                value.SetValue(returnInstance);
            }
        }

        private void _ReturnValue(long return_id, IGhost ghost)
        {
            IValue value = _ReturnValueQueue.PopReturnValue(return_id);
            if (value != null)
            {
                value.SetValue(ghost);
            }
        }

        private void _LoadSoulCompile(int type_id, long entity_id, long return_id )
        {
            
            MemberMap map = _Protocol.GetMemberMap();
            
            Type type = map.GetInterface(type_id);
            
            IProvider provider = _QueryProvider(type);

            IGhost ghost = provider.Ready(entity_id);
            if (return_id != 0)
                _ReturnValue(return_id, ghost);
        }

        

        private void _LoadSoul(int type_id, long id, bool return_type)
        {
            MemberMap map = _Protocol.GetMemberMap();
            Type type = map.GetInterface(type_id);
            IProvider provider = _QueryProvider(type);
            IGhost ghost = _BuildGhost(type, id, return_type);

            ghost.CallMethodEvent += new GhostMethodHandler(ghost, _ReturnValueQueue, _Protocol, _Requester).Run;
            ghost.AddEventEvent += new GhostEventMoveHandler(ghost, _Protocol, _Requester).Add;
            ghost.RemoveEventEvent += new GhostEventMoveHandler(ghost, _Protocol, _Requester).Remove;            



            provider.Add(ghost);

            if (ghost.IsReturnType())
            {
                _RegisterRelease(ghost);
            }
        }

        private void _RegisterRelease(IGhost ghost)
        {
            _AutoRelease.Register(ghost);
        }

        private void _UnloadSoul(long id)
        {
            lock(_Providers)
            {
                foreach (var provider in _Providers.Values)
                {
                    provider.Remove(id);
                }
            }
            

            
        }

        private IProvider _QueryProvider(Type type)
        {
            IProvider provider = null;
            lock (_Providers)
            {
                if (_Providers.TryGetValue(type, out provider) == false)
                {
                    provider = _BuildProvider(type);
                    _Providers.Add(type, provider);
                }
            }

            return provider;
        }

        private IProvider _BuildProvider(Type type)
        {
            MemberMap map = _Protocol.GetMemberMap();
            return map.CreateProvider(type);
        }

        public INotifier<T> QueryProvider<T>()
        {
            return _QueryProvider(typeof(T)) as INotifier<T>;
        }
        private void _RemovePropertySoul(PackagePropertySoul data)
        {
            var owner = _FindGhost(data.OwnerId);
            var ghost = _FindGhost(data.EntiryId);
            var accessible = _GetAccesser(data, owner);
            accessible.Remove(ghost.GetInstance());
        }

        private void _AddPropertySoul(PackagePropertySoul data)
        {
            var owner = _FindGhost(data.OwnerId);
            var ghost = _FindGhost(data.EntiryId);
            var accessible = _GetAccesser(data, owner);            
            accessible.Add(ghost.GetInstance());
        }

        private IObjectAccessible _GetAccesser(PackagePropertySoul data, IGhost owner)
        {
            MemberMap map = _Protocol.GetMemberMap();
            PropertyInfo info = map.GetProperty(data.PropertyId);
            var type = _InterfaceProvider.Find(info.DeclaringType);
            var fieldName = _GetFieldName(info);
            FieldInfo field = type.GetField( fieldName, BindingFlags.Instance | BindingFlags.Public);
            object filedValue = field.GetValue(owner.GetInstance());
            return filedValue as IObjectAccessible;
        }

        private static string _GetFieldName(PropertyInfo info)
        {
            return $"_{info.DeclaringType.FullName.Replace('.','_')}_{info.Name}";
        }

        private void _UpdateSetProperty(long entity_id, int property, byte[] buffer)
        {
            IGhost ghost = _FindGhost(entity_id);
            if (ghost == null)
                return;

            MemberMap map = _Protocol.GetMemberMap();
            PropertyInfo info = map.GetProperty(property);

            object value = _Serializer.Deserialize(buffer);
            object instance = ghost.GetInstance();
            Type type = _InterfaceProvider.Find(info.DeclaringType);
            FieldInfo field = type.GetField(_GetFieldName(info), BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                object filedValue = field.GetValue(instance);
                IAccessable updateable = filedValue as IAccessable;
                updateable.Set(value);

                PackageSetPropertyDone pkg = new PackageSetPropertyDone();
                pkg.EntityId = entity_id;
                pkg.Property = property;
                _Requester.Request(ClientToServerOpCode.UpdateProperty, pkg.ToBuffer(_Serializer));
            }
        }


        private void _InvokeEvent(long ghost_id, int event_id, long handler_id, byte[][] event_params)
        {
            IGhost ghost = _FindGhost(ghost_id);
            if (ghost == null)
            {
                return;
            }

            MemberMap map = _Protocol.GetMemberMap();
            EventInfo info = map.GetEvent(event_id);



            object instance = ghost.GetInstance();
            Type type = instance.GetType();
            

            var fieldName = _GetFieldName(info);
            FieldInfo eventInfos = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            object fieldValue = eventInfos.GetValue(instance);
            if (fieldValue is GhostEventHandler)
            {
                GhostEventHandler fieldValueDelegate = fieldValue as GhostEventHandler;

                object[] pars = (from a in event_params select _Serializer.Deserialize(a)).ToArray();
                try
                {
                    fieldValueDelegate.Invoke(handler_id, pars);
                }
                catch (TargetInvocationException tie)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(string.Format("Call event error in {0}:{1}. \n{2}", type.FullName, info.Name, tie.InnerException.ToString()));
                    throw tie;
                }
                catch (Exception e)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(string.Format("Call event error in {0}:{1}.", type.FullName, info.Name));
                    throw e;
                }
            }
            
        }

        private string _GetFieldName(EventInfo info)
        {
            return $"_{info.DeclaringType.FullName.Replace('.', '_')}_{info.Name}";
        }

        private IGhost _FindGhost(long ghost_id)
        {
            lock (_Providers)
            {
                return (from provider in _Providers
                        let r = (from g in provider.Value.Ghosts where ghost_id == g.GetID() select g).FirstOrDefault()
                        where r != null
                        select r).FirstOrDefault();
            }
        }

        protected void _StartPing()
        {
            _EndPing();
            lock (_Sync)
            {
                _PingTimer = new Timer(1000);
                _PingTimer.Enabled = true;
                _PingTimer.AutoReset = true;
                _PingTimer.Elapsed += _PingTimerElapsed;
                _PingTimer.Start();
            }
        }

        private void _PingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_Sync)
            {
                if (_PingTimer != null)
                {
                    _PingTimeCounter = new TimeCounter();
                    _Requester.Request(ClientToServerOpCode.Ping, new byte[0]);
                }
            }

            _EndPing();
        }

        protected void _EndPing()
        {
            lock (_Sync)
            {
                if (_PingTimer != null)
                {
                    _PingTimer.Stop();
                    _PingTimer = null;
                }
            }
        }

        private IGhost _BuildGhost(Type ghost_base_type, long id, bool return_type)
        {


            Type ghostType = _QueryGhostType(ghost_base_type);

            ConstructorInfo constructor = ghostType.GetConstructor(new[] { typeof(long), typeof(bool) });
            if (constructor == null)
            {
                List<string> constructorInfos = new List<string>();

                foreach (ConstructorInfo constructorInfo in ghostType.GetConstructors())
                {
                    constructorInfos.Add("(" + constructorInfo.GetParameters() + ")");

                }
                throw new Exception(string.Format("{0} Not found constructor.\n{1}", ghostType.FullName, string.Join("\n", constructorInfos.ToArray())));
            }


            object o = constructor.Invoke(new object[] { id, return_type });

            return (IGhost)o;
        }



        private Type _QueryGhostType(Type ghostBaseType)
        {
            return _InterfaceProvider.Find(ghostBaseType);
        }

        public event Action<string, string> ErrorMethodEvent;
    }
}
