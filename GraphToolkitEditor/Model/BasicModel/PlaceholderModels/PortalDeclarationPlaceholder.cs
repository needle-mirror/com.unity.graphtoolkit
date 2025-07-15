using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A model that represents the placeholder of a portal declaration.
    /// </summary>
    [Serializable]
    class PortalDeclarationPlaceholder : DeclarationModel, IPlaceholder
    {
        /// <inheritdoc />
        public long ReferenceId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalDeclarationPlaceholder"/> class.
        /// </summary>
        public PortalDeclarationPlaceholder()
        {
            PlaceholderModelHelper.SetPlaceholderCapabilities(this);
        }
    }
}
