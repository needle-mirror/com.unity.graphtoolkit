using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Customization of the <see cref="ContextualMenuManipulator"/> for the <see cref="GraphView"/>.
    /// </summary>
    [UnityRestricted]
    internal class GraphViewContextualMenuManipulator : ContextualMenuManipulator
    {
        /// <inheritdoc cref="ContextualMenuManipulator(Action{ContextualMenuPopulateEvent})"/>
        public GraphViewContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
            : base(menuBuilder)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse, modifiers = EventModifiers.Command });
            }
            else
            {
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse, modifiers = EventModifiers.Control });
            }
        }

        /// <inheritdoc />
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(SelectOnMouseDown, TrickleDown.TrickleDown);
            base.RegisterCallbacksOnTarget();
        }

        void SelectOnMouseDown(MouseDownEvent e)
        {
            if (CanStartManipulation(e))
            {
                var baseEvent = (EventBase)e;

                if (baseEvent.currentTarget is GraphElement graphElement)
                {
                    if (!graphElement.IsSelected())
                    {
                        GraphViewClickSelector.SelectElements(graphElement, e.actionKey, true);
                    }
                }
            }
        }
    }
}
