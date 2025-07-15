using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for cloneable graph element models.
    /// </summary>
    [UnityRestricted]
    internal interface ICloneable
    {
        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>A clone of this graph element.</returns>
        Model Clone();
    }
}
