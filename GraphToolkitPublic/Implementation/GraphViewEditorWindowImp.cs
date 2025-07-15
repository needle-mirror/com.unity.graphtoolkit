using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class GraphViewEditorWindowImp : GraphViewEditorWindow
    {
        static List<GraphViewEditorWindowImp> s_OpenedWindows;

        public static GraphViewEditorWindowImp GetOpenedWindow(GraphObjectImp graphObject)
        {
            BuildOpenedWindows();
            for(int i = s_OpenedWindows.Count - 1; i >= 0; --i)
            {
                var window = s_OpenedWindows[i];
                if (window.GraphTool.ToolState.GraphObject == graphObject)
                {
                    return window;
                }
            }

            return null;
        }

        static void BuildOpenedWindows()
        {
            if (s_OpenedWindows != null)
                return;
            s_OpenedWindows = new List<GraphViewEditorWindowImp>();
            var allWindows = Resources.FindObjectsOfTypeAll<GraphViewEditorWindowImp>();
            foreach (var window in allWindows)
            {
                if (window.GraphTool != null && window.GraphTool.ToolState.GraphObject != null)
                {
                    s_OpenedWindows.Add(window);
                }
            }
        }

        public static void ShowGraph(GraphObjectImp graphObject)
        {
            BuildOpenedWindows();
            foreach (var window in s_OpenedWindows)
            {
                if (window?.GraphTool?.ToolState?.GraphObject == graphObject)
                {
                    window.Focus();
                    return;
                }
            }
            var graphWindow = EditorWindow.CreateWindow<GraphViewEditorWindowImp>(typeof(GraphViewEditorWindowImp), typeof(GraphViewEditorWindow), typeof(SceneView));
            graphWindow.GraphTool.Dispatch(new LoadGraphCommand(graphObject.GraphModel));
        }

        protected override void OnEnable()
        {
            BuildOpenedWindows();
            PublicGraphFactory.EnsureStaticConstructorIsCalled();
            s_OpenedWindows.Add(this);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            s_OpenedWindows?.Remove(this);
        }

        protected override void OnFocus()
        {
            base.OnFocus();

            BuildOpenedWindows();
            if (s_OpenedWindows.Count > 0 && s_OpenedWindows[^1] != this)
            {
                s_OpenedWindows.Remove(this);
                s_OpenedWindows.Add(this);
            }
        }

        protected override GraphTool CreateGraphTool()
        {
            return GraphTool.Create<GraphToolImp>(WindowID);
        }

        public override ItemLibraryHelper CreateItemLibraryHelper(GraphModel graphModel)
        {
            return new PublicLibraryHelper(graphModel);
        }

        protected override GraphView CreateGraphView(GraphRootViewModel viewModel, ViewSelection viewSelection)
        {
            return new GraphViewImp(this, GraphTool, GraphViewName, viewModel, viewSelection);
        }

        protected override BlackboardContentModel CreateBlackboardContentModel()
        {
            return new BlackboardContentModelImp(GraphTool);
        }
    }
}
