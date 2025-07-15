using System;
using Unity.GraphToolkit.ItemLibrary.Editor;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Interface to provide custom data in a <see cref="ItemLibraryItem"/>
    /// </summary>
    interface IItemLibraryDataProvider
    {
        IItemLibraryData Data { get; }
    }
}
