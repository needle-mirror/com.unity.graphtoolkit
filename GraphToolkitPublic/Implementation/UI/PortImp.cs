using System;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor.Implementation.UI
{
    class PortImp : Port
    {
        protected override Action<MeshGenerationContext> GetConnectorVisualContentGenerator()
        {
            if (PortModel is PortModelImp { ConnectorUI: PortConnectorUI.Arrowhead })
                return OnGenerateTriangleConnectorVisualContent;

            return OnGenerateCircleConnectorVisualContent;
        }
    }

    [GraphElementsExtensionMethodsCache(typeof(GraphView))] // Use graphview so it encompasses GraphViewImp and PreviewGraphViewImp
    static class GraphToolkitImpFactoryExtensions
    {
        public static ModelView CreatePort(this ElementBuilder elementBuilder, PortModelImp model)
        {
            var ui = new PortImp();
            ui.SetupBuildAndUpdate(model, elementBuilder.View, elementBuilder.Context);
            return ui;
        }
    }
}
