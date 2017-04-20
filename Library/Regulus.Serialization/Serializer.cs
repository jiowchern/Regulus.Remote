using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Regulus.Extension;
using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class Serializer
    {
        private readonly ITypeProvider[] _TypeProviders;

        public Serializer(params ITypeProvider[] type_providers)
        {
            _TypeProviders = type_providers;
        }

        IEnumerable<int> _GetDataSizes(System.Type type, object instance)
        {
            var provider = (from p in _TypeProviders where p.InstanceType == type select p).Single();

            int size = provider.GetBufferSize(instance);
            yield return size;
            
            if (_HasElement(type))
            {
                var eleType = type.GetElementType();
                var list = instance as System.Collections.IList;
                for (int i = 0; i < list.Count; i++)
                {
                    var eleInstance = list[i];
                    if (eleInstance != null)
                        yield return _GetDataSizes(eleType, eleInstance).Sum();
                }
            }
            if ( type.IsEnum == false )
            {
                var fields = type.GetFields();
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.IsPublic && fieldInfo.IsStatic == false)
                    {
                        var fieldInstance = fieldInfo.GetValue(instance);
                        if (fieldInstance != null)
                            yield return _GetDataSizes(fieldInfo.FieldType, fieldInstance).Sum();
                    }
                }
            }
            
            
        }

        private bool _HasElement(Type type)
        {
            var eleType = type.GetElementType();
            return type.IsArray && (eleType.IsClass || eleType.IsEnum);
        }

        IEnumerable<int> _GetInstanceAmount(System.Type type, object instance)
        {
            yield return 1;

            if (_HasElement(type))
            {
                var eleType = type.GetElementType();
                var list = instance as System.Collections.IList;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var eleInstance = list[i];
                        if (eleInstance != null)
                            yield return _GetInstanceAmount(eleType, eleInstance).Sum();
                    }
                }
            }

            if (type.IsEnum == false)
            {
                var fields = type.GetFields();

                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.IsPublic && fieldInfo.IsStatic == false)
                    {
                        var eleInstance = fieldInfo.GetValue(instance);
                        if (eleInstance != null)
                            yield return _GetInstanceAmount(fieldInfo.FieldType, eleInstance).Sum();
                    }
                }
            }
            
            
        }

        IEnumerable<int> _GetValueAmount(System.Type type, object instance)
        {
            yield return 1;

            if (type.IsEnum == false)
            {
                var fields = type.GetFields();
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.IsPublic && fieldInfo.IsStatic == false)
                    {
                        var eleInstance = fieldInfo.GetValue(instance);
                        if (eleInstance != null)
                            yield return _GetValueAmount(fieldInfo.FieldType, eleInstance).Sum();
                    }

                }
            }
            

            if (_HasElement(type))
            {
                var eleType = type.GetElementType();
                var list = instance as System.Collections.IList;
                for (int i = 0; i < list.Count; i++)
                {
                    var eleInstance = list[i];
                    if (eleInstance != null)
                        yield return _GetInstanceAmount(eleType, eleInstance).Sum();
                }
            }
            
            
        }

        


        public byte[] Serialize<T>(T obj)
        {

            var type = typeof(T);
            var dataSize = _GetDataSizes(type , obj).Sum();
            var headSize = BufferHead.Size();            
            var instanceAmount = _GetInstanceAmount(type, obj).Sum();
            var instanceSize = InstanceInfo.Size() * instanceAmount;
            var valueAmount = _GetValueAmount(type, obj).Sum();
            var valueSize = ValueInfo.Size() * valueAmount;

            int dataIndex = headSize + instanceSize + valueSize;
            int valueIndex = headSize + instanceSize ;
            int instanceIndex = headSize ;
            var buffer = new byte[headSize + instanceSize + valueSize + dataSize];

            var provider = _FindProvider(type);
            var head = new BufferHead(provider.Id , instanceAmount, valueAmount);

            int readed;
            head.ToBytes(buffer, 0,out readed );
            _SerializeInstance(buffer , instanceIndex , valueIndex , dataIndex, type , obj );
            
            return buffer;
        }

        private void _SerializeInstance(byte[] buffer,int instance_index,int value_index, int data_index, Type type, object instance)
        {
            int id = 0;
            _WriteBuffer(type , instance , ref id, 0 , 0 , buffer , ref instance_index ,ref value_index, ref data_index);
        }

        private void _WriteBuffer(Type type, object instance, ref int id , int parent, int parent_field , byte[] buffer ,ref int instance_index,ref int value_index, ref int data_index)
        {
            ++id;
            var provider = _FindProvider(type);

            int dataReadCount;
            provider.Serialize(instance, buffer, data_index, out dataReadCount);
            if (dataReadCount > 0)
            {
                var info = new ValueInfo(id, data_index, data_index + dataReadCount);
                int valueReadCount;
                info.ToBytes(buffer, value_index, out valueReadCount);

                data_index += dataReadCount;
                value_index += valueReadCount;
            }
            _WriteBufferInstance(type, instance, ref id, parent, parent_field,  buffer,ref instance_index,ref value_index ,ref data_index, provider);

            
            
        }

        private void _WriteBufferInstance(Type type, object instance, ref int id, int parent, int parent_field,
            byte[] buffer, ref int instance_index,ref int value_index, ref int data_index, ITypeProvider provider)
        {
            var info = new InstanceInfo(id,  parent, parent_field);

            int readCount;
            info.ToBuffer(buffer, instance_index, out readCount);
            instance_index += readCount;


            if (_HasElement(type))
            {
                var eleType = type.GetElementType();
                
                var list = instance as System.Collections.IList;

                for (int i = 0; i < list.Count; i++)
                {
                    var eleInstance = list[i];
                    if (eleInstance != null)
                    {
                        _WriteBuffer(eleType, eleInstance, ref id, info.Id, i, buffer,
                            ref instance_index, ref value_index, ref data_index);
                    }
                    
                }
            }
            else if(type.IsClass)
            {
                foreach (var fieldInfo in type.GetFields())
                {
                    if (fieldInfo.IsStatic || fieldInfo.IsPrivate)
                        continue;

                    var field = provider.GetFieldId(fieldInfo.Name);
                    var eleInstance = fieldInfo.GetValue(instance);
                    if(eleInstance != null)
                        _WriteBuffer(fieldInfo.FieldType, eleInstance, ref id, info.Id, field, buffer,
                            ref instance_index, ref value_index, ref data_index);

                }
            }

            
        }

        private ITypeProvider _FindProvider(Type type)
        {
            return (from p in _TypeProviders where p.InstanceType == type select p).Single();
        }

        private ITypeProvider _FindProviderById(int id)
        {
            return (from p in _TypeProviders where p.Id == id select p).Single();
        }


        public T Deserialize<T>(byte[] buffer)
        {
            int point = 0;
            var readCount = 0;
            
            var head = buffer.ToStruct<BufferHead>(0 , out readCount);
            point += readCount;

            var instanceInfos = new List<InstanceInfo>();
            for (int i = 0; i < head.InstanceAmount; i++)
            {
                var info = InstanceInfo.FromBuffer(buffer, point,out readCount);
                point += readCount;
                instanceInfos.Add(info);
            }

            var valueInfos = new List<ValueInfo>();
            for (int i = 0; i < head.ValueAmount; i++)
            {
                var info = buffer.ToStruct<ValueInfo>(point ,out readCount);
                point += readCount;
                valueInfos.Add(info);
            }
            var rootProvider= _FindProviderById(head.Type);
            return (T)_Create(rootProvider , instanceInfos, valueInfos , new BufferInfo(buffer));
        }

        private object _Create(ITypeProvider root_provider ,List<InstanceInfo> instance_infos , List<ValueInfo> value_infos , BufferInfo buffer_info)
        {            
            var typeInstances = new List<TypeInstance>();

            var rootInfo = instance_infos.First();
            typeInstances.Add(_Create(value_infos, buffer_info, rootInfo, root_provider)); 

            foreach (var instanceInfo in instance_infos.Skip(1))
            {

                try
                {
                    var type = (from i in typeInstances where i.Id == instanceInfo.Parent select i.FindFieldType(instanceInfo.ParentField)).Single();
                    var typeId = (from t in _TypeProviders where t.InstanceType == type select t.Id).Single();
                    var provider = (from p in _TypeProviders where p.Id == typeId select p).Single();

                    typeInstances.Add(_Create(value_infos, buffer_info, instanceInfo, provider));
                }
                catch (System.InvalidOperationException ioe)
                {                    
                    throw;
                }
                
            }

            TypeInstance root = null;
            foreach (var instanceInfo in instance_infos)
            {
                var owner = (from t in typeInstances where t.Id == instanceInfo.Id select t).Single();
                var parent = (from t in typeInstances where t.Id == instanceInfo.Parent select t).FirstOrDefault();

                if (parent != null)
                {                    
                    parent.SetReference(instanceInfo.ParentField, owner.GetInstance());
                }
                else
                {
                    root = owner;
                }
            }

            return root.GetInstance();
        }

        private static TypeInstance _Create(List<ValueInfo> value_infos, BufferInfo buffer_info, InstanceInfo instanceInfo, ITypeProvider provider)
        {
            var valueInfo = (from v in value_infos where v.Id == instanceInfo.Id select v).FirstOrDefault();

            var instance = provider.Deserialize(buffer_info.GetBuffer(), valueInfo.Begin, valueInfo.End);
            var typeInstance = new TypeInstance(provider, instance, instanceInfo.Id);
            return typeInstance;
        }

        public class ZigZag
        {
            public static uint Encode(int number)
            {
                return (uint)((number << 1) ^ (number >> 31));
            }


            public static ulong Encode(long number)
            {
                return (ulong)((number << 1) ^ (number >> 63));
            }


            public static int Decode(uint number)
            {
                return (int)(number >> 1) ^ -(int)(number & 1);
            }

            public static long Decode(ulong number)
            {
                return (long)(number >> 1) ^ -(long)(number & 1);
            }
        }

        public class Varint
        {
            

            public static int NumberToBuffer(byte[] buffer, int offset, ulong value)
            {
                int i = 0;
                while (value >= 0x80)
                {
                    buffer[offset + i] = (byte)(value | 0x80);
                    value >>= 7;
                    i++;
                }
                buffer[offset + i] = (byte)value;
                return i + 1;
            }

            public static int GetByteCount(ulong value)
            {
                int i;
                for (i = 0; i < 9 && value >= 0x80; i++, value >>= 7) { }
                return i + 1;
            }
            

            public static int BufferToNumber(byte[] buffer, int offset, out ulong value)
            {
                value = 0;
                int s = 0;
                for (var i = 0; i < buffer.Length - offset; i++)
                {
                    if (buffer[offset + i] < 0x80)
                    {
                        if (i > 9 || i == 9 && buffer[offset + i] > 1)
                        {
                            value = 0;
                            return -(i + 1); // overflow
                        }
                        value |= (ulong)(buffer[offset + i] << s);
                        return i + 1;
                    }
                    value |= (ulong)(buffer[offset + i] & 0x7f) << s;
                    s += 7;
                }
                value = 0;
                return 0;
            }
        }
    }
}


