using System;

namespace Unity.GraphToolkit.Editor
{
    [GraphElementsExtensionMethodsCache(typeof(MiniMapView), GraphElementsExtensionMethodsCacheAttribute.lowestPriority)]
    [UnityRestricted]
    internal static class MiniMapViewFactoryExtensions
    {
        /// <summary>
        /// Creates a MiniMap from for the given model.
        /// </summary>
        /// <param name="elementBuilder">The element builder.</param>
        /// <param name="model">The <see cref="GraphModel"/> this <see cref="ModelView"/> will display.</param>
        /// <returns>A setup <see cref="ModelView"/>.</returns>
        public static ModelView CreateMiniMap(this ElementBuilder elementBuilder, GraphModel model)
        {
            ModelView ui = new MiniMap();
            ui.SetupBuildAndUpdate(model, elementBuilder.View, elementBuilder.Context);
            return ui;
        }
    }
}
