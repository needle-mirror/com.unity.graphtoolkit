using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.GraphToolkit.Editor;

namespace Unity.GraphToolkit.ItemLibrary.Editor
{
    static class VisualElementsHelpers
    {
        // /// <summary>
        // /// Loads a stylesheet from resources and the appropriate variant for the current skin.
        // /// </summary>
        // /// <remarks>If the stylesheet name is Common.uss and <see cref="EditorGUIUtility.isProSkin"/> is <c>true</c>,
        // /// this method will load "Common.uss" and "Common_dark.uss". If <see cref="EditorGUIUtility.isProSkin"/> is <c>false</c>,
        // /// this method will load "Common.uss" and "Common_light.uss".
        // /// </remarks>
        // /// <param name="ve">The visual element onto which to attach the stylesheets.</param>
        // /// <param name="stylesheetName">The name of the stylesheet from resources.</param>
        public static void AddStylesheetResourceWithSkinVariant(this VisualElement ve, string stylesheetName)
        {
            if (ve == null)
                return;

            ve.AddStyleSheetPath(stylesheetName);
            ve.AddStyleSheetPath(GetFileNameWithSkinVariant(stylesheetName));
        }

        public static void AddStylesheetResource(this VisualElement ve, string stylesheetName)
        {
            ve.AddStyleSheetPath(stylesheetName);
        }

        /// <summary>
        /// Loads a stylesheet by path and the appropriate variant for the current skin.
        /// </summary>
        /// <remarks>If the stylesheet name is Common.uss and <see cref="EditorGUIUtility.isProSkin"/> is <c>true</c>,
        /// this method will load "Common.uss" and "Common_dark.uss". If <see cref="EditorGUIUtility.isProSkin"/> is <c>false</c>,
        /// this method will load "Common.uss" and "Common_light.uss".
        /// </remarks>
        /// <param name="ve">The visual element onto which to attach the stylesheets.</param>
        /// <param name="stylesheetAssetPath">The name of the common stylesheet.</param>
        public static void AddStylesheetAssetWithSkinVariant(this VisualElement ve, string stylesheetAssetPath)
        {
            if (ve == null)
                return;

            ve.AddStylesheetAssetPath(stylesheetAssetPath);
            ve.AddStylesheetAssetPath(GetFileNameWithSkinVariant(stylesheetAssetPath), true);
        }

        static void AddStylesheetAssetPath(this VisualElement ve, string stylesheetPath, bool ignoreFail = false)
        {
            if (ve == null)
                return;

            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylesheetPath);
            if (stylesheet != null)
            {
                ve.styleSheets.Add(stylesheet);
            }
            else if (!ignoreFail)
            {
                Debug.Log("Failed to load stylesheet " + stylesheetPath);
            }
        }

        /// <summary>
        /// Get the variant of a file name depending on the current theme (dark or light).
        /// </summary>
        /// <remarks>dir/filename.ext becomes dir/filename_dark.ext or dir/filename_light.ext</remarks>
        /// <param name="fileName">Original file name.</param>
        /// <returns>The filename to use for the current skin.</returns>
        static string GetFileNameWithSkinVariant(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var baseName = Path.ChangeExtension(fileName, null);
            var suffix = EditorGUIUtility.isProSkin ? "_dark" : "_light";
            return baseName + suffix + extension;
        }
    }
}
