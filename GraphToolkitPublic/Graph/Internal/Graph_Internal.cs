using System;
using Unity.GraphToolkit.Editor.Implementation;

namespace Unity.GraphToolkit.Editor
{
    public partial class Graph
    {
        internal GraphModelImp m_Implementation;

        internal void SetImplementation(GraphModelImp implementation)
        {
            m_Implementation = implementation;
        }

        internal void CheckImplementation()
        {
            if( m_Implementation == null )
            {
                throw new InvalidOperationException("Only Graph instances returned by either GraphDatabase.LoadGraph or GraphDatabase.CreateGraph are valid.");
            }
        }
    }
}
