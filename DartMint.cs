using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartDeserialize
{
    public class DartMint
    {
        public long Value { get; private set; }
        public DartMint(long value)
        {
            Value = value;
        }
    }
}
