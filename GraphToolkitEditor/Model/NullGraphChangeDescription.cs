using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    class NullGraphChangeDescription : GraphChangeDescription
    {
        public NullGraphChangeDescription()
            : base(null, null, null) {}

        public override void Initialize(
            IEnumerable<Hash128> newModels,
            IReadOnlyCollection<KeyValuePair<Hash128, ChangeHintList>> changedModels,
            IEnumerable<Hash128> deletedModels)
        {
            throw new InvalidOperationException("Cannot initialize a null change description.");
        }
    }
}
