using System;
using System.Linq;
using System.Reflection;
using static Regulus.Remote.SystemReflectionExtensions;
namespace Regulus.Remote
{
    class GhostResponseHandler
    {
        public readonly IGhost Base;
        readonly MemberMap _MemberMap;
        readonly ISerializable _Serializer;
        public GhostResponseHandler(IGhost ghost, MemberMap map , ISerializable serializable)
        {
            _MemberMap = map;
            _Serializer = serializable;
            Base = ghost;
        }

        public void UpdateSetProperty(int property, byte[] payload)
        {
            var ghost = Base;
            
            var map = _MemberMap;
            var info = map.GetProperty(property);
            var value = _Serializer.Deserialize(info.DeclaringType, payload);

            object instance = ghost.GetInstance();
            var type = instance.GetType();

            var fieldName = $"_{info.GetPathName()}";
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new Exception($"Can't find field {fieldName}.");
            }

            var filedValue = field.GetValue(instance);
            var updateable = filedValue as IAccessable;
            updateable.Set(value);            
        }

        public void InvokeEvent(int event_id, long handler_id, byte[][] event_params)
        {
            IGhost ghost = Base;            

            MemberMap map = _MemberMap;
            EventInfo info = map.GetEvent(event_id);

            object instance = ghost.GetInstance();
            Type type = instance.GetType();

            var fieldName = info.GetPathName();
            FieldInfo eventInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            object fieldValue = eventInfo.GetValue(instance);
            if (fieldValue is GhostEventHandler fieldValueDelegate)
            {
                object[] pars = (from payload in event_params select _Serializer.Deserialize(eventInfo.FieldType, payload)).ToArray();
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
    }
}
