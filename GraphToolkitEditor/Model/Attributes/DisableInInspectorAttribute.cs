using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Attribute to disable a field in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [UnityRestricted]
    internal class DisableInInspectorAttribute : Attribute
    {
    }
}
