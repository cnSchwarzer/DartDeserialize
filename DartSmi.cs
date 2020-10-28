using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartDeserialize
{
    public class DartSmi
    {
        public const int kSmiBits = 62;

        public long Value { get; private set; }

        public static bool IsValid(long value)
        {
            return Utils.IsInt(kSmiBits + 1, value);
        }

        public DartSmi(long value)
        {
            Value = value;
        }
    }
}
