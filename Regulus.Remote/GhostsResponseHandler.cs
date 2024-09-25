using Regulus.Remote.Packages;

namespace Regulus.Remote
{
    namespace ProviderHelper
    {
        public class GhostsResponseHandler
        {
            private readonly IInternalSerializable _InternalSerializer;
            private readonly GhostsManager _GhostManager;
            private readonly GhostsReturnValueHandler _ReturnValueHandler;
            private readonly GhostsPingHandler _PingHandler;
            private readonly GhostsProviderManager _ProviderManager;
            private readonly IOpCodeExchangeable _Exchanger;
            private readonly IProtocol _Protocol;
            private bool _Active;

            public bool Active => _Active;

            public GhostsResponseHandler(
                IInternalSerializable internalSerializer,
                GhostsManager ghostManager,
                GhostsReturnValueHandler returnValueHandler,
                GhostsPingHandler pingHandler,
                GhostsProviderManager ghostsProviderManager,
                IOpCodeExchangeable exchanger,
                IProtocol protocol)
            {
                _InternalSerializer = internalSerializer;
                _GhostManager = ghostManager;
                _ReturnValueHandler = returnValueHandler;
                _PingHandler = pingHandler;
                _Exchanger = exchanger;
                _Protocol = protocol;
                _ProviderManager = ghostsProviderManager;
            }

            public void OnResponse(ServerToClientOpCode code, Regulus.Memorys.Buffer args)
            {
                _GhostManager.UpdateAutoRelease();

                switch (code)
                {
                    case ServerToClientOpCode.Ping:
                        _PingHandler.HandlePingResponse();
                        break;

                    case ServerToClientOpCode.SetProperty:
                        var setPropertyData = (PackageSetProperty)_InternalSerializer.Deserialize(args);
                        _GhostManager.UpdateSetProperty(setPropertyData.EntityId, setPropertyData.Property, setPropertyData.Value);
                        break;

                    case ServerToClientOpCode.InvokeEvent:
                        var invokeEventData = (PackageInvokeEvent)_InternalSerializer.Deserialize(args);
                        _GhostManager.InvokeEvent(invokeEventData.EntityId, invokeEventData.Event, invokeEventData.HandlerId, invokeEventData.EventParams);
                        break;

                    case ServerToClientOpCode.ErrorMethod:
                        var errorMethodData = (PackageErrorMethod)_InternalSerializer.Deserialize(args);
                        _ReturnValueHandler.ErrorReturnValue(errorMethodData.ReturnTarget, errorMethodData.Method, errorMethodData.Message);
                        break;

                    case ServerToClientOpCode.ReturnValue:
                        var returnValueData = (PackageReturnValue)_InternalSerializer.Deserialize(args);
                        _ReturnValueHandler.SetReturnValue(returnValueData.ReturnTarget, returnValueData.ReturnValue);
                        break;

                    case ServerToClientOpCode.LoadSoulCompile:
                        var loadSoulCompileData = (PackageLoadSoulCompile)_InternalSerializer.Deserialize(args);
                        LoadSoulCompile(loadSoulCompileData.TypeId, loadSoulCompileData.EntityId, loadSoulCompileData.ReturnId);
                        break;

                    case ServerToClientOpCode.LoadSoul:
                        var loadSoulData = (PackageLoadSoul)_InternalSerializer.Deserialize(args);
                        _GhostManager.LoadSoul(loadSoulData.TypeId, loadSoulData.EntityId, loadSoulData.ReturnType);
                        break;

                    case ServerToClientOpCode.UnloadSoul:
                        var unloadSoulData = (PackageUnloadSoul)_InternalSerializer.Deserialize(args);
                        _GhostManager.UnloadSoul(unloadSoulData.EntityId);
                        break;

                    case ServerToClientOpCode.AddPropertySoul:
                        var addPropertySoulData = (PackagePropertySoul)_InternalSerializer.Deserialize(args);
                        _GhostManager.AddPropertySoul(addPropertySoulData);
                        break;

                    case ServerToClientOpCode.RemovePropertySoul:
                        var removePropertySoulData = (PackagePropertySoul)_InternalSerializer.Deserialize(args);
                        _GhostManager.RemovePropertySoul(removePropertySoulData);
                        break;

                    case ServerToClientOpCode.ProtocolSubmit:
                        var protocolSubmitData = (PackageProtocolSubmit)_InternalSerializer.Deserialize(args);
                        ProtocolSubmit(protocolSubmitData);
                        break;

                    default:
                        // 處理其他未定義的操作碼
                        break;
                }
            }
            /*
             private void _LoadSoulCompile(int type_id, long entity_id, long return_id )
        {
            
            MemberMap map = _Protocol.GetMemberMap();
            
            Type type = map.GetInterface(type_id);
            
            IProvider provider = _QueryProvider(type);

            var handler = _FindHandler(entity_id);
            if (return_id != 0)
            {
                
                IValue value = _ReturnValueQueue.PopReturnValue(return_id);
                if (value != null)
                {
                    value.SetValue(ghost);
                }
            }
            else
            {
                provider.Ready(entity_id);
            }
                
        }
             */
            private void LoadSoulCompile(int typeId, long entityId, long returnId)
            {
                var map = _Protocol.GetMemberMap();
                var type = map.GetInterface(typeId);
                var provider = _ProviderManager.QueryProvider(type);

                var handler = _GhostManager.FindHandler(entityId);
                if (returnId != 0)
                {
                    var ghost = handler.FindGhost();
                    _ReturnValueHandler.PopReturnValue(returnId, ghost);
                }
                else
                {
                    provider.Ready(entityId);
                }
            }

            private void ProtocolSubmit(PackageProtocolSubmit data)
            {
                _Active = Comparison(_Protocol.VerificationCode, data.VerificationCode);
            }

            private bool Comparison(byte[] code1, byte[] code2)
            {
                return new Regulus.Utility.Comparer<byte>(code1, code2, (arg1, arg2) => arg1 == arg2).Same;
            }
        }

    }
}
