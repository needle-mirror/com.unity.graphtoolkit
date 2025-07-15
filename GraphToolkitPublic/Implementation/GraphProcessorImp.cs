namespace Unity.GraphToolkit.Editor.Implementation
{
    class ErrorsAndWarningsImp : ErrorsAndWarningsResult, IErrorsAndWarnings
    {
        Model m_DefaultModel;
        public ErrorsAndWarningsImp(Model defaultModel)
        {
            m_DefaultModel = defaultModel;
        }

        AbstractNodeModel GetNodeModel(object context)
        {
            if (context is Node userNode)
                return userNode.m_Implementation;
            if (context is INode node)
                return (AbstractNodeModel)node;
            return null;
        }

        void IErrorsAndWarnings.LogError(object message, object context)
        {
            AddError(message.ToString(), GetNodeModel(context) ?? m_DefaultModel);
        }
        void IErrorsAndWarnings.LogWarning(object message, object context)
        {
            AddWarning(message.ToString(), GetNodeModel(context) ?? m_DefaultModel);
        }
        void IErrorsAndWarnings.Log(object message, object context)
        {
            AddMessage(message.ToString(), GetNodeModel(context) ?? m_DefaultModel);
        }
    }

    class GraphProcessorImp : GraphProcessor
    {
        GraphModelImp m_GraphModel;

        public GraphProcessorImp(GraphModelImp graphModel)
        {
            m_GraphModel = graphModel;
        }

        public override BaseGraphProcessingResult ProcessGraph(Unity.GraphToolkit.Editor.GraphChangeDescription changes)
        {
            var result = new ErrorsAndWarningsImp(m_GraphModel);

            var graphChanges = new GraphLogger();
            graphChanges.errorsAndWarnings = result;
            m_GraphModel.Graph.OnGraphChanged(graphChanges);

            return result;
        }
    }
}
