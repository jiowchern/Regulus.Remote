using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Regulus.Remote.Tools.Protocol.Sources
{
    public static class MiscExtensions
    {
        public static byte[] ToMd5(this string source)
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(source));
        }

        public static string ToMd5String(this System.Collections.Generic.IEnumerable<byte> source)
        {
            return System.BitConverter.ToString(source.ToArray());
        }
        public static IEnumerable<int> GetSeries(this int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return i;
            }
        }
    }
}