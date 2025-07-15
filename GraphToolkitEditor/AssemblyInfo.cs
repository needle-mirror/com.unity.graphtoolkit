using System.Runtime.CompilerServices;

// Dev project
[assembly: InternalsVisibleTo("GraphToolkitTestProject")]

// Tests
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Testing.Editor")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Internal.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor.Tests.Performance")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor.Tests.UI")]

// Samples (Todo: Should remove these when the public samples use the public api)
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.VisualNovelDirector.Editor")]

// Public API
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Editor.Tests")]

// Unity users
[assembly: InternalsVisibleTo("Unity.Motion.Editor")]
[assembly: InternalsVisibleTo("Unity.Motion.Editor.Tests")]
[assembly: InternalsVisibleTo("Unity.Motion.Hybrid.Tests")]
[assembly: InternalsVisibleTo("Unity.Motion.StateMachine.Tests")]

// Test Samples
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.BlackboardSample")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.ContextSample")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.ImportedGraphEditor")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.ItemLibrary")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.RecipesEditor")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.SimpleMathBook")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.StateMachine")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.TestSample")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.VerticalFlow")]
[assembly: InternalsVisibleTo("Unity.GraphToolkit.Samples.SampleSupport")]

// For the creation of wrapper types (search the assembly name to find the relevant code).
[assembly: InternalsVisibleTo("WrapperTypesAssembly")]
