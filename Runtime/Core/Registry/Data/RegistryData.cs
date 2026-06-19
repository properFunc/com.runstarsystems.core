using System;
using System.Collections.Generic;

namespace RunstarSystems.ECS.Data
{
    /*
    *   Allows the registry to store the meta data without the need
    *   for attributes.
    *   Converts attribute data to this becuase inhertance can place
    *   attribute effects post delivery.
    */
    public readonly struct RegistryMetadata
    {
        public readonly Type MatchedType;
        public readonly Type SourceType;
        public readonly Type KeyType;
        public readonly object Metadata;
        public readonly bool IsInherited;

        public RegistryMetadata(
                Type matched_type,
                Type source_type,
                Type key_type,
                object metadata,
                bool is_inherited)
        {
            MatchedType = matched_type;
            SourceType = source_type;
            KeyType = key_type;
            Metadata = metadata;
            IsInherited = is_inherited;
        }
    }

    public readonly struct RegistryMetadata<TMetadata>
    {
        public readonly Type MatchedType;
        public readonly Type SourceType;
        public readonly TMetadata Metadata;
        public readonly bool IsInherited;

        public RegistryMetadata(
                Type matched_type,
                Type source_type,
                TMetadata metadata,
                bool is_inherited)
        {
            MatchedType = matched_type;
            SourceType = source_type;
            Metadata = metadata;
            IsInherited = is_inherited;
        }
    }
}
