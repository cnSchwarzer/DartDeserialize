using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartArray
    {
        public DartArray(int length)
        {
            Data = new DartObject[length];
        }

        public DartObject TypeArgument { get; set; }
        public DartSmi Length { get; set; }
        public DartObject[] Data { get; set; }
    }
}
