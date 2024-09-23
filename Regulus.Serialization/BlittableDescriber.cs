using System;
using System.Runtime.InteropServices;



namespace Regulus.Serialization
{
    public class BlittableDescriber<T> : BlittableDescriber
    {
        public BlittableDescriber() : base(typeof(T))
        {

        }
    }
    public class BlittableDescriber : ITypeDescriber
    {


        private readonly Type _Type;

        private readonly object _Default;

        private readonly int _Size;

        public BlittableDescriber(Type type)
        {

            _Type = type;

            _Default = Activator.CreateInstance(type);
            try
            {
                _Size = Marshal.SizeOf(_Type);
            }
            catch (Exception ex)
            {
                throw new DescriberException(typeof(BlittableDescriber), _Type, "Size", ex);
            }

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
            return _Size;
        }

        int ITypeDescriber.ToBuffer(object instance, Regulus.Memorys.Buffer buffer, int begin)
        {
            return _ToBuffer(instance, buffer, begin);
        }

        private int _ToBuffer(object instance, Regulus.Memorys.Buffer buffer, int begin)
        {
            int readCount;
            GCHandle pinStructure = GCHandle.Alloc(instance, GCHandleType.Pinned);
            try
            {
                readCount = _Size;
                var bytes = buffer.Bytes;
                Marshal.Copy(pinStructure.AddrOfPinnedObject(), bytes.Array, bytes.Offset + begin, readCount);
            }
            catch (Exception ex)
            {
                throw new DescriberException(typeof(BlittableDescriber), _Type, "ToBuffer", ex);
            }
            finally
            {
                if (pinStructure.IsAllocated)
                {
                    pinStructure.Free();
                }
            }

            return readCount;
        }

        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instnace)
        {

            try
            {
                int size = Marshal.SizeOf(_Type);

                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(buffer, begin, ptr, size);

                instnace = Marshal.PtrToStructure(ptr, _Type);
                Marshal.FreeHGlobal(ptr);
                return size;
            }
            catch (Exception ex)
            {
                throw new DescriberException(typeof(BlittableDescriber), _Type, "ToObject", ex);
            }
        }

    }
}