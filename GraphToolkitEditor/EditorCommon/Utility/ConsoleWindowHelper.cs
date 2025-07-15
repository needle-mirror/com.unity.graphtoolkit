using System;
using Unity.GraphToolkit.InternalBridge;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    static class ConsoleWindowHelper
    {
        const string k_LogIdentifier = "GraphToolkit";

#if UGTK_ULONG_LOG_ID
        public static void LogSticky(string message, string file, LogType logType, LogOption logOptions, InstanceID instanceId, string windowId)
        {
            EditorBridge.AddMessageWithDoubleClickCallback(message, file, logType, logOptions, instanceId, (ulong)(k_LogIdentifier + windowId).GetHashCode());
        }

#else
        public static void LogSticky(string message, string file, LogType logType, LogOption logOptions, int instanceId, string windowId)
        {
            EditorBridge.AddMessageWithDoubleClickCallback(message, file, logType, logOptions, instanceId, (k_LogIdentifier + windowId).GetHashCode());
        }

#endif

        public static void RemoveLogEntries(string windowId)
        {
#if UGTK_ULONG_LOG_ID
            EngineBridge.RemoveLogEntriesByIdentifier((ulong)(k_LogIdentifier + windowId).GetHashCode());
#else
            EngineBridge.RemoveLogEntriesByIdentifier((k_LogIdentifier + windowId).GetHashCode());
#endif
        }

        public static void ShowConsoleWindow(bool immediate = true)
        {
            EditorBridge.ShowConsoleWindow(immediate);
        }
    }
}
