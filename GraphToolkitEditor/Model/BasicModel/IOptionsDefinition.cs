using System;

namespace Unity.GraphToolkit.Editor
{
    interface IOptionsDefinition
    {
        INodeOption AddNodeOption(string optionName, Type dataType, string optionDisplayName = null, string tooltip = null,
            bool showInInspectorOnly = false, int order = 0, Attribute[] attributes = null, object defaultValue = null);
    }
}
