using System;
using Unity.GraphToolkit.CSO;
using Unity.GraphToolkit.ItemLibrary.Editor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Holds information about <see cref="ItemLibraryWindow"/> dimensions.
    /// </summary>
    [Serializable]
    [UnityRestricted]
    internal struct ItemLibrarySize
    {
        public static readonly ItemLibrarySize DefaultItemLibrarySize = new ItemLibrarySize { Size = new Vector2(500, 400), RightLeftRatio = 1.0f };

        public Vector2 Size;
        public float RightLeftRatio;
    }

    /// <summary>
    /// Extension methods for <see cref="ItemLibrarySize"/>.
    /// </summary>
    [UnityRestricted]
    internal static class ItemLibrarySizeExtensions
    {
        static SerializedValueDictionary<string, ItemLibrarySize> GetSizes(Preferences preferences)
        {
            SerializedValueDictionary<string, ItemLibrarySize> sizes = null;
            var valueString = preferences.GetString(StringPref.ItemLibrarySize);
            if (valueString != null)
            {
                sizes = JsonUtility.FromJson<SerializedValueDictionary<string, ItemLibrarySize>>(valueString);
            }

            sizes ??= new SerializedValueDictionary<string, ItemLibrarySize>();
            return sizes;
        }

        static void SaveSizes(Preferences preferences, SerializedValueDictionary<string, ItemLibrarySize> sizes)
        {
            if (sizes != null)
            {
                var valueString = JsonUtility.ToJson(sizes);
                preferences.SetString(StringPref.ItemLibrarySize, valueString);
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemLibraryWindow"/> rect and left right ratio for a context.
        /// </summary>
        /// <param name="preferences">The object that contains size information.</param>
        /// <param name="contextName">The name of the context in which we use the library.</param>
        internal static ItemLibrarySize GetItemLibrarySize(this Preferences preferences, string contextName)
        {
            var sizes = GetSizes(preferences);
            if (string.IsNullOrEmpty(contextName) || !sizes.TryGetValue(contextName, out var size))
            {
                if (!sizes.TryGetValue("", out size))
                {
                    size = ItemLibrarySize.DefaultItemLibrarySize;
                }
            }
            if (size.Size.x < ItemLibraryWindow.MinSize.x)
                size.Size.x = ItemLibraryWindow.MinSize.x;
            if (size.Size.y < ItemLibraryWindow.MinSize.y)
                size.Size.y = ItemLibraryWindow.MinSize.y;

            if (size.RightLeftRatio > 10.0f)
                size.RightLeftRatio = 10.0f;
            if (size.RightLeftRatio < 0.1f)
                size.RightLeftRatio = 0.1f;

            return size;
        }

        /// <summary>
        /// Sets default <see cref="ItemLibraryWindow"/> rect and left right ratio for a context.
        /// </summary>
        /// <param name="preferences">The object that contains size information.</param>
        /// <param name="contextName">The name of the context for which to set the size. Set to <c>null</c> to set the default for every context.</param>
        /// <param name="size">The size of the window.</param>
        /// <param name="rightLeftRatio">The ratio between the left panel and right (details) panel.</param>
        internal static void SetItemLibrarySize(this Preferences preferences, string contextName, Vector2 size, float rightLeftRatio = 1.0f)
        {
            contextName ??= "";

            var sizes = GetSizes(preferences);
            if (sizes.TryGetValue(contextName, out var currentSize))
            {
                if (currentSize.Size == size && currentSize.RightLeftRatio == rightLeftRatio)
                {
                    return;
                }
            }

            sizes[contextName] = new ItemLibrarySize { Size = size, RightLeftRatio = rightLeftRatio };

            SaveSizes(preferences, sizes);
        }

        /// <summary>
        /// Sets <see cref="ItemLibraryWindow"/> rect and left right ratio for a context, if it is not already set.
        /// </summary>
        /// <param name="preferences">The object that contains size information.</param>
        /// <param name="contextName">The name of the context for which to set the size. Set to <c>null</c> to set the default for every context.</param>
        /// <param name="size">The size of the window.</param>
        /// <param name="rightLeftRatio">The ratio between the left panel and right (details) panel.</param>
        public static void SetInitialItemLibrarySize(this Preferences preferences, string contextName, Vector2 size, float rightLeftRatio = 1.0f)
        {
            contextName ??= "";

            var sizes = GetSizes(preferences);
            if (!sizes.TryGetValue(contextName, out _))
            {
                sizes[contextName] = new ItemLibrarySize { Size = size, RightLeftRatio = rightLeftRatio };
                SaveSizes(preferences, sizes);
            }
        }

        internal static void ResetItemLibrarySizes(this Preferences preferences)
        {
            var sizes = GetSizes(preferences);
            sizes.Clear();
            SaveSizes(preferences, sizes);
        }
    }
}
