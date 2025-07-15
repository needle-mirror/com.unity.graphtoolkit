using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base classes for view that display an aggregation of models.
    /// </summary>
    [UnityRestricted]
    internal abstract class MultipleModelsView : ChildView
    {
        /// <summary>
        /// The models that backs the UI.
        /// </summary>
        public IReadOnlyList<Model> Models { get; private set; }

        /// <summary>
        /// Helper method that calls <see cref="Setup"/>, <see cref="View.BuildUITree"/> and <see cref="ChildView.DoCompleteUpdate"/>.
        /// </summary>
        /// <param name="models">The models that backs the instance.</param>
        /// <param name="view">The view to which the instance should be added.</param>
        /// <param name="context">The UI creation context.</param>
        public void SetupBuildAndUpdate(IReadOnlyList<Model> models, RootView view, IViewContext context = null)
        {
            Setup(models, view, context);
            BuildUITree();
            DoCompleteUpdate();
        }

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <param name="models">The models that backs the instance.</param>
        /// <param name="view">The view to which the instance should be added.</param>
        /// <param name="context">The UI creation context.</param>
        public void Setup(IReadOnlyList<Model> models, RootView view, IViewContext context = null)
        {
            Models = models;
            Setup(view, context);
        }

        /// <inheritdoc />
        public override void AddToRootView(RootView view)
        {
            base.AddToRootView(view);
            ViewForModel.AddOrReplaceModelView(this);
        }

        /// <inheritdoc />
        public override void RemoveFromRootView()
        {
            ViewForModel.RemoveModelView(this);
            base.RemoveFromRootView();
        }
    }
}
