using System.Runtime.CompilerServices;

// Dev project
[assembly: InternalsVisibleTo("GraphToolkitTestProject")]

// Other package assemblies
[assembly: InternalsVisibleTo("Unity.CommandStateObserver")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Utility.Editor")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Internal.Editor")]

// GTF Tests
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Testing.Editor")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Internal.Tests")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Internal.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor.Tests.Performance")]
