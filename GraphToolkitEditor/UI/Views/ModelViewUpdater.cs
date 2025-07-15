using System;
using Unity.GraphToolkit.CSO;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Observer that updates a <see cref="View"/>.
    /// </summary>
    [UnityRestricted]
    internal class ModelViewUpdater : StateObserver
    {
        RootView m_View;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelViewUpdater" /> class.
        /// </summary>
        /// <param name="view">The <see cref="View"/> to update.</param>
        /// <param name="observedStateComponents">The state components that can cause the view to be updated.</param>
        public ModelViewUpdater(RootView view, params IStateComponent[] observedStateComponents) :
            base(observedStateComponents)
        {
            m_View = view;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelViewUpdater" /> class.
        /// </summary>
        /// <param name="view">The <see cref="View"/> to update.</param>
        /// <param name="observedStateComponents">The state components that can cause the view to be updated.</param>
        /// <param name="modifiedStateComponents">The state components that are modified by the view.</param>
        public ModelViewUpdater(RootView view, IStateComponent[] observedStateComponents, IStateComponent[] modifiedStateComponents) :
            base(observedStateComponents, modifiedStateComponents)
        {
            m_View = view;
        }

        /// <inheritdoc/>
        public override void Observe()
        {
            m_View?.Update();
        }
    }
}
