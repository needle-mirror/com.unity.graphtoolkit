using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using InstanceID = System.Int32;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// A reference to a <see cref="GraphModel"/>.
    /// </summary>
    /// <remarks> This struct is designed to hold references to graph models.
    /// It can be used to uniquely identify and resolve graph models and their associated assets.
    /// The <see cref="GraphReference"/> can be initialized with either a <see cref="GraphModel"/> instance or
    /// directly with the GUIDs of the graph model and its asset. It provides methods to resolve these references
    /// to their actual objects.
    /// </remarks>
    [Serializable]
    [UnityRestricted]
    internal struct GraphReference : IEquatable<GraphReference>
    {
        public const InstanceID InstanceIDNone = 0;

        [SerializeField]
        Hash128 m_AssetGuidAsHash;

        [SerializeField]
        Hash128 m_GraphModelGuid;

        [SerializeField]
        InstanceID m_GraphObjectInstanceID;

        /// <summary>
        /// The GUID of the asset referenced by this <see cref="GraphReference"/>.
        /// </summary>
        public readonly GUID AssetGuid => Hash128Helpers.ToGUID(m_AssetGuidAsHash);

        /// <summary>
        /// The GUID of the GraphModel referenced by this <see cref="GraphReference"/>.
        /// </summary>
        public Hash128 GraphModelGuid => m_GraphModelGuid;

        /// <summary>
        /// The InstanceID of the GraphObject if AssetGuid == default.
        /// </summary>
        public InstanceID graphObjectInstanceID => m_GraphObjectInstanceID;

        /// <summary>
        /// The path of the asset referenced by this <see cref="GraphReference"/>.
        /// </summary>
        public string FilePath => AssetDatabase.GUIDToAssetPath(AssetGuid);


        /// <summary>
        /// True if the reference has an asset reference. False otherwise.
        /// </summary>
        public bool HasAssetReference => AssetGuid != default || m_GraphObjectInstanceID != InstanceIDNone;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphReference"/> struct from a <see cref="GraphModel"/>.
        /// </summary>
        /// <param name="graphModel">The <see cref="GraphModel"/> this reference points to.</param>
        /// <remarks>
        /// When the <see cref="GraphModel"/> is null, the GUIDs of the graph model and its asset will be initialized
        /// with default values. If only the asset is null, only its GUID will be initialized with a default value.
        /// </remarks>
        public GraphReference(GraphModel graphModel)
        {
            m_AssetGuidAsHash = default;
            m_GraphObjectInstanceID = InstanceIDNone;
            if (graphModel == null)
            {
                m_GraphModelGuid = default;
                return;
            }

            if (graphModel.GraphObject != null)
            {
                m_AssetGuidAsHash = Hash128Helpers.FromGUID(graphModel.GraphObject?.AssetFileGuid ?? default);
                if( m_AssetGuidAsHash == default)
                    m_GraphObjectInstanceID = graphModel.GraphObject.GetInstanceID();
            }
            m_GraphModelGuid = graphModel.Guid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphReference"/> struct using the provided GUID of a graph model, and either the graph object asset GUID
        /// or graph object instance id.
        /// </summary>
        /// <param name="graphModelGuid">The GUID of the <see cref="GraphModel"/>. </param>
        /// <param name="assetGuid">The GUID of the <see cref="GraphObject"/> asset. </param>
        /// <param name="graphObjectInstanceID">The instance id of the <see cref="GraphObject"/> used if the object is not stored in an asset.</param>
        public GraphReference(Hash128 graphModelGuid, GUID assetGuid, InstanceID graphObjectInstanceID = InstanceIDNone)
        {
            m_GraphModelGuid = graphModelGuid;
            m_AssetGuidAsHash = Hash128Helpers.FromGUID(assetGuid);
            m_GraphObjectInstanceID = m_AssetGuidAsHash == default ? graphObjectInstanceID : InstanceIDNone;
        }

        /// <summary>
        /// Resolve the graph reference to the actual graph model.
        /// </summary>
        /// <remarks>This function will first attempt to resolve the corresponding <see cref="GraphObject"/>. If the GraphObject
        /// can't be resolved from the GUIDs, it will try to rebind the graph object using the instance ID.</remarks>
        public static GraphModel ResolveGraphModel(in GraphReference graphReference)
        {
            var graphObject = graphReference.ResolveAsset();

            // Try to rebind the graph object using the instance ID if the object is not loaded.
            if (graphObject == null && graphReference.m_GraphObjectInstanceID != InstanceIDNone)
            {
#if UNITY_6000_3_OR_NEWER
                graphObject = EditorUtility.EntityIdToObject(graphReference.m_GraphObjectInstanceID) as GraphObject;
#else
                graphObject = EditorUtility.InstanceIDToObject(graphReference.m_GraphObjectInstanceID) as GraphObject;
#endif
            }
            return graphObject?.GetGraphModelByGuid(graphReference.m_GraphModelGuid);
        }

        readonly GraphObject ResolveAsset()
        {
            if (AssetGuid != default)
            {
                var filePath = AssetDatabase.GUIDToAssetPath(AssetGuid);
                var asset = GraphObject.LoadGraphObjectAtPath(filePath);

                if (asset == null)
                    return null;

                if (asset.GraphModel?.Guid == m_GraphModelGuid)
                    return asset;

                // GTF-1760: This could be a previous version of the graph object with a local subgraph as their own asset.
                // This code is only for migration, it searches for the correct graph model.
                var assets = AssetDatabase.LoadAllAssetsAtPath(filePath);
                foreach (var subAsset in assets)
                {
                    if( subAsset is not GraphObject subGraphAsset)
                        continue;
                    // We want to load an asset with the same guid and localId
                    if (subGraphAsset.GraphModel?.Guid == m_GraphModelGuid)
                    {
                        return subGraphAsset;
                    }
                }

                if (asset != null)
                {
                    return asset;
                }
            }
            else if( m_GraphObjectInstanceID != InstanceIDNone)
            {
#if UNITY_6000_3_OR_NEWER
                return EditorUtility.EntityIdToObject(m_GraphObjectInstanceID) as GraphObject;
#else
                return EditorUtility.InstanceIDToObject(m_GraphObjectInstanceID) as GraphObject;
#endif
            }

            return null;
        }

        /// <summary>
        /// Returns true if the reference refers to the given asset. False otherwise.
        /// </summary>
        /// <param name="assetGuid">The GUID of the asset to be tested.</param>
        /// <returns>true if the reference refers to the given asset. False otherwise.</returns>
        public bool RefersToFile(GUID assetGuid)
        {
            return AssetGuid == assetGuid;
        }

        /// <inheritdoc />
        public bool Equals(GraphReference other)
        {
            if (m_GraphObjectInstanceID == default && other.m_GraphObjectInstanceID == default &&
                AssetGuid == default && other.AssetGuid == default &&
                m_GraphModelGuid == default && other.m_GraphModelGuid == default)
            {
                return true;
            }

            if (m_GraphObjectInstanceID != default && other.m_GraphObjectInstanceID != default)
            {
                return m_GraphModelGuid == other.m_GraphModelGuid &&
                    m_GraphObjectInstanceID.Equals(other.m_GraphObjectInstanceID);
            }

            if (AssetGuid != default && other.AssetGuid != default)
            {
                return m_GraphModelGuid == other.m_GraphModelGuid && AssetGuid == other.AssetGuid;
            }

            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is not GraphReference gr) return false;
            return Equals(gr);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return HashCode.Combine(AssetGuid, m_GraphObjectInstanceID, m_GraphModelGuid);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <summary>
        /// Compares two <see cref="GraphReference"/> objects for equality.
        /// </summary>
        /// <param name="left">Left <see cref="GraphReference"/>.</param>
        /// <param name="right">Right <see cref="GraphReference"/>.</param>
        /// <returns>True if they are equal. False otherwise.</returns>
        public static bool operator ==(GraphReference left, GraphReference right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two <see cref="GraphReference"/> objects for inequality.
        /// </summary>
        /// <param name="left">Left <see cref="GraphReference"/>.</param>
        /// <param name="right">Right <see cref="GraphReference"/>.</param>
        /// <returns>True if they are different. False otherwise.</returns>
        public static bool operator !=(GraphReference left, GraphReference right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Type: {GetType()}\n\tInstanceID: {m_GraphObjectInstanceID}\n\tGraphModelGuid: {m_GraphModelGuid}\n\tPath: {AssetDatabase.GUIDToAssetPath(AssetGuid)}";
        }
    }
}
