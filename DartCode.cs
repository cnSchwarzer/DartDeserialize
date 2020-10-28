using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartDeserialize
{
    public class DartCode
    {
        public ulong entry_point = 0;
        public ulong unchecked_entry_point = 0;

        //virtual overload?
        public ulong monomorphic_entry_point = 0;
        public ulong monomorphic_unchecked_entry_point = 0;

        public DartObject owner = null;
        public DartObject exception_handlers = null;
        public DartObject pc_descriptors = null;
        public DartObject catch_entry = null;
        public DartObject compressed_stackmaps = null;
        public DartObject inlined_id_to_function = null;
        public DartObject code_source_map = null;
        public uint state_bits = 0;
    }
}
