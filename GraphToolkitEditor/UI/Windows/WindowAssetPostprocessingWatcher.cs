using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    class WindowAssetPostprocessingWatcher : AssetPostprocessor
    {
        // ReSharper disable once Unity.IncorrectMethodSignature
        // ReSharper disable once UnusedMember.Local
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            DoUpdatesOnAssetRename(importedAssets);

            UpdateExternalAssetStateComponents(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths, didDomainReload);

            UpdateBreadcrumbs(importedAssets, deletedAssets, movedAssets);
        }

        /// <summary>
        /// Renames the graph model name and updates the editor windows that are displaying the graph asset.
        /// </summary>
        /// <remarks>
        /// Even though the asset post processor has 'movedAssets' and 'movedFromAssetPaths', we use 'importedAssets' and compare it to the
        /// name stored in the model. This is because doing it this way catches the regular case (of an asset being renamed in the project window)
        /// but it *also* catches the case of a user renaming an asset outside the Unity editor (e.g. in the file system) and rename the asset
        /// but not the meta file. When this happens, the asset post processor will treat it as a new asset being imported (+ a deletedAsset).
        /// So the safest way for us to catch a rename in all cases is to just compare against the name stored in the Model.
        /// </remarks>
        static void DoUpdatesOnAssetRename(string[] importedAssets)
        {
            foreach (var assetFilePath in importedAssets)
            {
                var assetName = System.IO.Path.GetFileNameWithoutExtension(assetFilePath);
                var graphObject = GraphObject.LoadGraphObjectAtPath(assetFilePath);

                // Skip if the asset is not a graph asset
                if (graphObject == null) continue;

                if (graphObject.GraphModel.Name != assetName)
                    graphObject.GraphModel.Name = assetName;

                UpdateEditorWindowsOnGraphAssetRename(assetFilePath, assetName);
            }
        }

        static void UpdateEditorWindowsOnGraphAssetRename(string assetFilePath, string newName)
        {
            // Note: Could optimize to not include windows that aren't visible (no API to check this currently)
            // If we use CSO to do the update, this isn't required

#if UGTK_EDITOR_WINDOW_API
            var windows = EditorWindow.GetEditorWindowsOfType<GraphViewEditorWindow>();
#else
            var windows = Resources.FindObjectsOfTypeAll<GraphViewEditorWindow>();
#endif
            var guid = AssetDatabase.GUIDFromAssetPath(assetFilePath);
            foreach (var window in windows)
            {
                if (window.GraphView == null) continue;

                if (!IsWindowDisplayingGraphAsset(window, guid)) continue;

                window.OnUpdateModelName(newName);
            }
        }

        static void FilterOutOpenedAssets(string[] inPaths, IReadOnlyList<GraphReference> openedGraphs, List<string> outPaths, string[] correspondingPaths = null, List<string> outCorrespondingPaths = null)
        {
            for (var index = 0; index < inPaths.Length; index++)
            {
                var path = inPaths[index];
                var isOpened = false;
                foreach (var openedGraph in openedGraphs)
                {
                    if (openedGraph.RefersToFile(AssetDatabase.GUIDFromAssetPath(path)))
                    {
                        isOpened = true;
                        break;
                    }
                }

                if (!isOpened)
                {
                    outPaths.Add(path);

                    if (outCorrespondingPaths != null && correspondingPaths != null)
                    {
                        outCorrespondingPaths.Add(correspondingPaths[index]);
                    }
                }
            }
        }

        static void UpdateExternalAssetStateComponents(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (didDomainReload || importedAssets.Length == 0 && deletedAssets.Length == 0 && movedAssets.Length == 0)
                return;

#if UGTK_EDITOR_WINDOW_API
            var windows = EditorWindow.GetEditorWindowsOfType<GraphViewEditorWindow>();
#else
            var windows = Resources.FindObjectsOfTypeAll<GraphViewEditorWindow>();
#endif
            foreach (var window in windows)
            {
                var openedGraphs = window.GraphTool.ToolState.SubgraphStack;

                var importedAssetList = new List<string>(importedAssets.Length);
                FilterOutOpenedAssets(importedAssets, openedGraphs, importedAssetList);

                var movedAssetList = new List<string>(movedAssets.Length);
                var movedFromAssetPathList = new List<string>(movedFromAssetPaths.Length);
                FilterOutOpenedAssets(movedAssets, openedGraphs, movedAssetList, movedFromAssetPaths, movedFromAssetPathList);

                var deletedAssetList = new List<string>(deletedAssets.Length);
                FilterOutOpenedAssets(deletedAssets, openedGraphs, deletedAssetList);

                using var updater = window.GraphTool.ExternalAssetsState.UpdateScope;
                updater.AddImportedAssets(importedAssetList);
                updater.AddMovedAssets(movedAssetList, movedFromAssetPathList);
                updater.AddDeletedAssets(deletedAssetList);
            }
        }

        static void UpdateBreadcrumbs(string[] importedAssets, string[] deletedAssets, string[] movedAssets)
        {
            if (importedAssets.Length == 0 && deletedAssets.Length == 0 && movedAssets.Length == 0)
                return;

#if UGTK_EDITOR_WINDOW_API
            var windows = EditorWindow.GetEditorWindowsOfType<GraphViewEditorWindow>();
            if (windows.Count == 0)
                return;
#else
            var windows = Resources.FindObjectsOfTypeAll<GraphViewEditorWindow>();
            if (windows.Length == 0)
                return;
#endif

            var changedAssets = new HashSet<string>(importedAssets);
            changedAssets.UnionWith(movedAssets);
            changedAssets.UnionWith(deletedAssets);

            // Deleted graphs have already been unloaded by WindowAssetModificationWatcher, just before they were deleted.

            var changedGuids = changedAssets.ToDictionary(path => path, AssetDatabase.GUIDFromAssetPath);
            foreach (var window in windows)
            {
                if (window.GraphView != null)
                {
                    // Update all breadcrumb overlays for moved assets
                    foreach (var movedAsset in movedAssets)
                    {
                        if (!changedAssets.TryGetValue(movedAsset, out _))
                            continue;

                        var guid = AssetDatabase.GUIDFromAssetPath(movedAsset);

                        if (!IsWindowDisplayingGraphAsset(window, guid) && !IsWindowDisplayingSubgraphAssetOfParentGraphAsset(window, guid))
                            continue;

                        if (window.TryGetOverlay(BreadcrumbsToolbar.toolbarId, out var overlay))
                        {
                            var graphBreadcrumbs = overlay.rootVisualElement.SafeQ<GraphBreadcrumbs>();
                            graphBreadcrumbs?.Update();
                        }

                        break;
                    }
                }
            }
        }

        // Returns true if the window is displaying the graph graphGuid.
        public static bool IsWindowDisplayingGraphAsset(GraphViewEditorWindow window, GUID graphGuid)
        {
            return window.GraphTool?.ToolState.CurrentGraph.RefersToFile(graphGuid) ?? false;
        }

        // Returns true if the window is displaying a subgraph of the graph parentGraphGuid.
        static bool IsWindowDisplayingSubgraphAssetOfParentGraphAsset(GraphViewEditorWindow window, GUID parentGraphGuid)
        {
            return window.GraphTool != null && window.GraphTool.ToolState.SubgraphStack.Any(openedGraph => openedGraph.AssetGuid == parentGraphGuid);
        }
    }
}
