namespace Regulus.Serialization
{
    internal class BufferInfo
    {
        private readonly byte[] _Buffer;        

        public BufferInfo(byte[] buffer)
        {
            _Buffer = buffer;
            
        }

        public byte[] GetBuffer()
        {
            return _Buffer;
        }
    }
}