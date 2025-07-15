using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface for node models that own a <see cref="DeclarationModel"/>.
    /// </summary>
    [UnityRestricted]
    internal interface IHasDeclarationModel
    {
        /// <summary>
        /// The declaration model.
        /// </summary>
        DeclarationModel DeclarationModel { get; }

        /// <summary>
        /// Sets the declaration model.
        /// </summary>
        void SetDeclarationModel(DeclarationModel value);
    }
}
