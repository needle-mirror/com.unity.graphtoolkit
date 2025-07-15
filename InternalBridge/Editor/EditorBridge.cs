using System;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Utils;
using UnityEngine;

namespace Unity.GraphToolsAuthoringFramework.InternalEditorBridge
{
    static class EditorBridge
    {
        static int LogTypeOptionsToMode(LogType logType, LogOption logOptions)
        {
            ConsoleWindow.Mode mode;

            if (logType == LogType.Log) // LogType::Log
                mode = ConsoleWindow.Mode.ScriptingLog;
            else if (logType == LogType.Warning) // LogType::Warning
                mode = ConsoleWindow.Mode.ScriptingWarning;
            else if (logType == LogType.Error) // LogType::Error
                mode = ConsoleWindow.Mode.ScriptingError;
            else if (logType == LogType.Exception) // LogType::Exception
                mode = ConsoleWindow.Mode.ScriptingException;
            else
                mode = ConsoleWindow.Mode.ScriptingAssertion;

            if (logOptions == LogOption.NoStacktrace)
                mode |= ConsoleWindow.Mode.DontExtractStacktrace;

            return (int)mode;
        }

        public static string GetShortcutMenuString(this ShortcutBinding binding)
        {
            //Note: DefaultBinding.ToString does not return the right string to be added in the menu item.
            // It returns an already formatted string with the right unicode characters, which cause the editor code, which translates the string itself to not right align the shortcut in the menu.
            return KeyCombination.SequenceToMenuString(binding.keyCombinationSequence);
        }

#if UGTK_INSTANCE_ID
        public static void SetEntryDoubleClickedDelegate(Action<string, InstanceID> doubleClickedCallback)
#else
        public static void SetEntryDoubleClickedDelegate(Action<string, int> doubleClickedCallback)
#endif
        {
            ConsoleWindow.entryWithManagedCallbackDoubleClicked += CallEntryDoubleClickedCallback;

            return;

            void CallEntryDoubleClickedCallback(LogEntry logEntry) => doubleClickedCallback(logEntry.file, logEntry.instanceID);
        }

#if UGTK_INSTANCE_ID && UGTK_ULONG_LOG_ID
        public static void AddMessageWithDoubleClickCallback(string message, string file, LogType logType, LogOption logOptions, InstanceID instanceId, ulong logIdentifier)
#else
        public static void AddMessageWithDoubleClickCallback(string message, string file, LogType logType, LogOption logOptions, int instanceId, int logIdentifier)
#endif
        {
            int mode = LogTypeOptionsToMode(logType, logOptions) | (int)ConsoleWindow.Mode.StickyError;

            LogEntries.AddMessageWithDoubleClickCallback(new LogEntry
            {
                message = message,
                file = file,
                mode = mode,
                identifier = logIdentifier,
                instanceID = instanceId,
            });
        }

        public static void ShowConsoleWindow(bool immediate)
        {
            ConsoleWindow.ShowConsoleWindow(immediate);
        }

        public static Action CallDelayed(EditorApplication.CallbackFunction action, double delaySeconds = 0.0f)
        {
            return EditorApplication.CallDelayed(action, delaySeconds);
        }

        public static void ShowColorPicker(Action<Color> colorChangedCallback, Color col, bool showAlpha = true, bool hdr = false)
        {
            ColorPicker.Show(colorChangedCallback, col, showAlpha, hdr);
        }

        public static string NormalizePath(this string path)
        {
            return Paths.NormalizePath(path);
        }

        public static KeyCombination FromKeyboardInput(KeyCode keyCode, EventModifiers modifiers)
        {
            return KeyCombination.FromKeyboardInput(keyCode, modifiers);
        }

        public static bool CanClose(EditorWindow window)
        {
            return ContainerWindow.CanClose(window);
        }

        public static void RegisterFileSavedCallback(EditorApplication.CallbackFunction callback)
        {
            EditorApplication.fileMenuSaved += callback;
        }

        public static bool HasCustomPropertyDrawer(Type type)
        {
            var drawerType = ScriptAttributeUtility.GetDrawerTypeForType(type, null);
            return typeof(PropertyDrawer).IsAssignableFrom(drawerType);
        }
    }
}
