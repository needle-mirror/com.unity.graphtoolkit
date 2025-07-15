using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolsAuthoringFramework.InternalEditorBridge
{
    static class BridgeForTests
    {
        public static void SetDisableInputEvents(this EditorWindow window, bool value)
        {
            window.disableInputEvents = value;
        }

        public static void ClearPersistentViewData(this EditorWindow window)
        {
            window.ClearPersistentViewData();
        }

        public static void DisableViewDataPersistence(this EditorWindow window)
        {
            window.DisableViewDataPersistence();
        }
    }
}
