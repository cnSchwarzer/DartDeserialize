using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartDeserialize
{
    public class SnapshotReaderVM : SnapshotReader
    { 
        public SnapshotReaderVM(Snapshot snapshot) : base(snapshot)
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

            AddBaseObjects(env);

            DeserializationCluster[] clusters = new DeserializationCluster[num_clusters];

            Logger.WriteLine("VM ReadAlloc");

            for (uint i = 0; i < num_clusters; ++i)
            {
                clusters[i] = Metadata.ReadCluster();
                clusters[i].ReadAlloc(this, env);
                Logger.WriteLine(clusters[i].ToString());
            }

            Logger.WriteLine("VM ReadFill");

            for (uint i = 0; i < num_clusters; ++i)
            { 
                clusters[i].ReadFill(this, env); 
            }

            DartObject symbol_table = Metadata.ReadRef(env);
            for (int i = 0; i<DartEnv.NumStubEntries; ++i)
            {
                DartObject code = Metadata.ReadRef(env);
            }

            Logger.WriteLine("VM PostLoad");

            for (uint i = 0; i < num_clusters; ++i)
            {
                clusters[i].PostLoad(this, env);
            }
        }

        private void AddBaseObjects(DartEnv env)
        {
            env.AddRef("Object::null()");
            env.AddRef("Object::sentinel().raw()");
            env.AddRef("Object::transition_sentinel().raw()");
            env.AddRef("Object::empty_array().raw()");
            env.AddRef("Object::zero_array().raw()");
            env.AddRef("Object::dynamic_type().raw()");
            env.AddRef("Object::void_type().raw()");
            env.AddRef("Object::empty_type_arguments().raw()");
            env.AddRef("Bool::True().raw()");
            env.AddRef("Bool::False().raw()");
            env.AddRef("Object::extractor_parameter_types().raw()");
            env.AddRef("Object::extractor_parameter_names().raw()");
            env.AddRef("Object::empty_context_scope().raw()");
            env.AddRef("Object::empty_descriptors().raw()");
            env.AddRef("Object::empty_var_descriptors().raw()");
            env.AddRef("Object::empty_exception_handlers().raw()");
            env.AddRef("Object::implicit_getter_bytecode().raw()");
            env.AddRef("Object::implicit_setter_bytecode().raw()");
            env.AddRef("Object::implicit_static_getter_bytecode().raw()");
            env.AddRef("Object::method_extractor_bytecode().raw()");
            env.AddRef("Object::invoke_closure_bytecode().raw()");
            env.AddRef("Object::invoke_field_bytecode().raw()");
            env.AddRef("Object::nsm_dispatcher_bytecode().raw()");
            env.AddRef("Object::dynamic_invocation_forwarder_bytecode().raw()");

            for (int i = 0; i < DartEnv.CachedDescriptorCount; ++i)
                env.AddRef($"cached_args_descriptors[{i}]");

            for (int i = 0; i < DartEnv.CachedICDataArrayCount; ++i)
                env.AddRef($"cached_icdata_arrays[{i}]"); 

            env.AddRef("SubtypeTestCache::cached_array_");

            for (int i = (int)ClassId.kClassCid; i <= (int)ClassId.kUnwindErrorCid; ++i)
            {
                if (i != (int)ClassId.kErrorCid && i != (int)ClassId.kCallSiteDataCid)
                {
                    env.AddRef((ClassId)i);
                }
            }

            env.AddRef(ClassId.kDynamicCid);
            env.AddRef(ClassId.kVoidCid);
        } 
    }
}
