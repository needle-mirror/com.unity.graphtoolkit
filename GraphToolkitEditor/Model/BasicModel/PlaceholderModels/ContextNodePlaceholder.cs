using System;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A model that represents the placeholder of a context node.
    /// </summary>
    [Serializable]
    class ContextNodePlaceholder : ContextNodeModel, IPlaceholder
    {
        /// <inheritdoc />
        public long ReferenceId { get; set; }

        /// <inheritdoc />
        protected override void OnDefineNode(NodeDefinitionScope scope)
        {
            base.OnDefineNode(scope);
            PlaceholderModelHelper.SetPlaceholderCapabilities(this);
        }

        /// <inheritdoc />
        protected override void DisconnectPort(PortModel portModel)
        {
            // We do not want to disconnect ports that are unused, to create missing ports.
        }
    }
}
