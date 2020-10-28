using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartObject
    {
        public int Id { get; private set; }
        public object Object { get; set; } 

        public DartObject(object obj, int id) : this(id)
        {
            Object = obj;
        }

        public DartObject(int id)
        {
            Id = id;
        }
    }
}
