namespace ASOFT.CoreAI.Entities
{
    public sealed class CollectionModelBuildingOptions
    {
        /// <summary>
        /// Gets a value that indicates whether multiple key properties are supported.
        /// </summary>
        public bool SupportsMultipleKeys { get; init; }

        /// <summary>
        /// Gets a value that indicates whether multiple vector properties are supported.
        /// </summary>
        public bool SupportsMultipleVectors { get; init; }

        /// <summary>
        /// Gets a value that indicates whether at least one vector property is required.
        /// </summary>
        public bool RequiresAtLeastOneVector { get; init; }

        /// <summary>
        /// Gets a value that indicates whether an external serializer will be used (for example, System.Text.Json).
        /// </summary>
        public bool UsesExternalSerializer { get; init; }

        /// <summary>
        /// Gets the special, reserved name for the key property of the database.
        /// When set, the model builder manages the key storage name, and users cannot customize it.
        /// </summary>
        public string? ReservedKeyStorageName { get; init; }
    }
}