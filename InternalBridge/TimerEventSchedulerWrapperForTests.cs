using System;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.InternalBridge
{
    // *** Only used in tests ***
    class TimerEventSchedulerWrapperForTests : IDisposable
    {
        readonly VisualElement m_VisualElement;

        public static TimerEventSchedulerWrapperForTests CreateTimerEventSchedulerWrapper(VisualElement graphView)
        {
            return new TimerEventSchedulerWrapperForTests(graphView);
        }

        TimerEventSchedulerWrapperForTests(VisualElement visualElement)
        {
            m_VisualElement = visualElement;
            Panel.TimeSinceStartup = () => TimeSinceStartup;
        }

        public long TimeSinceStartup { get; set; }

        public void Dispose()
        {
            Panel.TimeSinceStartup = null;
        }

        public void UpdateScheduledEvents()
        {
            TimerEventScheduler s = (TimerEventScheduler)m_VisualElement.elementPanel.scheduler;
            s.UpdateScheduledEvents();
        }

        public static void SetTimeSinceStartupCallback(Func<long> cb)
        {
            if (cb == null)
                Panel.TimeSinceStartup = null;
            else
                Panel.TimeSinceStartup = () => cb();
        }
    }
}
