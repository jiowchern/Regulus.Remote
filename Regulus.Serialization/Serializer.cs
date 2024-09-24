using System;

namespace Regulus.Serialization
{


    public class Serializer 
    {
        private readonly DescriberProvider _Provider;

        static Regulus.Memorys.Pool _Create()
        {
            return new Memorys.Pool(
                                   new Memorys.ChunkSetting[]
                                   {
                        //new Memorys.ChunkSetting(1, 1024),
                        //new Memorys.ChunkSetting(2, 1024),
                        new Memorys.ChunkSetting(4, 1024),
                        new Memorys.ChunkSetting(8, 1024),
                        new Memorys.ChunkSetting(16, 1024),
                        new Memorys.ChunkSetting(32, 1024),
                        new Memorys.ChunkSetting(64, 1024),
                        new Memorys.ChunkSetting(128, 1024),
                        new Memorys.ChunkSetting(256, 1024),
                        new Memorys.ChunkSetting(512, 1024),
                        new Memorys.ChunkSetting(1024, 128),
                        new Memorys.ChunkSetting(2048, 128) }                        
                                   
                                                  );
        }
        internal readonly static Regulus.Memorys.Pool Shared = _Create();

        public Serializer(DescriberProvider provider)
        {
            _Provider = provider;
        }

        public Regulus.Memorys.Buffer ObjectToBuffer(object instance)
        {

            try
            {
                if (instance == null)
                {
                    return _NullBuffer();
                }


                Type type = instance.GetType();
                ITypeDescriber describer = _Provider.TypeDescriberFinders.Get(type);

                int idCount = _Provider.KeyDescriber.GetByteCount(type);
                int bufferCount = describer.GetByteCount(instance);
                var buffer = Shared.Alloc(idCount + bufferCount);
                var bytes = buffer.Bytes;
                int readCount = _Provider.KeyDescriber.ToBuffer(type, buffer, 0);
                describer.ToBuffer(instance, buffer, readCount);
                return buffer;
            }
            catch (DescriberException ex)
            {

                if (instance != null)
                {
                    throw new SystemException(string.Format("ObjectToBuffer {0}", instance.GetType()), ex);
                }
                else
                {
                    throw new SystemException(string.Format("ObjectToBuffer null"), ex);
                }
            }

        }

        private Memorys.Buffer _NullBuffer()
        {
            int idCount = Varint.GetByteCount(0);
            var buffer = Shared.Alloc(idCount);
            var bytes = buffer.Bytes;
            Varint.NumberToBuffer(bytes.Array, bytes.Offset, 0);
            return buffer;
        }

        public object BufferToObject(Memorys.Buffer buffer)
        {
            Type id = null;
            try
            {

                int readIdCount = _Provider.KeyDescriber.ToObject(buffer, 0, out id);
                if (id == null)
                    return null;

                ITypeDescriber describer = _Provider.TypeDescriberFinders.Get(id);
                object instance;
                describer.ToObject(buffer, readIdCount, out instance);
                return instance;
            }
            catch (DescriberException ex)
            {
                ITypeDescriber describer = _Provider.TypeDescriberFinders.Get(id);
                if (describer != null)
                    throw new SystemException(string.Format("BufferToObject {0}:{1}", id, describer.Type.FullName), ex);
                else
                {
                    throw new SystemException(string.Format("BufferToObject {0}:unkown", id), ex);
                }
            }

        }



      

        public bool TryBufferToObject<T>(Regulus.Memorys.Buffer buffer, out T pkg)
        {

            pkg = default(T);
            try
            {
                object instance = BufferToObject(buffer);
                pkg = (T)instance;
                return true;
            }
            catch (Exception e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(e.ToString());
            }

            return false;
        }

        public bool TryBufferToObject(Regulus.Memorys.Buffer buffer, out object pkg)
        {

            pkg = null;
            try
            {
                object instance = BufferToObject(buffer);
                pkg = instance;
                return true;
            }
            catch (Exception e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(e.ToString());
            }

            return false;
        }
       
    }


}



