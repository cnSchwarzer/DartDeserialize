using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartScript
    {
        public DartObject url;

        public uint line_offset, col_offset;
        public byte flags;
        public uint kernel_script_index;
        public uint load_timestamp = 0;
    }
}
