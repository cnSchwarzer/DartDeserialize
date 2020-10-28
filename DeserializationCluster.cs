using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DartDeserialize
{ 
    public abstract class DeserializationCluster
    {
        public int start_ref_idx = -1;
        public int stop_ref_idx = -1;

        public abstract void ReadAlloc(SnapshotReader d, DartEnv env);
        public abstract void ReadFill(SnapshotReader s, DartEnv env);
        public virtual void PostLoad(SnapshotReader s, DartEnv env)
        {

        }

        public override string ToString()
        {
            return $"{GetType()} start = {start_ref_idx} end = {stop_ref_idx}";
        }
    }

    public class InstanceDeserializationCluster : DeserializationCluster
    {
        public int cid;
        public InstanceDeserializationCluster(int cid)
        {
            this.cid = cid;
        }

        public int next_field_offset_in_words;
        public int instance_size_in_words;
         
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            next_field_offset_in_words = (int)s.Metadata.Read32();
            instance_size_in_words = (int)s.Metadata.Read32();
            for(uint i = 0; i < count; ++i)
            {
                env.AddRef(new byte[DartEnv.WordSize * instance_size_in_words]);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                bool canonical = s.Metadata.Read8() == 1; 
            }
        }

        public override string ToString()
        {
            return base.ToString() + $" cid = {(ClassId)cid}:{cid} next = {next_field_offset_in_words} ins = {instance_size_in_words}";
        }
    }

    public class ClassDeserializationCluster : DeserializationCluster
    {
        public int predefined_start_index;
        public int predefined_stop_index;

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            predefined_start_index = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for(uint i = 0; i < count; ++i)
            {
                int cid = s.Metadata.ReadCid();
                env.AddRef(new DartClass());
            }
            predefined_stop_index = env.NextRefIdx;

            start_ref_idx = env.NextRefIdx;
            count = s.Metadata.ReadUnsigned();
            for(uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartClass());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = predefined_start_index; i < predefined_stop_index; ++i)
            {
                DartClass c = env.Refs[i].Object as DartClass;

                c.name = s.Metadata.ReadRef(env);
                c.user_name = s.Metadata.ReadRef(env);
                c.functions = s.Metadata.ReadRef(env);
                c.functions_hash_table = s.Metadata.ReadRef(env);
                c.fields = s.Metadata.ReadRef(env);
                c.offset_in_words_to_field = s.Metadata.ReadRef(env);
                c.interfaces = s.Metadata.ReadRef(env);
                c.script = s.Metadata.ReadRef(env);
                c.library = s.Metadata.ReadRef(env);
                c.type_parameters = s.Metadata.ReadRef(env);
                c.super_type = s.Metadata.ReadRef(env);
                c.signature_function = s.Metadata.ReadRef(env);
                c.constants = s.Metadata.ReadRef(env);
                c.declaration_type = s.Metadata.ReadRef(env);
                c.invocation_dispatcher_cache = s.Metadata.ReadRef(env);
                c.allocation_stub = s.Metadata.ReadRef(env);

                c.id = s.Metadata.ReadCid(); 

                if (!ClassIdHelper.IsInternalVMdefinedClassId(c.id))
                {
                    c.host_instance_size_in_words = (int)s.Metadata.Read32();
                    c.host_next_field_offset_in_words = (int)s.Metadata.Read32();
                }
                else
                {
                    s.Metadata.Read32();
                    s.Metadata.Read32();
                }

                c.host_type_arguments_field_offset_in_words = (int)s.Metadata.Read32();
                c.num_type_arguments = s.Metadata.Read16();
                c.num_native_fields = s.Metadata.Read16();

                c.token_pos = (int)s.Metadata.Read32();
                c.end_token_pos = (int)s.Metadata.Read32();
                c.state_bits = (int)s.Metadata.Read32();

                s.Metadata.ReadUnsigned();
            }
             
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartClass c = env.Refs[i].Object as DartClass;

                c.name = s.Metadata.ReadRef(env);
                c.user_name = s.Metadata.ReadRef(env);
                c.functions = s.Metadata.ReadRef(env);
                c.functions_hash_table = s.Metadata.ReadRef(env);
                c.fields = s.Metadata.ReadRef(env);
                c.offset_in_words_to_field = s.Metadata.ReadRef(env);
                c.interfaces = s.Metadata.ReadRef(env);
                c.script = s.Metadata.ReadRef(env);
                c.library = s.Metadata.ReadRef(env);
                c.type_parameters = s.Metadata.ReadRef(env);
                c.super_type = s.Metadata.ReadRef(env);
                c.signature_function = s.Metadata.ReadRef(env);
                c.constants = s.Metadata.ReadRef(env);
                c.declaration_type = s.Metadata.ReadRef(env);
                c.invocation_dispatcher_cache = s.Metadata.ReadRef(env);
                c.allocation_stub = s.Metadata.ReadRef(env);

                c.id = s.Metadata.ReadCid();
                
                c.host_instance_size_in_words = (int)s.Metadata.Read32();
                c.host_next_field_offset_in_words = (int)s.Metadata.Read32(); 
                c.host_type_arguments_field_offset_in_words = (int)s.Metadata.Read32();

                c.num_type_arguments = s.Metadata.Read16();
                c.num_native_fields = s.Metadata.Read16();

                c.token_pos = (int)s.Metadata.Read32();
                c.end_token_pos = (int)s.Metadata.Read32();
                c.state_bits = (int)s.Metadata.Read32();


                if (!ClassIdHelper.IsTopLevelCid(c.id))
                {
                    s.Metadata.ReadUnsigned();
                }
            }
        }
    }

    public class PatchClassDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for(uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartPatchClass());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartPatchClass c = env.Refs[i].Object as DartPatchClass;
                c.patched_class = s.Metadata.ReadRef(env);
                c.origin_class = s.Metadata.ReadRef(env);
                c.script = s.Metadata.ReadRef(env);
            }
        }
    }

    public class FunctionDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartFunction());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartFunction f = env.Refs[i].Object as DartFunction;

                f.name = s.Metadata.ReadRef(env);
                f.nameString = Utils.StringFromROData(env, f.name.Object as byte[]);

                if (f.nameString.Contains("checkFlag"))
                {
                     Debugger.Break();
                }

                f.owner = s.Metadata.ReadRef(env);
                f.result_type = s.Metadata.ReadRef(env);
                f.parameter_types = s.Metadata.ReadRef(env);
                f.parameter_names = s.Metadata.ReadRef(env);
                f.type_parameters = s.Metadata.ReadRef(env);
                f.data = s.Metadata.ReadRef(env);

                f.code = s.Metadata.ReadRef(env);
                f.packed_fields = s.Metadata.Read32();
                f.kind_tag = s.Metadata.Read32();
            }
        }
    }

    public class ClosureDataDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartClosureData());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartClosureData d = env.Refs[i].Object as DartClosureData;
                d.context_scope = null;
                d.parent_function = s.Metadata.ReadRef(env);
                d.signature_type = s.Metadata.ReadRef(env);
                d.closure = s.Metadata.ReadRef(env);
            }
        }
    }

    public class SignatureDataDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartSignatureData());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartSignatureData d = env.Refs[i].Object as DartSignatureData;
                d.parent_function = s.Metadata.ReadRef(env);
                d.signature_type = s.Metadata.ReadRef(env);
            }
        }
    }

    public class FieldDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartField());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartField d = env.Refs[i].Object as DartField;
                d.name = s.Metadata.ReadRef(env);
                d.owner = s.Metadata.ReadRef(env);
                d.type = s.Metadata.ReadRef(env);
                d.initializer_function = s.Metadata.ReadRef(env); 

                d.kind_bits = s.Metadata.Read16();
                DartObject value_or_offset = s.Metadata.ReadRef(env);
                if((d.kind_bits & 0b10) == 0b10)
                {
                    ulong field_id = s.Metadata.ReadUnsigned();
                    d.host_offset_or_field_id = new DartSmi((long)field_id);
                }
                else
                {
                    d.host_offset_or_field_id = value_or_offset.Object as DartSmi;
                }
            }
        }
    }

    public class ScriptDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartScript());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartScript d = env.Refs[i].Object as DartScript;
                d.url = s.Metadata.ReadRef(env);

                d.line_offset = s.Metadata.Read32();
                d.col_offset = s.Metadata.Read32();
                d.flags = s.Metadata.Read8();
                d.kernel_script_index = s.Metadata.Read32(); 
            }
        }
    }

    public class LibraryDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartLibrary());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartLibrary d = env.Refs[i].Object as DartLibrary;
                d.name = s.Metadata.ReadRef(env);
                d.url = s.Metadata.ReadRef(env);
                d.private_key = s.Metadata.ReadRef(env);
                d.dictionary = s.Metadata.ReadRef(env);
                d.metadata = s.Metadata.ReadRef(env);
                d.toplevel_class = s.Metadata.ReadRef(env);
                d.used_scripts = s.Metadata.ReadRef(env);
                d.loading_unit = s.Metadata.ReadRef(env);
                d.imports = s.Metadata.ReadRef(env);
                d.exports = s.Metadata.ReadRef(env);

                d.index = s.Metadata.Read32();
                d.num_imports = s.Metadata.Read16();
                d.load_state = s.Metadata.Read8();
                d.flags = s.Metadata.Read8();
            }
        }
    }

    public class CodeDeserializationCluster : DeserializationCluster
    {
        public int deferred_start_idx, deferred_stop_idx;

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartCode());
            }
            stop_ref_idx = env.NextRefIdx;

            deferred_start_idx = env.NextRefIdx;
            count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(new DartCode());
            }
            deferred_stop_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                ReadFill(s, env, i, false);
            }

            for (int i = deferred_start_idx; i < deferred_stop_idx; ++i)
            {
                ReadFill(s, env, i, true);
            }
        }

        public void ReadFill(SnapshotReader s, DartEnv env, int id, bool deferred)
        {
            DartCode code = env.Refs[id].Object as DartCode;
            s.Metadata.ReadInstructions(code, deferred);
            code.owner = s.Metadata.ReadRef(env);
            code.exception_handlers = s.Metadata.ReadRef(env);
            code.pc_descriptors = s.Metadata.ReadRef(env);
            code.catch_entry = s.Metadata.ReadRef(env);
            code.compressed_stackmaps = s.Metadata.ReadRef(env);
            code.inlined_id_to_function = s.Metadata.ReadRef(env);
            code.code_source_map = s.Metadata.ReadRef(env);
            code.state_bits = s.Metadata.Read32();
        }
    }

    public class ObjectPoolDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class RODataDeserializationCluster: DeserializationCluster
    {
        public ClassId cid;
        
        public RODataDeserializationCluster(ClassId cid)
        {
            this.cid = cid;
        }

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            ulong running_offset = s.Metadata.ReadUnsigned() << DartEnv.ObjectAlignmentLog2;
            s.Data.BaseStream.Position = (long)running_offset;
            for (uint i = 1; i < count; ++i)
            {
                ulong previous_size = s.Metadata.ReadUnsigned() << DartEnv.ObjectAlignmentLog2; 
                env.AddRef(s.Data.ReadBytes((int)previous_size));
                running_offset += previous_size;
            } 
            env.AddRef(s.Data.ReadBytes((int)s.Data.BaseStream.Length - (int)running_offset));
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        { 

        }

        public override string ToString()
        {
            return base.ToString() + $" cid = {cid}:{(int)cid}";
        }
    }

    public class ExceptionHandlersDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class UnlinkedCallDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class MegamorphicCacheDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class SubtypeTestCacheDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class LoadingUnitDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class TypeArgumentsDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class TypeDeserializationCluster : DeserializationCluster
    {
        public int canonical_start_idx, canonical_stop_idx;

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            canonical_start_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            { 
                env.AddRef(null);
            }
            canonical_stop_idx = env.NextRefIdx;

            start_ref_idx = env.NextRefIdx;
            count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            { 
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class TypeRefDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class TypeParameterDeserializationCluster : DeserializationCluster
    {
        public int canonical_start_idx, canonical_stop_idx;

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            canonical_start_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            canonical_stop_idx = env.NextRefIdx;

            start_ref_idx = env.NextRefIdx;
            count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class ClosureDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class MintDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                bool is_canonical = (s.Metadata.Read8() == 1);
                long value = (long)s.Metadata.Read64();
                if (DartSmi.IsValid(value))
                {
                    env.AddRef(new DartSmi(value));
                }
                else
                {
                    env.AddRef(new DartMint(value));
                }
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
        }
    }

    public class DoubleDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class GrowableObjectArrayDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }
    }

    public class ArrayDeserializationCluster : DeserializationCluster
    {
        public ClassId cid;

        public ArrayDeserializationCluster(ClassId cid)
        {
            this.cid = cid;
        }

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(new DartArray((int)length));
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartArray array = env.Refs[i].Object as DartArray;
                ulong length = s.Metadata.ReadUnsigned();
                bool is_canonical = s.Metadata.Read8() == 1;
                array.TypeArgument = s.Metadata.ReadRef(env);
                array.Length = new DartSmi((long)length);
                for(uint k = 0; k < length; ++k)
                {
                    array.Data[k] = s.Metadata.ReadRef(env);
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + $" cid = {cid}:{(int)cid}";
        }
    }

    public class TypedDataDeserializationCluster : DeserializationCluster
    {
        public ClassId cid;

        public TypedDataDeserializationCluster(ClassId cid)
        {
            this.cid = cid;
        }

        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(null);
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {

            }
        }

        public override string ToString()
        {
            return base.ToString() + $" cid = {cid}:{(int)cid}";
        }
    }

    public class OneByteStringDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(new DartString());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartString str = env.Refs[i].Object as DartString;
                ulong length = s.Metadata.ReadUnsigned();
                bool is_canonical = s.Metadata.Read8() == 1;
                str.Length = new DartSmi((long)length);
                str.Hash = s.Metadata.Read32();
                for (uint k = 0; k < length; ++k)
                {
                    str.String += (char)s.Metadata.Read8();
                }
            }
        }
    }

    public class TwoByteStringDeserializationCluster : DeserializationCluster
    {
        public override void ReadAlloc(SnapshotReader s, DartEnv env)
        {
            start_ref_idx = env.NextRefIdx;
            ulong count = s.Metadata.ReadUnsigned();
            for (uint i = 0; i < count; ++i)
            {
                ulong length = s.Metadata.ReadUnsigned();
                env.AddRef(new DartString());
            }
            stop_ref_idx = env.NextRefIdx;
        }

        public override void ReadFill(SnapshotReader s, DartEnv env)
        {
            for (int i = start_ref_idx; i < stop_ref_idx; ++i)
            {
                DartString str = env.Refs[i].Object as DartString;
                ulong length = s.Metadata.ReadUnsigned();
                bool is_canonical = s.Metadata.Read8() == 1;
                str.Length = new DartSmi((long)length);
                str.Hash = s.Metadata.Read32();
                str.String = Encoding.UTF8.GetString(s.Metadata.ReadBytes((int)length * 2));
            }
        }
    }
}
