using Unity.GraphToolkit.CSO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Editor
{
    class GraphProcessingStatusObserver : StateObserver
    {
        Label m_StatusLabel;
        GraphProcessingStateComponent m_GraphProcessingStateComponent;

        public GraphProcessingStatusObserver(Label statusLabel, GraphProcessingStateComponent graphProcessingState)
            : base(graphProcessingState)
        {
            m_StatusLabel = statusLabel;
            m_GraphProcessingStateComponent = graphProcessingState;
        }

        public override void Observe()
        {
            using (var observation = this.ObserveState(m_GraphProcessingStateComponent))
            {
                if (observation.UpdateType != UpdateType.None)
                {
                    m_StatusLabel?.EnableInClassList(
                        GraphViewEditorWindow.graphProcessingPendingUssClassName,
                        m_GraphProcessingStateComponent.GraphProcessingPending);
                }
            }
        }
    }
}
