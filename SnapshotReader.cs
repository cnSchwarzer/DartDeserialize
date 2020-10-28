using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartDeserialize
{
    public abstract class SnapshotReader
    {
        public Snapshot Snapshot;

        public MetadataStreamReader Metadata;
        public BinaryReader Data;
        public BinaryReader Instruction;

        public SnapshotReader(Snapshot snapshot)
        {
            Snapshot = snapshot;
            Metadata = snapshot.GetSnapshotMetadataStream();
            Data = new BinaryReader(snapshot.GetSnapshotDataStream());
            Instruction = new BinaryReader(snapshot.GetSnapshotInstructionStream());
        }

        public abstract void ResolveSnapshot(DartEnv env);
         
        protected void ResolveVersionAndFeature()
        {
            Metadata.Skip(20);
            string version = Encoding.ASCII.GetString(Metadata.ReadBytes(32));
            string features = string.Empty;
            byte b = Metadata.Read8();
            while (b != 0)
            {
                features += (char)b;
                b = Metadata.Read8();
            }
        }
    }
}
