using System.Collections.Generic;
using System.Linq;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base class for UI parts that display a list of <see cref="Model"/>s.
    /// </summary>
    /// <remarks>
    /// 'BaseMultipleModelViewsPart' is an abstract base class for parts that display a list of <see cref="Model"/> instances. It provides a structured way
    /// to manage and present multiple models within a part, which ensures consistency in how model lists are rendered and interacted with.
    /// </remarks>
    [UnityRestricted]
    internal abstract class BaseMultipleModelViewsPart : ModelViewPart
    {
        protected List<Model> m_Models;

        protected RootView OwnerRootView => m_OwnerElement?.RootView;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModelViewPart"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="models">The models displayed in this part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        protected BaseMultipleModelViewsPart(string name, IEnumerable<Model> models, ChildView ownerElement, string parentClassName) :
            base(name, ownerElement, parentClassName)
        {
            m_Models = models.ToList();
        }
    }
}
