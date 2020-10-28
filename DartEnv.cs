using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DartDeserialize
{
    public enum TargetId
    {
        Error,
        IA32,
        X64,
        ARM,
        ARM64
    }

    public class DartEnv
    {
        public static int MaxObjectAlignment { get; private set; } = 16;
        public static int WordSize { get; private set; } = 0;
        public static int PolymorphicEntryOffsetAOT { get; private set; } = 0; 
        public static int MonomorphicEntryOffsetAOT { get; private set; } = 0;
        public static int ObjectAlignmentLog2 { get; private set; } = 0;
        public static int NumStubEntries { get; private set; } = 95;
        public static int CachedDescriptorCount { get; private set; } = 32;
        public static int CachedICDataArrayCount { get; private set; } = 4;

        public TargetId Target { get; private set; }
        public bool Is64Bit
        {
            get
            {
                return Is64BitTarget(Target);
            }
        }

        public List<DartObject> Refs { get; set; } = new List<DartObject>();

        public int NextRefIdx
        {
            get
            {
                return Refs.Count;
            }
        }

        public DartEnv(TargetId target)
        {
            Refs.Add(new DartObject(Refs.Count));
            InitializedTargetSpecificParameters(target);
            Target = target;
        }

        public void AddRef(object obj)
        {
            Refs.Add(new DartObject(obj, Refs.Count));
        }  

        private static bool Is64BitTarget(TargetId target)
        {
            return target == TargetId.ARM64 || target == TargetId.X64;
        }

        private void InitializedTargetSpecificParameters(TargetId target)
        {
            switch (target)
            {
                case TargetId.IA32:
                    PolymorphicEntryOffsetAOT = 0;
                    MonomorphicEntryOffsetAOT = 0;
                    break;
                case TargetId.X64:
                    PolymorphicEntryOffsetAOT = 8;
                    MonomorphicEntryOffsetAOT = 22;
                    break;
                case TargetId.ARM:
                    PolymorphicEntryOffsetAOT = 0;
                    MonomorphicEntryOffsetAOT = 12;
                    break;
                case TargetId.ARM64:
                    PolymorphicEntryOffsetAOT = 8;
                    MonomorphicEntryOffsetAOT = 20;
                    break; 
            }
            if (Is64BitTarget(target))
            {
                WordSize = 8;
                ObjectAlignmentLog2 = 4;
            }
            else
            {
                WordSize = 4;
                ObjectAlignmentLog2 = 3;
            }
        }
    }
}
