using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartDeserialize
{
    public class DartString
    {
        public string String { get; set; } = string.Empty;
        public DartSmi Length { get; set; }
        public uint Hash { get; set; } 
    }
}
