using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A model to hold the result of the graph processing.
    /// </summary>
    [UnityRestricted]
    internal class GraphProcessingErrorModel : ErrorMarkerModel
    {
        /// <summary>
        /// The GUID of the model to which the error is associated with.
        /// </summary>
        public Hash128 ParentModelGuid { get; }

        /// <summary>
        /// The graph reference of the model to which the error is associated with.
        /// </summary>
        public GraphReference SourceGraphReference { get; }

        /// <summary>
        /// The context of the error.
        /// </summary>
        /// <remarks>A context is a path of models to the source of the error. The last element of the list is the source of the error.</remarks>
        public IReadOnlyList<GraphElementModel> Context { get; }

        /// <inheritdoc />
        public override LogType ErrorType { get; }

        /// <inheritdoc />
        public override string ErrorMessage { get; }

        /// <summary>
        /// The <see cref="QuickFix"/> for the error.
        /// </summary>
        public virtual QuickFix Fix { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphProcessingErrorModel" /> class.
        /// </summary>
        /// <param name="error">The <see cref="GraphProcessingError"/> used to initialize the instance.</param>
        public GraphProcessingErrorModel(GraphProcessingError error)
        {
            ParentModelGuid = error.SourceModelGuid;
            ErrorMessage = error.Description;
            ErrorType = error.ErrorType;
            Fix = error.Fix;
            SourceGraphReference = error.SourceGraphReference;
            Context = error.Context;
        }

        /// <inheritdoc />
        public override GraphElementModel GetParentModel(GraphModel graphModel)
        {
            return graphModel.GetModel(ParentModelGuid);
        }
    }
}
