using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Model for exit portals.
    /// </summary>
    [Serializable]
    [UnityRestricted]
    internal class WirePortalExitModel : WirePortalModel, ISingleOutputPortNodeModel
    {
        /// <inheritdoc />
        public PortModel OutputPort { get; protected set; }

        /// <inheritdoc />
        protected override void OnDefineNode(NodeDefinitionScope scope)
        {
            OutputPort = scope.AddOutputPort("", GetPortDataTypeHandle(), PortType);
        }

        /// <inheritdoc />
        public override bool CanCreateOppositePortal()
        {
            var portalRefs = GraphModel.FindReferencesInGraph<WirePortalModel>(DeclarationModel);
            foreach (var portalRef in portalRefs)
            {
                if (portalRef is ISingleInputPortNodeModel)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
