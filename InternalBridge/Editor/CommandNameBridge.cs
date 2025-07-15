namespace Unity.GraphToolkit.InternalBridge
{
    static class CommandMenuItemNames
    {
        public static readonly string Cut = "Cut " + UnityEditor.Menu.GetHotkey("Edit/Cut");
        public static readonly string Copy = "Copy " + UnityEditor.Menu.GetHotkey("Edit/Copy");
        public static readonly string Paste = "Paste " + UnityEditor.Menu.GetHotkey("Edit/Paste");
        public static readonly string Duplicate = "Duplicate " + UnityEditor.Menu.GetHotkey("Edit/Duplicate");
        public static readonly string Delete = "Delete " + UnityEditor.Menu.GetHotkey("Edit/Delete");
        public static readonly string FrameSelected = "Frame In Graph " + UnityEditor.Menu.GetHotkey("Edit/Frame Selected in Window under Cursor");
        public static readonly string Rename = "Rename " + UnityEditor.Menu.GetHotkey("Edit/Rename");
        public static readonly string SelectAll = "Select All " + UnityEditor.Menu.GetHotkey("Edit/Select All");
    }
}
