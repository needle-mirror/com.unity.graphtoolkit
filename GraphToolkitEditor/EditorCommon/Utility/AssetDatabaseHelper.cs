using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    [UnityRestricted]
    internal static class AssetDatabaseHelper
    {
        public static bool TryGetGUIDAndLocalFileIdentifier(Object obj, out GUID guid, out long localId)
        {
#if UGTK_ASSET_DATABASE_GUID
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out localId);
#else
            var success = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guidStr, out localId);
            GUID.TryParse(guidStr, out guid);
            return success;
#endif
        }
    }
}
