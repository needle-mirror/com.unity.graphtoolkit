using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.InternalBridge;
using Unity.GraphToolsAuthoringFramework.InternalEditorBridge;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    class ShortcutBlocker
    {
        VisualElement m_RegistrationTarget;
        VisualElement m_DragTarget;

        Dictionary<string, HashSet<KeyCombination>> m_ForbiddenShortcuts = new();

        public void Enable(VisualElement dragTarget)
        {
            if (dragTarget == null)
                return;

            m_DragTarget = dragTarget;

            // It's possible that the target has not been attached to the panel yet. In that case,
            // delay the initialization of the registration target.
            if (dragTarget.panel == null)
            {
                dragTarget.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            }
            else
            {
                InitRegistrationTarget();
            }

            var shortcutIds = ShortcutManager.instance.GetAvailableShortcutIds();
            foreach (var shortcutId in shortcutIds)
            {
                var binding = ShortcutManager.instance.GetShortcutBinding(shortcutId);
                m_ForbiddenShortcuts.TryAdd(shortcutId, new HashSet<KeyCombination>(binding.keyCombinationSequence));
            }

            ShortcutManager.instance.shortcutBindingChanged += OnShortcutBindingChanged;

            m_DragTarget.RegisterCallback<PointerDownEvent>(OnPointerDownEvent, TrickleDown.TrickleDown);
            m_DragTarget.RegisterCallback<PointerUpEvent>(OnPointerUpEvent, TrickleDown.TrickleDown);
        }

        public void Disable()
        {
            if (m_DragTarget == null)
                return;

            // Remove all possible bindings
            ShortcutManager.instance.shortcutBindingChanged -= OnShortcutBindingChanged;
            m_DragTarget.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            m_DragTarget.UnregisterCallback<PointerDownEvent>(OnPointerDownEvent, TrickleDown.TrickleDown);
            m_DragTarget.UnregisterCallback<PointerUpEvent>(OnPointerUpEvent, TrickleDown.TrickleDown);
            UnregisterShortcutBlockingHandlers();
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_DragTarget.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            InitRegistrationTarget();
        }

        void InitRegistrationTarget()
        {
            // Get the top most VisualElement of the target so we can catch the keyboard events as soon
            // as possible. GetRoot is not enough to get the panel element.
            m_RegistrationTarget = m_DragTarget.GetFirstAncestorWhere(v => v.hierarchy.parent == null);
        }

        void OnShortcutBindingChanged(ShortcutBindingChangedEventArgs args)
        {
            if (!m_ForbiddenShortcuts.ContainsKey(args.shortcutId))
                return;
            m_ForbiddenShortcuts[args.shortcutId] = new HashSet<KeyCombination>(args.newBinding.keyCombinationSequence);
        }

        public void StartBlocking()
        {
            m_RegistrationTarget?.RegisterCallback<KeyDownEvent>(OnKeyDownEvent, TrickleDown.TrickleDown);
        }

        public void StopBlocking()
        {
            UnregisterShortcutBlockingHandlers();
        }

        void OnPointerDownEvent(PointerDownEvent evt)
        {
            // Don't stop the propagation of the event
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                m_DragTarget.RegisterCallback<PointerCaptureEvent>(OnPointerCaptureEvent, TrickleDown.TrickleDown);
                m_DragTarget.RegisterCallback<MouseLeaveWindowEvent>(OnMouseLeaveWindowEvent);
                StartBlocking();
            }
        }

        void OnMouseLeaveWindowEvent(MouseLeaveWindowEvent evt)
        {
            // We received a MouseLeaveWindowEvent and the mouse was not captured, so we are likely no longer focusing the
            // relevant target. Consider this as a PointerUpEvent. Don't stop the propagation of the event.
            StopBlocking();
        }

        void OnPointerCaptureEvent(PointerCaptureEvent evt)
        {
            // PointerDown captured the pointer, so we are not going
            // to receive a PointerUp event. Register PointerCaptureOut event instead.
            m_DragTarget.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOutEvent, TrickleDown.TrickleDown);

            // And since we are capturing the mouse, we are probably dragging something so no
            // need to check for MouseLeaveWindowEvent
            m_DragTarget.UnregisterCallback<MouseLeaveWindowEvent>(OnMouseLeaveWindowEvent);
        }

        void OnPointerUpEvent(PointerUpEvent evt)
        {
            // Don't stop the propagation of the event
            if (evt.button == (int)MouseButton.LeftMouse)
            {
                StopBlocking();
            }
        }

        void OnPointerCaptureOutEvent(PointerCaptureOutEvent evt)
        {
            // Don't stop the propagation of the event
            StopBlocking();
        }

        void OnKeyDownEvent(KeyDownEvent evt)
        {
            var currentCombination = EditorBridge.FromKeyboardInput(evt.keyCode, evt.modifiers);

            if (m_ForbiddenShortcuts.Values.Any(set => set.Contains(currentCombination)))
            {
                evt.StopPropagation();
            }
        }

        void UnregisterShortcutBlockingHandlers()
        {
            m_RegistrationTarget?.UnregisterCallback<KeyDownEvent>(OnKeyDownEvent, TrickleDown.TrickleDown);

            m_DragTarget.UnregisterCallback<PointerCaptureEvent>(OnPointerCaptureEvent, TrickleDown.TrickleDown);
            m_DragTarget.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOutEvent, TrickleDown.TrickleDown);
            m_DragTarget.UnregisterCallback<MouseLeaveWindowEvent>(OnMouseLeaveWindowEvent);
        }
    }
}
