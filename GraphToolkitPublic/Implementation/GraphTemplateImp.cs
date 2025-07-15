using System;
using System.Reflection;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class GraphTemplateImp : GraphTemplate
    {
        /// <inheritdoc />
        public override Type GraphModelType { get; }

        public override string NewAssetName { get; }

        public GraphTemplateImp(Type graphType, string newAssetName = "New Graph")
            : base(newAssetName, GetGraphExtension(graphType))
        {
            GraphModelType = typeof(GraphModelImp);
            NewAssetName = newAssetName;
        }

        static string GetGraphExtension(Type graphType)
        {
            return graphType.GetCustomAttribute<GraphAttribute>(false)?.extension;
        }
    }
    class SubgraphTemplateImp : GraphTemplateImp
    {
        Type m_GraphType;

        public SubgraphTemplateImp(Type graphType, string graphTypeName = "Graph")
            : base(graphType, graphTypeName)
        {
            m_GraphType = graphType;
        }

        public override void InitBasicGraph(GraphModel graphModel)
        {
            base.InitBasicGraph(graphModel);

            // the GraphModel will first use the graphtype from its GraphObjectImp, we need to change it to the subgraph type.
            if (graphModel is GraphModelImp graphModelImp && ! m_GraphType.IsInstanceOfType(graphModelImp.Graph))
            {
                graphModelImp.RecreateGraph(m_GraphType);
            }
        }
    }
}
