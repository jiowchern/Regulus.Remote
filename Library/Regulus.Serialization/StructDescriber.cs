using System;
using System.Runtime.InteropServices;



namespace Regulus.Serialization
{
    public class StructDescriber<T> : StructDescriber
    {
        public StructDescriber(int id) : base(id, typeof (T))
        {
            
        }
    }
    public class StructDescriber : ITypeDescriber 
    {
        private readonly int _Id;

        private readonly Type _Type;

        private readonly object _Default;

        public StructDescriber(int id, Type type)
        {
            _Id = id;
            _Type = type;

            _Default = Activator.CreateInstance(type);
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        Type ITypeDescriber.Type
        {
            get { return _Type; }
        }

        object ITypeDescriber.Default
        {
            get { return _Default; }
        }

        int ITypeDescriber.GetByteCount(object instance)
        {
            return Marshal.SizeOf(_Type);
        }

        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {
         
            int readCount;
            GCHandle pinStructure = GCHandle.Alloc(instance, GCHandleType.Pinned);
            try
            {
                readCount = Marshal.SizeOf(_Type);
                Marshal.Copy(pinStructure.AddrOfPinnedObject(), buffer, begin, readCount);
            }
            finally
            {
                if (pinStructure.IsAllocated)
                    pinStructure.Free();
            }

            return readCount;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {
            int size = Marshal.SizeOf(_Type);
            
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, begin, ptr, size);

            instnace = Marshal.PtrToStructure(ptr, _Type);
            Marshal.FreeHGlobal(ptr);


            return size;
        }

        void ITypeDescriber.SetMap(TypeSet type_set)
        {
            
        }
    }
}