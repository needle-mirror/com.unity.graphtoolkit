using System;
using Unity.GraphToolkit.InternalBridge;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// UIToolkit event sent to ask for the library to be displayed.
    /// </summary>
    [UnityRestricted]
    internal class PromptItemLibraryEvent : EventBase<PromptItemLibraryEvent>
    {
        /// <summary>
        /// The location where the library should be displayed.
        /// </summary>
        public Vector2 MenuPosition;

        /// <summary>
        /// Gets a <see cref="PromptItemLibraryEvent"/> from the pool of events and initializes it.
        /// </summary>
        /// <param name="menuPosition">The location where the library should be displayed.</param>
        /// <returns>A freshly initialized event.</returns>
        public static PromptItemLibraryEvent GetPooled(Vector2 menuPosition)
        {
            var e = GetPooled();
            e.MenuPosition = menuPosition;
            return e;
        }

        /// <summary>
        /// Initializes the event.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.SetPropagationBothWay();
        }
    }
}
