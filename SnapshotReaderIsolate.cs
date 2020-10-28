using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartDeserialize
{
    public class SnapshotReaderIsolate : SnapshotReader
    {
        public SnapshotReaderIsolate(Snapshot snapshot) : base(snapshot)
        {

        }

        public override void ResolveSnapshot(DartEnv env)
        {
            ResolveVersionAndFeature();
            ResolveMetadata(env);
        }

        private void ResolveMetadata(DartEnv env)
        {
            ulong num_base_objects = Metadata.ReadUnsigned();
            ulong num_objects = Metadata.ReadUnsigned();
            ulong num_clusters = Metadata.ReadUnsigned();
            ulong field_table_len = Metadata.ReadUnsigned();

            if (num_base_objects + 1 != (ulong)env.NextRefIdx)
                throw new InvalidDataException("Base object count != env.NextRefIdx");

            DeserializationCluster[] clusters = new DeserializationCluster[num_clusters];

            Logger.WriteLine("Isolate ReadAlloc");

            for (uint i = 0; i < num_clusters; ++i)
            {
                clusters[i] = Metadata.ReadCluster();
                clusters[i].ReadAlloc(this, env);
                Logger.WriteLine(clusters[i].ToString());
            }

            Logger.WriteLine("Isolate ReadFill");

            for (uint i = 0; i < num_clusters; ++i)
            {
                clusters[i].ReadFill(this, env);
            }

            DartObject symbol_table = Metadata.ReadRef(env);
            for (int i = 0; i < DartEnv.NumStubEntries; ++i)
            {
                DartObject code = Metadata.ReadRef(env);
            }

            Logger.WriteLine("Isolate PostLoad");

            for (uint i = 0; i < num_clusters; ++i)
            {
                clusters[i].PostLoad(this, env);
            }
        } 
    }
}
