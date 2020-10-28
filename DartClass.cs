using System;
using System.Collections.Generic;
using System.Text;

namespace DartDeserialize
{
    public class DartClass
    {
        public int id;

        public DartObject name;
        public DartObject user_name;
        public DartObject functions;
        public DartObject functions_hash_table;
        public DartObject fields;
        public DartObject offset_in_words_to_field;
        public DartObject interfaces;  // Array of AbstractType.
        public DartObject script;
        public DartObject library;
        public DartObject type_parameters;  // Array of TypeParameter.
        public DartObject super_type;
        public DartObject signature_function;  // Associated function for typedef class.
        public DartObject constants;        // Canonicalized const instances of this class.
        public DartObject declaration_type;  // Declaration type for this class.
        public DartObject invocation_dispatcher_cache;  // Cache for dispatcher functions.
        public DartObject allocation_stub;                    // CHA optimized codes.

        public int host_instance_size_in_words;
        public int host_next_field_offset_in_words;
        public int host_type_arguments_field_offset_in_words;

        public int num_type_arguments;
        public int num_native_fields;
        public int token_pos;
        public int end_token_pos;
        public int state_bits;
    }
}
