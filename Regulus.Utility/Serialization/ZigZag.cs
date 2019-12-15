namespace Regulus.Serialization
{
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
}