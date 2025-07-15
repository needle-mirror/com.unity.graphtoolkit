using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    /// <summary>
    /// Helper class to handle library preferences per library tool.
    /// </summary>
    /// <remarks>Internal class for automated tests purposes.</remarks>
    class ItemLibraryPreferences
    {
        /// <summary>
        /// The data serialized in the EditorPrefs as json for each ItemLibrary tool.
        /// </summary>
        /// <remarks>Internal class for automated tests purposes.</remarks>
        [Serializable]
        [UnityRestricted]
        internal class DataPerTool
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataPerTool"/> class.
            /// </summary>
            public DataPerTool()
            {
                favoritesPerContext = new List<SerializableValue<List<string>>>();
                collapsedPerContext = new List<SerializableValue<List<string>>>();
                boolPrefs = new List<SerializableValue<bool>>();
                intPrefs = new List<SerializableValue<int>>();
                stringPrefs = new List<SerializableValue<string>>();
            }

            [Serializable]
            [UnityRestricted]
            internal class SerializableValue<T>
            {
                public string key;
                public T value;
            }

            public List<SerializableValue<List<string>>> favoritesPerContext;
            public List<SerializableValue<List<string>>> collapsedPerContext;
            public List<SerializableValue<bool>> boolPrefs;
            public List<SerializableValue<int>> intPrefs;
            public List<SerializableValue<string>> stringPrefs;

            public IReadOnlyList<string> GetFavorites(string context)
            {
                return Get(context, k_EmptyStringList, favoritesPerContext);
            }

            public IReadOnlyList<string> GetCollapsedItems(string context)
            {
                return Get(context, k_EmptyStringList, collapsedPerContext);
            }

            public void SetFavorite(string context, string itemPath, bool setFavorite = true)
            {
                var favorites = GetFavorites(context).ToList();
                if (setFavorite)
                    favorites.Add(itemPath);
                else
                    favorites.Remove(itemPath);
                Set(context, favorites, favoritesPerContext);
            }

            public void SetCollapsed(string context, string itemPath, bool setCollapsed = true)
            {
                var collapsedItems = GetCollapsedItems(context).ToList();
                if (setCollapsed)
                    collapsedItems.Add(itemPath);
                else
                    collapsedItems.Remove(itemPath);
                Set(context, collapsedItems, collapsedPerContext);
            }

            public bool GetBool(string key, bool defaultValue)
            {
                return Get(key, defaultValue, boolPrefs);
            }

            public void SetBool(string key, bool value)
            {
                Set(key, value, boolPrefs);
            }

            public int GetInt(string key, int defaultValue)
            {
                return Get(key, defaultValue, intPrefs);
            }

            public void SetInt(string key, int value)
            {
                Set(key, value, intPrefs);
            }

            public string GetString(string key, string defaultValue)
            {
                return Get(key, defaultValue, stringPrefs);
            }

            public void SetString(string key, string value)
            {
                Set(key, value, stringPrefs);
            }

            public T Get<T>(string key, T defaultValue, List<SerializableValue<T>> list)
            {
                var keyIndex = GetIndexForKey(key, list);
                if (keyIndex == -1)
                    return defaultValue;
                return list[keyIndex].value;
            }

            public void Set<T>(string key, T value, List<SerializableValue<T>> list)
            {
                var keyIndex = GetIndexForKey(key, list);
                var newTuple = new SerializableValue<T>() { key = key, value = value };
                if (keyIndex == -1)
                    list.Add(newTuple);
                else
                    list[keyIndex] = newTuple;
            }

            int GetIndexForKey<T>(string key, List<SerializableValue<T>> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].key == key)
                    {
                        return i;
                    }
                }
                return -1;
            }

            static readonly List<string> k_EmptyStringList = new List<string>();
        }

        /// <summary>
        /// The key used in <see cref="EditorPrefs"/> to store preferences.
        /// </summary>
        public string PreferenceKey => $"ItemLibraryPreferences.{ToolName}";

        /// <summary>
        /// The name of the ItemLibrary Tool accessing preferences.
        /// </summary>
        public string ToolName { get; }

        /// <summary>
        /// The name of the context in which the library was created.
        /// </summary>
        public string Context { get; }

        static Dictionary<string, DataPerTool> s_CachedPrefs = new Dictionary<string, DataPerTool>();

        /// <summary>
        /// Used in tests as tests delete their own EditorPrefs keys to cleanup.
        /// </summary>
        public static void InvalidateCache()
        {
            s_CachedPrefs = new Dictionary<string, DataPerTool>();
        }

        DataPerTool ToolPref
        {
            get => s_CachedPrefs[ToolName];
            set => s_CachedPrefs[ToolName] = value;
        }

        static readonly string k_PreviewTogglePrefName = "PreviewToggle";

        /// <summary>
        /// Gets or Sets the Visibility of the preview panel in the library.
        /// </summary>
        public bool PreviewVisibility
        {
            get => ToolPref.GetBool(k_PreviewTogglePrefName, false);
            set
            {
                ToolPref.SetBool(k_PreviewTogglePrefName, value);
                Save();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLibraryPreferences"/> class.
        /// </summary>
        /// <param name="toolName">The name of the ItemLibrary Tool accessing preferences.</param>
        /// <param name="context">The name of the context in which the library was created.</param>
        public ItemLibraryPreferences(string toolName, string context)
        {
            ToolName = toolName;
            Context = context;
            Load();
        }

        /// <summary>
        /// Get a list of all favorite items in the current tool and context.
        /// </summary>
        /// <returns>A list of all favorite items in the current tool and context by their path.</returns>
        public IReadOnlyList<string> GetFavorites()
        {
            return ToolPref.GetFavorites(Context);
        }

        /// <summary>
        /// Get a list of all items that should be collapsed in the current tool and context.
        /// </summary>
        /// <returns>A list of all collapsed items in the current tool and context by their path.</returns>
        public IReadOnlyList<string> GetCollapsedCategories()
        {
            return ToolPref.GetCollapsedItems(Context);
        }

        /// <summary>
        /// Adds or remove a favorite in the current tool and context.
        /// </summary>
        /// <param name="itemPath">The path of the item to be favorite</param>
        /// <param name="setFavorite">If true, adds a favorite. Removes from favorites otherwise.</param>
        public void SetFavorite(string itemPath, bool setFavorite)
        {
            ToolPref.SetFavorite(Context, itemPath, setFavorite);
            Save();
        }

        /// <summary>
        /// Adds or remove a collapsed item in the current tool and context.
        /// </summary>
        /// <param name="itemPath">The path of the item to be collapsed</param>
        /// <param name="setCollapsed">If true, set the item to collapsed. Removes from collapsed items otherwise.</param>
        public void SetCollapsed(string itemPath, bool setCollapsed)
        {
            ToolPref.SetCollapsed(Context, itemPath, setCollapsed);
            Save();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return ToolPref.GetBool(key, defaultValue);
        }

        public void SetBool(string key, bool value)
        {
            ToolPref.SetBool(key, value);
            Save();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return ToolPref.GetInt(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            ToolPref.SetInt(key, value);
            Save();
        }

        public string GetString(string key, string defaultValue = "")
        {
            return ToolPref.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            ToolPref.SetString(key, value);
            Save();
        }

        /// <summary>
        /// Clear all favorites for the current tool and context.
        /// </summary>
        public void ClearFavorites()
        {
            var favoritesPerContext = ToolPref.favoritesPerContext;
            favoritesPerContext.RemoveAll(f => f.key == Context);
            Save();
        }

        void Load()
        {
            if (!s_CachedPrefs.ContainsKey(ToolName))
            {
                ToolPref = RetrievePrefs(PreferenceKey);
            }
        }

        void Save()
        {
            var value = JsonUtility.ToJson(ToolPref);
            EditorPrefs.SetString(PreferenceKey, value);
        }

        public static DataPerTool RetrievePrefs(string preferenceKey)
        {
            if (EditorPrefs.HasKey(preferenceKey))
            {
                var prefStr = EditorPrefs.GetString(preferenceKey, "");
                var prefs = JsonUtility.FromJson<DataPerTool>(prefStr);
                if (prefs != null)
                    return prefs;
            }

            return new DataPerTool();
        }
    }
}
