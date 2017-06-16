namespace Regulus.Serialization
{
    public class Varint
    {

        public static int NumberToBuffer(byte[] buffer, int offset, int value)
        {
            return Varint.NumberToBuffer(buffer , offset , (ulong)value);
        }
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

        public static int GetByteCount(int value)
        { 
            return Varint.GetByteCount((ulong)value);
        }
        public static int GetByteCount(ulong value)
        {
            int i;
            for (i = 0; i < 9 && value >= 0x80; i++, value >>= 7) { }
            return i + 1;
        }

        public static int BufferToNumber(byte[] buffer, int offset, out int value)
        {
            ulong val;
            var count = BufferToNumber(buffer, offset, out val);
            value = (int)val;

            return count;
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

        public static int GetMaxInt32Length()
        {
            return GetByteCount(-1);
        }
    }
}