using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base class for transition UI.
    /// </summary>
    [UnityRestricted]
    internal abstract class AbstractTransition : AbstractWire
    {
        bool m_Hovered;

        /// <summary>
        /// The transition model.
        /// </summary>
        public TransitionSupportModel TransitionModel => Model as TransitionSupportModel;

        /// <summary>
        /// Whether the transition is hovered by the mouse.
        /// </summary>
        public virtual bool Hovered
        {
            get => m_Hovered;
            set => m_Hovered = value;
        }
    }
}
