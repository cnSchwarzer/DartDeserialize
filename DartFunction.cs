using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartFunction
    {
        public DartObject name;
        public DartObject owner;
        public DartObject result_type;
        public DartObject parameter_types;
        public DartObject parameter_names;
        public DartObject type_parameters;
        public DartObject data;

        public DartObject code;
        public uint packed_fields;
        public uint kind_tag;

        public string nameString;
    }
}
