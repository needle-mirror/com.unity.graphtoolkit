using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    [Serializable]
    [UnityRestricted]
    internal struct SubgraphAssetProperty
    {
        [SerializeField]
        GraphReference m_SubgraphReference;

        public GraphReference SubgraphReference => m_SubgraphReference;

        public SubgraphAssetProperty(GraphReference value)
        {
            m_SubgraphReference = value;
        }
    }
}
