

using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;

using System.Timers;


using Regulus.Utility;

using Regulus.Serialization;


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

		private IGhostRequest _Requester;

		private readonly InterfaceProvider _InterfaceProvider;
	    private readonly ISerializer _Serializer;
		readonly SoulNotifier _NotifierPassage;

		public void AddProvider(Type type, IProvider provider)
		{
			_Providers.Add(type , provider);
		}
		private IProtocol _Protocol;

        public event Action<byte[], byte[]> ErrorVerifyEvent;

        public long Ping { get; private set; }

		public bool Enable { get; private set; }

		public GhostProvider(IProtocol protocol, IGhostRequest req)
		{
			_Requester = req;
			_Requester.ResponseEvent += OnResponse;
			_NotifierPassage = new SoulNotifier();
			_ReturnValueQueue = new ReturnValueQueue();
            _Protocol = protocol;
            _InterfaceProvider = _Protocol.GetInterfaceProvider();
		    _Serializer = _Protocol.GetSerialize();
		    _Providers = new Dictionary<Type, IProvider>();
            _AutoRelease = new AutoRelease(_Requester , _Serializer);
			_StartPing();
			Enable = true;
		}

        public void Dispose()
        {
			_Requester.ResponseEvent -= OnResponse;
			Enable = false;
			lock (_Providers)
			{
				foreach (var providerPair in _Providers)
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
			if(id == ServerToClientOpCode.Ping)
			{
				Ping = _PingTimeCounter.Ticks;
				_StartPing();
			}
			else if (id == ServerToClientOpCode.SetProperty)
			{
				var data = args.ToPackageData<PackageSetProperty>(_Serializer);
				_UpdateSetProperty(data.EntityId, data.Property, data.Value);
			}
			
			else if(id == ServerToClientOpCode.InvokeEvent)
			{
                var data = args.ToPackageData<PackageInvokeEvent>(_Serializer);
                _InvokeEvent(data.EntityId, data.Event,data.HandlerId, data.EventParams);
            }
			else if(id == ServerToClientOpCode.ErrorMethod)
			{
                var data = args.ToPackageData<PackageErrorMethod>(_Serializer);

                _ErrorReturnValue(data.ReturnTarget, data.Method, data.Message);
            }
			else if(id == ServerToClientOpCode.ReturnValue)
			{

                var data = args.ToPackageData<PackageReturnValue>(_Serializer);
                _SetReturnValue(data.ReturnTarget, data.ReturnValue);
            }
			else if(id == ServerToClientOpCode.LoadSoulCompile)
			{

                var data = args.ToPackageData<PackageLoadSoulCompile>(_Serializer);
                _LoadSoulCompile(data.TypeId, data.EntityId, data.ReturnId,data.PassageId);
            }
			else if(id == ServerToClientOpCode.LoadSoul)
			{
                var data = args.ToPackageData<PackageLoadSoul>(_Serializer);
                _LoadSoul(data.TypeId, data.EntityId, data.ReturnType);
            }
			else if(id == ServerToClientOpCode.UnloadSoul)
			{
                var data = args.ToPackageData<PackageUnloadSoul>(_Serializer);
                _UnloadSoul(data.TypeId, data.EntityId, data.PassageId);
            }
            else if (id == ServerToClientOpCode.ProtocolSubmit)
            {
                var data = args.ToPackageData<PackageProtocolSubmit>(_Serializer);

                _ProtocolSubmit(data);
            }

        }

        private void _ProtocolSubmit(PackageProtocolSubmit data)
        {

            if (_Comparison(_Protocol.VerificationCode, data.VerificationCode) == false)
                ErrorVerifyEvent(_Protocol.VerificationCode , data.VerificationCode);


        }

        private bool _Comparison(byte[] code1, byte[] code2)
        {
            return new Regulus.Utility.Comparison<byte>(code1 , code2 , (arg1, arg2) => arg1 == arg2).Same;
        }

        private void _ErrorReturnValue(long return_target, string method, string message)
		{
			_ReturnValueQueue.PopReturnValue(return_target);

			if(ErrorMethodEvent != null)
			{
				ErrorMethodEvent(method , message);
			}
		}

		private void _SetReturnValue(long returnTarget, byte[] returnValue)
		{
			var value = _ReturnValueQueue.PopReturnValue(returnTarget);
			if(value != null)
			{
			    var returnInstance = _Serializer.Deserialize(returnValue);
                value.SetValue(returnInstance);
			}
		}

		private void _SetReturnValue(long return_id, IGhost ghost)
		{
			var value = _ReturnValueQueue.PopReturnValue(return_id);
			if(value != null)
			{
				value.SetValue(ghost);
			}
		}

		private void _LoadSoulCompile(int type_id, long entity_id, long return_id,long passage_id)
		{
		    var map = _Protocol.GetMemberMap();

		    var type = map.GetInterface(type_id);

            var provider = _QueryProvider(type);
			if(provider != null)
			{
				var ghost = provider.Ready(entity_id);				
				_NotifierPassage.Supply(ghost, passage_id);
				_SetReturnValue(return_id, ghost);				
			}
		}

        

        private void _LoadSoul(int type_id, long id, bool return_type)
		{
		    var map = _Protocol.GetMemberMap();		                
            var type = map.GetInterface(type_id);
            var provider = _QueryProvider(type);
			var ghost = _BuildGhost(type, id, return_type);

		    ghost.CallMethodEvent += new GhostMethodHandler(ghost, _ReturnValueQueue , _Protocol , _Requester ).Run;
			ghost.AddEventEvent += new GhostEventMoveHandler(ghost,  _Protocol, _Requester).Add;
			ghost.RemoveEventEvent += new GhostEventMoveHandler(ghost, _Protocol, _Requester).Remove;
			ghost.AddSupplyNoitfierEvent += new GhostNotifierHandler(ghost, _Protocol, _Requester,_NotifierPassage).AddSupply;
			ghost.RemoveSupplyNoitfierEvent += new GhostNotifierHandler(ghost, _Protocol, _Requester, _NotifierPassage).RemoveSupply;
			ghost.AddUnsupplyNoitfierEvent += new GhostNotifierHandler(ghost, _Protocol, _Requester, _NotifierPassage).AddUnsupply;
			ghost.RemoveUnsupplyNoitfierEvent += new GhostNotifierHandler(ghost, _Protocol, _Requester, _NotifierPassage).RemoveUnsupply;



			provider.Add(ghost);

			if(ghost.IsReturnType())
			{
				_RegisterRelease(ghost);
			}
		}

		private void _RegisterRelease(IGhost ghost)
		{
			_AutoRelease.Register(ghost);
		}

		private void _UnloadSoul(int type_id, long id,long passage_id)
		{
		    var map = _Protocol.GetMemberMap();
		    var type = map.GetInterface(type_id);
            var provider = _QueryProvider(type);
			if(provider == null)
			{
				return;
			}

			var ghost = provider.Ghosts.FirstOrDefault(g => g.GetID() == id);
			if (ghost == null)
				return;			
			_NotifierPassage.Unsupply(ghost, passage_id);
			provider.Remove(id);			
		}

		private IProvider _QueryProvider(Type type)
		{
            IProvider provider = null;
			lock(_Providers)
			{
				if(_Providers.TryGetValue(type, out provider) == false)
				{
                    provider = _BuildProvider(type);
                    _Providers.Add(type, provider);
                }
			}

			return provider;
		}

		private IProvider _BuildProvider(Type type)
		{
		    var map = _Protocol.GetMemberMap();
		    return map.CreateProvider(type);
		}

		public INotifier<T> QueryProvider<T>()
		{
			return _QueryProvider(typeof(T)) as INotifier<T>;
		}

		private void _UpdateSetProperty(long entity_id, int property, byte[] buffer)
		{
			var ghost = _FindGhost(entity_id);
			if (ghost == null)
				return;

			var map = _Protocol.GetMemberMap();
			var info = map.GetProperty(property);
			
			var value = _Serializer.Deserialize(buffer);
			var instance = ghost.GetInstance();
			var type = _InterfaceProvider.Find(info.DeclaringType);
			var field = type.GetField("_" + info.Name, BindingFlags.Instance | BindingFlags.Public);
			if (field != null)
			{
				
				var filedValue = field.GetValue(instance);
				var updateable =  filedValue as IAccessable;
				updateable.Set(value);


				var pkg = new PackageSetPropertyDone();
				pkg.EntityId = entity_id;
				pkg.Property = property;
				_Requester.Request(ClientToServerOpCode.UpdateProperty, pkg.ToBuffer(_Serializer));
			}

			
		}
		

		private void _InvokeEvent(long ghost_id, int event_id,long handler_id, byte[][] event_params)
		{
			var ghost = _FindGhost(ghost_id);
			if(ghost != null)
			{
			    var map = _Protocol.GetMemberMap();
			    var info = map.GetEvent(event_id);

                
                Type type = _InterfaceProvider.Find(info.DeclaringType);
			    var instance = ghost.GetInstance();
                if (type != null)
                {
                    var eventInfos = type.GetField("_" + info.Name, BindingFlags.Instance | BindingFlags.Public);
                    var fieldValue = eventInfos.GetValue(instance);
                    if (fieldValue is GhostEventHandler)
                    {
                        var fieldValueDelegate = fieldValue as GhostEventHandler;

                        var pars = (from a in event_params select _Serializer.Deserialize(a)).ToArray();
                        try
                        {
                            fieldValueDelegate.Invoke(handler_id,pars);
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
            }
		}

		private IGhost _FindGhost(long ghost_id)
		{
			lock(_Providers)
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
			lock(_Sync)
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
			lock(_Sync)
			{
				if(_PingTimer != null)
				{
					_PingTimeCounter = new TimeCounter();
					_Requester.Request(ClientToServerOpCode.Ping, new byte[0]);
				}
			}

			_EndPing();
		}

		protected void _EndPing()
		{
			lock(_Sync)
			{
				if(_PingTimer != null)
				{
					_PingTimer.Stop();
					_PingTimer = null;
				}
			}
		}

		private IGhost _BuildGhost(Type ghost_base_type, long id, bool return_type)
		{
			

			var ghostType = _QueryGhostType(ghost_base_type);

		    var constructor = ghostType.GetConstructor(new[] {typeof (long),typeof (bool)});
		    if (constructor == null)
		    {
		        List<string> constructorInfos = new List<string>();

                foreach (var constructorInfo in ghostType.GetConstructors())
		        {
                    constructorInfos.Add("(" + constructorInfo.GetParameters()+ ")");

                }
                throw new Exception(string.Format("{0} Not found constructor.\n{1}" , ghostType.FullName , string.Join("\n" , constructorInfos.ToArray())));
            }
                

		    var o = constructor.Invoke(new object[] { id, return_type});            

            return (IGhost)o;
		}

       

        private Type _QueryGhostType(Type ghostBaseType)
		{			
			return _InterfaceProvider.Find(ghostBaseType);
        }

		public event Action<string, string> ErrorMethodEvent;
	}
}
