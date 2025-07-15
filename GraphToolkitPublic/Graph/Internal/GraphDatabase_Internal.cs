using Unity.GraphToolkit.Editor.Implementation;

namespace Unity.GraphToolkit.Editor
{
    public static partial class GraphDatabase
    {
        static GraphDatabase()
        {
            PublicGraphFactory.EnsureStaticConstructorIsCalled();
        }
    }
}
