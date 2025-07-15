/* This file contains the example code for Context and Block nodes.
 * The regions are directly used in the documentation.
 * Please build and check the documentation when making changes.
 */

using System;
using Unity.GraphToolkit.Editor;

namespace Samples.DocCodeSamples.Editor
{
    class GtkContextNodeExamples
    {
        #region MyContextNode

        [Serializable]
        public class MyContextNode : ContextNode
        {
        }

        #endregion MyContextNode

        #region MyBlockNode

        [UseWithContext(typeof(MyContextNode))]
        [Serializable]
        public class MyBlockNode : BlockNode
        {
        }

        #endregion MyBlockNode

        [Serializable]
        public class MyOtherContextNode : ContextNode
        {
        }

        #region MyBlockNodeWithMultipleContexts

        [UseWithContext(typeof(MyContextNode), typeof(MyOtherContextNode))]
        [Serializable]
        public class MyBlockNodeWithMultipleContexts : BlockNode
        {
        }

        #endregion MyBlockNodeWithMultipleContexts
    }
}
