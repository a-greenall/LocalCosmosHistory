using System;
using Newtonsoft.Json;

namespace LocalCosmosHistory
{
    public abstract class CosmosItem
    {
        /// <summary>
        /// Gets or sets the item's globally unique identifier.
        /// </summary>
        /// <remarks>
        /// Initialized by <see cref="Guid.NewGuid"/>.
        /// </remarks>
        [JsonProperty("id")]
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Gets or sets the item's type name. This is used as a discriminator.
        /// </summary>
        [JsonIgnore]
        public string Type { get; set; }
        
        public abstract string PartitionKey { get; }
        
        /// <summary>
        /// Default constructor, assigns type name to <see cref="Type"/> property.
        /// </summary>
        public CosmosItem() => Type = GetType().Name;
    }
}