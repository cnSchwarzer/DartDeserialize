using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartField
    {
        public DartObject name;
        public DartObject owner;  // Class or patch class or mixin class
                                   // where this field is defined or original field.
        public DartObject type;
        public DartObject initializer_function; 

        public uint kind_bits;
        public DartSmi host_offset_or_field_id;
    }
}
