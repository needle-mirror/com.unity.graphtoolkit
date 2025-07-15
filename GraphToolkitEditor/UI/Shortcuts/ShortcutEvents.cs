using System;
using UnityEditor.ShortcutManagement;
using UnityEngine;

// ReSharper disable RedundantArgumentDefaultValue

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// An event sent by the Frame All shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    [UnityRestricted]
    internal class ShortcutFrameAllEvent : ShortcutEventBase<ShortcutFrameAllEvent>
    {
        public const string id = "Frame All";
        const KeyCode k_KeyCode = KeyCode.A;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.None;
    }

    /// <summary>
    /// An event sent by the Frame Origin shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    [UnityRestricted]
    internal class ShortcutFrameOriginEvent : ShortcutEventBase<ShortcutFrameOriginEvent>
    {
        public const string id = "Frame Origin";
        const KeyCode k_KeyCode = KeyCode.O;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.None;
    }

    /// <summary>
    /// An event sent by the Delete shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    [UnityRestricted]
    internal class ShortcutDeleteEvent : ShortcutEventBase<ShortcutDeleteEvent>
    {
        public const string id = "Delete";
        const KeyCode k_KeyCode = KeyCode.Backspace;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.None;
    }

    /// <summary>
    /// An event sent by the Show Item Library shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    [UnityRestricted]
    internal class ShortcutShowItemLibraryEvent : ShortcutEventBase<ShortcutShowItemLibraryEvent>
    {
        public const string id = "Show Item Library";
        const KeyCode k_KeyCode = KeyCode.Space;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.None;
    }

    /// <summary>
    /// An event sent by the Convert Variable And Constant shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    [UnityRestricted]
    internal class ShortcutConvertConstantAndVariableEvent : ShortcutEventBase<ShortcutConvertConstantAndVariableEvent>
    {
        public const string id = "Convert Variable And Constant";
        const KeyCode k_KeyCode = KeyCode.C;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.None;
    }

    /* TODO OYT (GTF-804): For V1, access to the Align Items and Align Hierarchy features was removed as they are confusing to users. To be improved before making them accessible again.
    /// <summary>
    /// An event sent by the Align Nodes shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    public class ShortcutAlignNodesEvent : ShortcutEventBase<ShortcutAlignNodesEvent>
    {
        public const string id = "Align Nodes";
        const KeyCode k_KeyCode = KeyCode.I;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.None;
    }

    /// <summary>
    /// An event sent by the Align Hierarchies shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    public class ShortcutAlignNodeHierarchiesEvent : ShortcutEventBase<ShortcutAlignNodeHierarchiesEvent>
    {
        public const string id = "Align Hierarchies";
        const KeyCode k_KeyCode = KeyCode.I;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.Shift;
    }
    */

    /// <summary>
    /// An event sent by the Create Sticky Note.
    /// </summary>
    [ToolShortcutEvent(null, id)]
    [UnityRestricted]
    internal class ShortcutCreateStickyNoteEvent : ShortcutEventBase<ShortcutCreateStickyNoteEvent>
    {
        public const string id = "Create Sticky Note";
    }

    /// <summary>
    /// An event sent by the Paste Without Wires shortcut.
    /// </summary>
    /// <remarks>The same shortcut is used for "Paste Transitions as New"</remarks>
    [ToolShortcutEvent(null, id, keyCode, modifiers)]
    [UnityRestricted]
    internal class ShortCutPasteWithoutWires : ShortcutEventBase<ShortCutPasteWithoutWires>
    {
        public const string id = "Paste Without Wires";
        const KeyCode keyCode = KeyCode.V;
        const ShortcutModifiers modifiers = ShortcutModifiers.Shift | ShortcutModifiers.Action;
    }

    /// <summary>
    /// An event sent by the Duplicate Without Wires shortcut.
    /// </summary>
    [ToolShortcutEvent(null, id, k_KeyCode, k_Modifiers)]
    [UnityRestricted]
    internal class ShortCutDuplicateWithoutWires : ShortcutEventBase<ShortCutDuplicateWithoutWires>
    {
        public const string id = "Duplicate Without Wires";
        const KeyCode k_KeyCode = KeyCode.D;
        const ShortcutModifiers k_Modifiers = ShortcutModifiers.Shift | ShortcutModifiers.Action;
    }
}
