using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DartDeserialize
{
    public class MetadataStreamReader : LEB128StreamReader
    {
        public MetadataStreamReader(Stream stream) : base(stream)
        {

        }

        private long previous_text_offset = 0;

        public int ReadCid()
        {
            return (int)Read32();
        }

        public DartObject ReadRef(DartEnv env)
        {
            return env.Refs[(int)ReadUnsigned()];
        }

        public void ReadInstructions(DartCode code, bool deferred)
        {
            //It seems in bare instruction mode
            if (!deferred)
            {
                ulong add_offset = ReadUnsigned();
                previous_text_offset += (long)add_offset;

                long payload_start = previous_text_offset;
                ulong payload_info = ReadUnsigned();
                ulong unchecked_offset = payload_info >> 1;

                bool has_monomorphic_entrypoint = (payload_info & 0x1) == 1;
                int entry_offset = has_monomorphic_entrypoint ? DartEnv.PolymorphicEntryOffsetAOT : 0;
                int monomorphic_entry_offset = has_monomorphic_entrypoint ? DartEnv.MonomorphicEntryOffsetAOT : 0;

                long entry_point = payload_start + entry_offset;
                long monomorphic_entry_point = payload_start + monomorphic_entry_offset;

                code.entry_point = (ulong)entry_point;
                code.unchecked_entry_point = (ulong)entry_point + unchecked_offset;
                code.monomorphic_entry_point = (ulong)monomorphic_entry_point;
                code.monomorphic_unchecked_entry_point = (ulong)monomorphic_entry_point + unchecked_offset;
            }
            else
            {

            }
        }

        public DeserializationCluster ReadCluster()
        {
            int cid = ReadCid();
            if (cid >= (int)ClassId.kNumPredefinedCids || cid == (int)ClassId.kInstanceCid)
            {
                return new InstanceDeserializationCluster(cid);
            }
            if (ClassIdHelper.IsTypedDataViewClassId(cid))
            {
                return null;
            }
            if (ClassIdHelper.IsExternalTypedDataClassId(cid))
            {
                return null;
            }
            if (ClassIdHelper.IsTypedDataClassId(cid))
            {
                return new TypedDataDeserializationCluster((ClassId)cid);
            }

            if (true) //Of course we are AOT
            {
                switch ((ClassId)cid)
                {
                    case ClassId.kPcDescriptorsCid:
                    case ClassId.kCodeSourceMapCid:
                    case ClassId.kCompressedStackMapsCid:
                    case ClassId.kOneByteStringCid:
                    case ClassId.kTwoByteStringCid:
                        return new RODataDeserializationCluster((ClassId)cid);
                }
            }

            switch ((ClassId)cid)
            {
                case ClassId.kClassCid:
                    return new ClassDeserializationCluster();
                case ClassId.kTypeArgumentsCid:
                    return new TypeArgumentsDeserializationCluster();
                case ClassId.kPatchClassCid:
                    return new PatchClassDeserializationCluster();
                case ClassId.kFunctionCid:
                    return new FunctionDeserializationCluster();
                case ClassId.kClosureDataCid:
                    return new ClosureDataDeserializationCluster();
                case ClassId.kSignatureDataCid:
                    return new SignatureDataDeserializationCluster();
                //case ClassId.kRedirectionDataCid:
                //    return new RedirectionDataDeserializationCluster();
                //case ClassId.kFfiTrampolineDataCid:
                //    return new FfiTrampolineDataDeserializationCluster();
                case ClassId.kFieldCid:
                    return new FieldDeserializationCluster();
                case ClassId.kScriptCid:
                    return new ScriptDeserializationCluster();
                case ClassId.kLibraryCid:
                    return new LibraryDeserializationCluster();
                //case ClassId.kNamespaceCid:
                //    return new NamespaceDeserializationCluster(); 
                case ClassId.kCodeCid:
                    return new CodeDeserializationCluster();
                case ClassId.kObjectPoolCid:
                    return new ObjectPoolDeserializationCluster();
                //case ClassId.kPcDescriptorsCid:
                //    return new PcDescriptorsDeserializationCluster();
                case ClassId.kExceptionHandlersCid:
                    return new ExceptionHandlersDeserializationCluster();
                //case ClassId.kContextCid:
                //    return new ContextDeserializationCluster();
                //case ClassId.kContextScopeCid:
                //    return new ContextScopeDeserializationCluster();
                //case ClassId.kParameterTypeCheckCid:
                //    return new ParameterTypeCheckDeserializationCluster();
                case ClassId.kUnlinkedCallCid:
                    return new UnlinkedCallDeserializationCluster();
                //case ClassId.kICDataCid:
                //    return new ICDataDeserializationCluster();
                case ClassId.kMegamorphicCacheCid:
                    return new MegamorphicCacheDeserializationCluster();
                case ClassId.kSubtypeTestCacheCid:
                    return new SubtypeTestCacheDeserializationCluster();
                case ClassId.kLoadingUnitCid:
                    return new LoadingUnitDeserializationCluster();
                //case ClassId.kLanguageErrorCid:
                //    return new LanguageErrorDeserializationCluster();
                //case ClassId.kUnhandledExceptionCid:
                //    return new UnhandledExceptionDeserializationCluster();
                //case ClassId.kLibraryPrefixCid:
                //    return new LibraryPrefixDeserializationCluster();
                case ClassId.kTypeCid:
                    return new TypeDeserializationCluster();
                case ClassId.kTypeRefCid:
                    return new TypeRefDeserializationCluster();
                case ClassId.kTypeParameterCid:
                    return new TypeParameterDeserializationCluster();
                case ClassId.kClosureCid:
                    return new ClosureDeserializationCluster();
                case ClassId.kMintCid:
                    return new MintDeserializationCluster();
                case ClassId.kDoubleCid:
                    return new DoubleDeserializationCluster();
                case ClassId.kGrowableObjectArrayCid:
                    return new GrowableObjectArrayDeserializationCluster();
                //case ClassId.kStackTraceCid:
                //    return new StackTraceDeserializationCluster();
                //case ClassId.kRegExpCid:
                //    return new RegExpDeserializationCluster();
                //case ClassId.kWeakPropertyCid:
                //    return new WeakPropertyDeserializationCluster();
                //case ClassId.kLinkedHashMapCid:
                //    return new LinkedHashMapDeserializationCluster();
                case ClassId.kArrayCid:
                    return new ArrayDeserializationCluster(ClassId.kArrayCid);
                case ClassId.kImmutableArrayCid:
                    return new ArrayDeserializationCluster(ClassId.kImmutableArrayCid);
                case ClassId.kOneByteStringCid:
                    return new OneByteStringDeserializationCluster();
                case ClassId.kTwoByteStringCid:
                    return new TwoByteStringDeserializationCluster();
                //case ClassId.kWeakSerializationReferenceCid:
                //    return new WeakSerializationReferenceDeserializationCluster();
                default:
                    break;
            }

            throw new InvalidDataException($"CID : {cid} : {(ClassId)cid} not implemented");  
        }
    }
}
