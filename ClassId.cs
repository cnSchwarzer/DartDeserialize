using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartDeserialize
{ 
    public enum ClassId
    {
        kIllegalCid = 0,

        kFreeListElement,
        kForwardingCorpse,

        kObjectCid, kClassCid, kPatchClassCid, kFunctionCid, kClosureDataCid, kSignatureDataCid, kRedirectionDataCid, kFfiTrampolineDataCid, kFieldCid, kScriptCid, kLibraryCid, kNamespaceCid, kKernelProgramInfoCid, kCodeCid, kBytecodeCid, kInstructionsCid, kInstructionsSectionCid, kObjectPoolCid, kPcDescriptorsCid, kCodeSourceMapCid, kCompressedStackMapsCid, kLocalVarDescriptorsCid, kExceptionHandlersCid, kContextCid, kContextScopeCid, kParameterTypeCheckCid, kSingleTargetCacheCid, kUnlinkedCallCid, kMonomorphicSmiableCallCid, kCallSiteDataCid, kICDataCid, kMegamorphicCacheCid, kSubtypeTestCacheCid, kLoadingUnitCid, kErrorCid, kApiErrorCid, kLanguageErrorCid, kUnhandledExceptionCid, kUnwindErrorCid, kInstanceCid, kLibraryPrefixCid, kTypeArgumentsCid, kAbstractTypeCid, kTypeCid, kTypeRefCid, kTypeParameterCid, kClosureCid, kNumberCid, kIntegerCid, kSmiCid, kMintCid, kDoubleCid, kBoolCid, kGrowableObjectArrayCid, kFloat32x4Cid, kInt32x4Cid, kFloat64x2Cid, kTypedDataBaseCid, kTypedDataCid, kExternalTypedDataCid, kTypedDataViewCid, kPointerCid, kDynamicLibraryCid, kCapabilityCid, kReceivePortCid, kSendPortCid, kStackTraceCid, kRegExpCid, kWeakPropertyCid, kMirrorReferenceCid, kLinkedHashMapCid, kFutureOrCid, kUserTagCid, kTransferableTypedDataCid, kWeakSerializationReferenceCid, kArrayCid, kImmutableArrayCid, kStringCid, kOneByteStringCid, kTwoByteStringCid, kExternalOneByteStringCid, kExternalTwoByteStringCid,
        kFfiPointerCid, kFfiNativeFunctionCid, kFfiInt8Cid, kFfiInt16Cid, kFfiInt32Cid, kFfiInt64Cid, kFfiUint8Cid, kFfiUint16Cid, kFfiUint32Cid, kFfiUint64Cid, kFfiIntPtrCid, kFfiFloatCid, kFfiDoubleCid, kFfiVoidCid, kFfiHandleCid, kFfiNativeTypeCid, kFfiDynamicLibraryCid, kFfiStructCid,
        kWasmInt32Cid, kWasmInt64Cid, kWasmFloatCid, kWasmDoubleCid, kWasmVoidCid,
        kTypedDataInt8ArrayCid, kTypedDataInt8ArrayViewCid, kExternalTypedDataInt8ArrayCid, kTypedDataUint8ArrayCid, kTypedDataUint8ArrayViewCid, kExternalTypedDataUint8ArrayCid, kTypedDataUint8ClampedArrayCid, kTypedDataUint8ClampedArrayViewCid, kExternalTypedDataUint8ClampedArrayCid, kTypedDataInt16ArrayCid, kTypedDataInt16ArrayViewCid, kExternalTypedDataInt16ArrayCid, kTypedDataUint16ArrayCid, kTypedDataUint16ArrayViewCid, kExternalTypedDataUint16ArrayCid, kTypedDataInt32ArrayCid, kTypedDataInt32ArrayViewCid, kExternalTypedDataInt32ArrayCid, kTypedDataUint32ArrayCid, kTypedDataUint32ArrayViewCid, kExternalTypedDataUint32ArrayCid, kTypedDataInt64ArrayCid, kTypedDataInt64ArrayViewCid, kExternalTypedDataInt64ArrayCid, kTypedDataUint64ArrayCid, kTypedDataUint64ArrayViewCid, kExternalTypedDataUint64ArrayCid, kTypedDataFloat32ArrayCid, kTypedDataFloat32ArrayViewCid, kExternalTypedDataFloat32ArrayCid, kTypedDataFloat64ArrayCid, kTypedDataFloat64ArrayViewCid, kExternalTypedDataFloat64ArrayCid, kTypedDataFloat32x4ArrayCid, kTypedDataFloat32x4ArrayViewCid, kExternalTypedDataFloat32x4ArrayCid, kTypedDataInt32x4ArrayCid, kTypedDataInt32x4ArrayViewCid, kExternalTypedDataInt32x4ArrayCid, kTypedDataFloat64x2ArrayCid, kTypedDataFloat64x2ArrayViewCid, kExternalTypedDataFloat64x2ArrayCid,
        kByteDataViewCid,

        kByteBufferCid,

        kNullCid,
        kDynamicCid,
        kVoidCid,
        kNeverCid,

        kNumPredefinedCids
    }

    public static class ClassIdHelper
    { 
        public const int kTypedDataCidRemainderInternal = 0;
        public const int kTypedDataCidRemainderView = 1;
        public const int kTypedDataCidRemainderExternal = 2;
        public const int kTopLevelCidOffset = (1 << 16);

        public static bool IsTypedDataBaseClassId(int index)
        { 
            return index >= (int)ClassId.kTypedDataInt8ArrayCid && index < (int)ClassId.kByteDataViewCid;
        }
        public static bool IsTypedDataViewClassId(int cid)
        {
            return cid == (int)ClassId.kByteDataViewCid
                || (IsTypedDataBaseClassId(cid) && ((cid - (int)ClassId.kTypedDataInt8ArrayCid) % 3) == kTypedDataCidRemainderView);
        }
        public static bool IsExternalTypedDataClassId(int cid)
        {
            return IsTypedDataBaseClassId(cid) && ((cid - (int)ClassId.kTypedDataInt8ArrayCid) % 3) == kTypedDataCidRemainderExternal;
        }
        public static bool IsTypedDataClassId(int cid)
        {
            return IsTypedDataBaseClassId(cid) && ((cid - (int)ClassId.kTypedDataInt8ArrayCid) % 3) == kTypedDataCidRemainderInternal;
        }
        public static bool IsInternalVMdefinedClassId(int cid)
        {
            return ((cid < (int)ClassId.kNumPredefinedCids) && !IsImplicitFieldClassId(cid));
        }
        public static bool IsImplicitFieldClassId(int cid)
        {
            return cid == (int)ClassId.kByteBufferCid;
        }
        public static bool IsTopLevelCid(int cid)
        {
            return cid >= kTopLevelCidOffset;
        }
    }
}
