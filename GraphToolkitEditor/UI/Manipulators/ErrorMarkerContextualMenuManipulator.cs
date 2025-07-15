using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Customization of the <see cref="ContextualMenuManipulator"/> for an <see cref="ErrorMarker"/>.
    /// </summary>
    [UnityRestricted]
    internal class ErrorMarkerContextualMenuManipulator : ContextualMenuManipulator
    {
        /// <inheritdoc cref="ContextualMenuManipulator(Action{ContextualMenuPopulateEvent})"/>
        public ErrorMarkerContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
            : base(menuBuilder)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }
    }
}
