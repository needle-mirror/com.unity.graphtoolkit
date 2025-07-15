using System;

namespace Unity.GraphToolkit.Editor.Implementation
{
    class BlackboardContentModelImp : BlackboardContentModel
    {
        public BlackboardContentModelImp(GraphTool graphTool) : base(graphTool)
        { }

        public override string GetTitle()
        {
            return (GraphModel as GraphModelImp)?.Graph.name ?? "Graph";
        }

        public override string GetSubTitle()
        {
            var subTitle =(GraphModel as GraphModelImp)?.Graph.GetType().Name;
            return subTitle != null ? $"({subTitle})" : String.Empty;
        }
    }
}
