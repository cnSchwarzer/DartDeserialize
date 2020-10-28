using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ELFSharp.ELF;
using ELFSharp.ELF.Sections;

namespace DartDeserialize
{
    public class SerializedDartReaderElf
    { 
        private readonly IELF elf;

        public SerializedDartReaderElf(Stream stream)
        {
            elf = ELFReader.Load(stream, true);  
        }

        public (Snapshot, Snapshot, TargetId) GetVMIsolateSnapshotAndTarget()
        {
            TargetId target = TargetId.Error;
            switch (elf.Machine)
            {
                case Machine.ARM:
                    target = TargetId.ARM;
                    break;
                case Machine.AArch64:
                    target = TargetId.ARM64;
                    break;
                case Machine.AMD64:
                    target = TargetId.X64;
                    break;
                case Machine.Intel386:
                    target = TargetId.IA32;
                    break;
            }

            var segments = elf.Sections;

            var dynsym = segments.First((s) => s.Type == SectionType.DynamicSymbolTable);
            byte[] vm_ins = null, vm_data = null, iso_ins = null, iso_data = null;
            foreach (ISymbolEntry entry in (dynsym as ISymbolTable).Entries)
            {
                if (entry.Name.Equals("_kDartVmSnapshotInstructions"))
                {
                    vm_ins = entry.PointedSection.GetContents();
                }
                else if (entry.Name.Equals("_kDartVmSnapshotData"))
                {
                    vm_data = entry.PointedSection.GetContents();
                }
                else if (entry.Name.Equals("_kDartIsolateSnapshotInstructions"))
                {
                    iso_ins = entry.PointedSection.GetContents();
                }
                else if (entry.Name.Equals("_kDartIsolateSnapshotData"))
                {
                    iso_data = entry.PointedSection.GetContents();
                }
            }

            if(vm_ins == null || vm_data == null|| iso_ins == null || iso_data == null)
            {
                Logger.WriteLine("Dart ELF format error.");
                return (null, null, TargetId.Error);
            }

            return (new Snapshot(vm_data, vm_ins), new Snapshot(iso_data, iso_ins), target);
        }
    }
}
