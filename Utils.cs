using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DartDeserialize
{
    public static class Utils
    {
        public const long kLongOne = 1;

        public static bool IsInt(int N, long value)
        {
            long limit = kLongOne << (N - 1);
            return (-limit <= value) && (value < limit);
        }
        public static ulong RoundDown(ulong value, long n)
        {
            return (value & (ulong)(-n));
        }
        public static ulong RoundUp(ulong value, long n)
        {
            return RoundDown(value + (ulong)n - 1, n);
        }
        public static string StringFromROData(DartEnv env, byte[] bytes)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(bytes));

            if (env.Is64Bit)
                br.ReadInt64();
            else
                br.ReadInt32();
            long length = (env.Is64Bit ? br.ReadInt64() : br.ReadInt32()) / 2;

            if (!env.Is64Bit) 
                br.ReadInt32();

            string str = Encoding.UTF8.GetString(br.ReadBytes((int)length));

            br.Close();

            return str;
        }
    }
}
