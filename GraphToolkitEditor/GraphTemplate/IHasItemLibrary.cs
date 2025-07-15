using System;

namespace Unity.GraphToolkit.Editor
{
    [UnityRestricted]
    internal interface IHasItemLibrary
    {
        ItemLibraryHelper GetItemLibraryHelper();
    }
}
