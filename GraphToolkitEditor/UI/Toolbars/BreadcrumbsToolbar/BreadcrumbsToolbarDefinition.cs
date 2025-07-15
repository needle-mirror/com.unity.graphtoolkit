using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Class that defines the content of the breadcrumbs toolbar.
    /// </summary>
    [UnityRestricted]
    internal class BreadcrumbsToolbarDefinition : ToolbarDefinition
    {
        /// <inheritdoc />
        public override IEnumerable<string> ElementIds => new[] { GraphBreadcrumbs.id };
    }
}
