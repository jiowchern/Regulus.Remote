// BindHandler.cs
using Regulus.Remote.Packages;
using Regulus.Remote;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace Regulus.Remote
{
    internal class SoulBindHandler
    {
        private readonly IdLandlord _IdLandlord;
        private readonly IResponseQueue _Queue;
        private readonly IProtocol _Protocol;
        private readonly ConcurrentDictionary<long, SoulProxy> _Souls;
        private readonly ISerializable _Serializer;
        private readonly IInternalSerializable _InternalSerializable;
        private readonly EventProvider _EventProvider;

        public SoulBindHandler(
            IdLandlord idLandlord,
            IResponseQueue queue,
            IProtocol protocol,
            ConcurrentDictionary<long, SoulProxy> souls,
            ISerializable serializer,
            IInternalSerializable internalSerializable,
            EventProvider eventProvider)
        {
            _IdLandlord = idLandlord;
            _Queue = queue;
            _Protocol = protocol;
            _Souls = souls;
            _Serializer = serializer;
            _InternalSerializable = internalSerializable;
            _EventProvider = eventProvider;
        }

        public ISoul Return<TSoul>(TSoul soul)
        {
            if (soul == null)
                throw new ArgumentNullException(nameof(soul));

            return _Bind(soul, true, 0);
        }

        public ISoul Bind<TSoul>(TSoul soul)
        {
            return _Bind(soul, false, 0);
        }

        public void Unbind(ISoul soul)
        {
            _Unbind(soul);
        }

        private ISoul _Bind<TSoul>(TSoul soul, bool returnType, long returnId)
        {
            return _Bind(soul, typeof(TSoul), returnType, returnId);
        }

        private SoulProxy _Bind(object soul, Type soulType, bool returnType, long returnId)
        {
            var newSoul = _NewSoul(soul, soulType);
            _LoadSoul(newSoul.InterfaceId, newSoul.Id, returnType);
            _LoadProperty(newSoul);
            _LoadSoulCompile(newSoul.InterfaceId, newSoul.Id, returnId);
            newSoul.Initial(_Protocol.GetMemberMap().Propertys.Item1s);
            return newSoul;
        }

        private SoulProxy _NewSoul(object soul, Type soulType)
        {
            int interfaceId = _Protocol.GetMemberMap().GetInterface(soulType);
            var newSoul = new SoulProxy(_IdLandlord.Rent(), interfaceId, soulType, soul);
            newSoul.SupplySoulEvent += _PropertyBind;
            newSoul.UnsupplySoulEvent += _PropertyUnbind;
            newSoul.PropertyChangedEvent += _LoadProperty;
            _Souls.TryAdd(newSoul.Id, newSoul);
            return newSoul;
        }

        private void _Unbind(ISoul soul)
        {
            if (!_Souls.TryRemove(soul.Id, out var soulInfo))
                throw new Exception($"Can't find the soul {soul.Id} to delete.");

            soulInfo.Release();
            soulInfo.SupplySoulEvent -= _PropertyBind;
            soulInfo.UnsupplySoulEvent -= _PropertyUnbind;
            soulInfo.PropertyChangedEvent -= _LoadProperty;
            _UnloadSoul(soulInfo.InterfaceId, soulInfo.Id);
            _IdLandlord.Return(soulInfo.Id);
        }

        private void _LoadSoul(int typeId, long id, bool returnType)
        {
            var package = new PackageLoadSoul
            {
                TypeId = typeId,
                EntityId = id,
                ReturnType = returnType
            };
            _Queue.Push(ServerToClientOpCode.LoadSoul, _InternalSerializable.Serialize(package));
        }
        private void _LoadProperty(long soul, IPropertyIdValue prop)
        {
            _LoadProperty(soul, prop.Id, prop.Instance);
        }
        private void _LoadProperty(SoulProxy soul)
        {
            var properties = soul.PropertyInfos;
            var map = _Protocol.GetMemberMap();

            foreach (var property in properties)
            {
                int id = map.GetProperty(property);
                if (typeof(IDirtyable).IsAssignableFrom(property.PropertyType))
                {
                    var value = property.GetValue(soul.ObjectInstance);
                    var accessable = value as IAccessable;
                    _LoadProperty(soul.Id, id, accessable.Get());
                }
            }
        }

        private void _LoadProperty(long id, int propertyId, object value)
        {
            var info = _Protocol.GetMemberMap().GetProperty(propertyId);
            var package = new PackageSetProperty
            {
                EntityId = id,
                Property = propertyId,
                Value = _Serializer.Serialize(info.PropertyType, value).ToArray()
            };
            _Queue.Push(ServerToClientOpCode.SetProperty, _InternalSerializable.Serialize(package));
        }

        private void _LoadSoulCompile(int typeId, long id, long returnId)
        {
            var package = new PackageLoadSoulCompile
            {
                EntityId = id,
                ReturnId = returnId,
                TypeId = typeId
            };
            _Queue.Push(ServerToClientOpCode.LoadSoulCompile, _InternalSerializable.Serialize(package));
        }

        private void _UnloadSoul(int typeId, long id)
        {
            var package = new PackageUnloadSoul { EntityId = id };
            _Queue.Push(ServerToClientOpCode.UnloadSoul, _InternalSerializable.Serialize(package));
        }

        private ISoul _PropertyBind(long soulId, int propertyId, TypeObject typeObject)
        {
            var soul = _Bind(typeObject.Instance, typeObject.Type, false, 0);
            var package = new PackagePropertySoul
            {
                OwnerId = soulId,
                PropertyId = propertyId,
                EntityId = soul.Id
            };
            _Queue.Push(ServerToClientOpCode.AddPropertySoul, _InternalSerializable.Serialize(package));
            return soul;
        }

        private void _PropertyUnbind(long soulId, int propertyId, long propertySoulId)
        {
            var package = new PackagePropertySoul
            {
                OwnerId = soulId,
                PropertyId = propertyId,
                EntityId = propertySoulId
            };
            _Queue.Push(ServerToClientOpCode.RemovePropertySoul, _InternalSerializable.Serialize(package));

            if (_Souls.TryGetValue(propertySoulId, out var soul))
            {
                _Unbind(soul);
            }
        }
    }

}
