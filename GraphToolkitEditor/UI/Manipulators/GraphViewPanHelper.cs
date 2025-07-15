using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    class GraphViewPanHelper
    {
        IVisualElementScheduledItem m_PanSchedule;
        GraphView m_GraphView;
        Action<TimerState> m_OnPan;

        public Vector2 CurrentPanSpeed { get; private set; } = Vector2.zero;
        public Vector2 LastLocalMousePosition { get; private set; }
        public Vector2 TraveledThisFrame { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Scale { get; private set; }

        public void OnMouseDown(IMouseEvent e, GraphView graphView, Action<TimerState> onPan)
        {
            if (graphView == null)
                return;

            m_GraphView = graphView;
            m_OnPan = onPan;

            if (m_PanSchedule == null)
            {
                var panInterval = GraphView.panIntervalMs;
                m_PanSchedule = m_GraphView.schedule.Execute(Pan).Every(panInterval).StartingIn(panInterval);
                m_PanSchedule.Pause();
            }

            LastLocalMousePosition = e.localMousePosition;
        }

        public void OnMouseMove(IMouseEvent e)
        {
            if (m_GraphView == null)
                return;

            LastLocalMousePosition = e.localMousePosition;
            CurrentPanSpeed = m_GraphView.GetEffectivePanSpeed(e.mousePosition);
            if (CurrentPanSpeed != Vector2.zero)
            {
                m_PanSchedule.Resume();
            }
            else
            {
                m_PanSchedule.Pause();
            }
        }

        public void OnMouseUp(IMouseEvent e)
        {
            LastLocalMousePosition = e.localMousePosition;
            Stop();
        }

        public void Stop()
        {
            m_PanSchedule?.Pause();
            m_OnPan = null;
        }

        void Pan(TimerState timerState)
        {
            if (m_GraphView == null)
                return;

            TraveledThisFrame = CurrentPanSpeed * timerState.deltaTime;
            Position = m_GraphView.ContentViewContainer.resolvedStyle.translate - (Vector3)TraveledThisFrame;
            Scale = m_GraphView.ContentViewContainer.resolvedStyle.scale.value;
            m_GraphView.Dispatch(new ReframeGraphViewCommand(Position, Scale));

            m_OnPan?.Invoke(timerState);
        }
    }
}
