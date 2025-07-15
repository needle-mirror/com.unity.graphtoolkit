using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    [UnityRestricted]
    internal class MissingBlocksContainerPart : BlocksContainerPart
    {
        public MissingBlocksContainerPart(string name, ContextNodeModel model, ChildView ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {}

        public override void SetLevelOfDetail(float zoom, GraphViewZoomMode newZoomMode, GraphViewZoomMode oldZoomMode)
        {
            foreach (var blockPlaceholder in Root.Children())
            {
                if (blockPlaceholder is GraphElement ge)
                    ge.SetLevelOfDetail(zoom, newZoomMode, oldZoomMode);
            }
        }

        /// <inheritdoc />
        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor)
        {
            using (m_BlocksContainer.Children().OfTypeToPooledList(out List<ModelView> blockPlaceholders))
                UpdateBlocks(ContextNodeModel.BlockPlaceholders, blockPlaceholders);
        }
    }
}
