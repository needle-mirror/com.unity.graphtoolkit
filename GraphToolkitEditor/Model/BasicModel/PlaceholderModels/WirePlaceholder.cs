using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A model that represents the placeholder of a wire.
    /// </summary>
    [Serializable]
    class WirePlaceholder : WireModel, IPlaceholder
    {
        /// <inheritdoc />
        public long ReferenceId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WirePlaceholder" /> class.
        /// </summary>
        public WirePlaceholder()
        {
            PlaceholderModelHelper.SetPlaceholderCapabilities(this);
        }
    }
}
