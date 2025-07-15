using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Data to be displayed by a marker.
    /// </summary>
    [UnityRestricted]
    internal abstract class MarkerModel : Model
    {
        /// <summary>
        /// The model to which the marker is attached.
        /// </summary>
        public abstract GraphElementModel GetParentModel(GraphModel graphModel);
    }
}
