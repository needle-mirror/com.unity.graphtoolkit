using System;
using System.Collections.Generic;
using UnityEditor;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class PublicGraphElementItemDatabase : GraphElementItemDatabase
    {
        /// <inheritdoc />
        public PublicGraphElementItemDatabase(GraphModel graphModel)
            : base(graphModel) {}

        /// <inheritdoc />
        protected override IEnumerable<GraphModel> GetSubgraphs()
        {
            if (GraphModel is not GraphModelImp graphModelImp)
                return new List<GraphModel>();

            List<GraphModel> subGraphModels = null;

            // Get Local sub-graphs
            if (GraphModel is not null && GraphModel.LocalSubgraphs.Count > 0)
            {
                subGraphModels = new List<GraphModel>();
                subGraphModels.AddRange(GraphModel.LocalSubgraphs);
            }

            // Get Asset sub-graphs
            var graphObjectType = graphModelImp.GetType();
#if UGTK_ASSET_DATABASE_SEARCH
            var searchQuery = AssetDatabase.SourceSearchQuery.ByObjectTypeNameInNativeAssets(graphObjectType.Name);
            var searchResults = AssetDatabase.SearchAsync(searchQuery);

            if (searchResults.Result.Items.Length > 0)
            {
                var results = searchResults.Result.Items.Select(i => i.guid);
#else
            var validSubgraphTypes = PublicGraphFactory.GetSubGraphTypes(graphModelImp.Graph.GetType());
            var validExtensions = new List<string>(validSubgraphTypes.Count);
            const string globFilterStr = "glob:\"*.{0}\"";
            var filter = string.Empty;
            for (var i = 0; i < validSubgraphTypes.Count; i++)
            {
                var subgraphType = validSubgraphTypes[i];
                var extension = PublicGraphFactory.GetExtensionByGraphType(subgraphType);
                if (extension == null || validExtensions.Contains(extension))
                    continue;

                if (i > 0)
                    filter += " ";

                filter += string.Format(globFilterStr, extension);
                validExtensions.Add(extension);
            }

            var results = AssetDatabase.FindAssets(filter);

            if (results.Length > 0)
            {
#endif
                subGraphModels ??= new List<GraphModel>();
                foreach (var result in results)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(result);
                    var assetGraphModel = GraphObject.LoadGraphObjectAtPath(assetPath, graphObjectType)?.GraphModel;
                    if (assetGraphModel is not null && !assetGraphModel.IsContainerGraph() && assetGraphModel.CanBeSubgraph())
                        subGraphModels.Add(assetGraphModel);
                }
            }

            return subGraphModels;
        }
    }
}
