using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartLibrary
    {
        public DartObject name;
        public DartObject url;
        public DartObject private_key;
        public DartObject dictionary;              // Top-level names in this library.
        public DartObject metadata;  // Metadata on classes, methods etc.
        public DartObject toplevel_class;          // Class containing top-level elements.
        public DartObject used_scripts;
        public DartObject loading_unit;
        public DartObject imports;  // List of Namespaces imported without prefix.
        public DartObject exports;

        public uint index;
        public ushort num_imports;
        public byte load_state;
        public byte flags;
    }
}
