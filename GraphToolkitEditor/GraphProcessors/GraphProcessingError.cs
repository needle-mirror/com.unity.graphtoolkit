using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Definition of a graph processing error.
    /// </summary>
    [UnityRestricted]
    internal class GraphProcessingError : IEquatable<GraphProcessingError>
    {
        /// <summary>
        /// Description of the error.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Unique ID of the model that is the source of the error.
        /// </summary>
        public Hash128 SourceModelGuid { get; }

        /// <summary>
        /// The context of the error.
        /// </summary>
        /// <remarks>A context is a path of models to the source of the error. The last element of the list is the source of the error.</remarks>
        public IReadOnlyList<GraphElementModel> Context { get; }

        /// <summary>
        /// The reference to the graph that is the source of the error.
        /// </summary>
        public GraphReference SourceGraphReference { get; }

        /// <summary>
        /// QuickFix to address the error.
        /// </summary>
        public QuickFix Fix { get; }

        /// <summary>
        /// The error type.
        /// </summary>
        public LogType ErrorType { get; }

        internal GraphProcessingError() { }

        public GraphProcessingError(string description, Hash128 sourceModelGuid, LogType errorType, GraphReference sourceGraphReference, IReadOnlyList<GraphElementModel> context, QuickFix fix = null)
            : this()
        {
            Description = description;
            SourceModelGuid = sourceModelGuid;
            ErrorType = errorType;
            SourceGraphReference = sourceGraphReference;
            Context = context;
            Fix = fix;
        }

        /// <summary>
        /// Returns a string that represents the current error.
        /// </summary>
        /// <returns>A string that represents the current error.</returns>
        public override string ToString()
        {
            return $"Graph Processing Error: {Description}";
        }

        /// <inheritdoc />
        public bool Equals(GraphProcessingError other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            var isEquals =
                Description == other.Description &&
                SourceGraphReference == other.SourceGraphReference &&
                SourceModelGuid == other.SourceModelGuid &&
                ErrorType == other.ErrorType &&
                Equals(Context, other.Context);

            return isEquals;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GraphProcessingError)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ SourceGraphReference.GetHashCode();
                hashCode = (hashCode * 397) ^ SourceModelGuid.GetHashCode();
                hashCode = (hashCode * 397) ^ ErrorType.GetHashCode();
                return hashCode;
            }
        }
    }
}
