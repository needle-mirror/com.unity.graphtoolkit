using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    class ContextualizedModelViews
    {
        public readonly RootView View;
        public readonly IViewContext Context;
        public readonly Dictionary<Hash128, ChildView> ModelViews;

        public ContextualizedModelViews(RootView view, IViewContext context)
        {
            View = view;
            Context = context;
            ModelViews = new Dictionary<Hash128, ChildView>();
        }
    }
}
