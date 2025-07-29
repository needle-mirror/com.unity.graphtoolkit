/* This code is used in the documentation.
 * Please build and check the documentation when making changes.
 */
using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

class GtkNodeExamples
{
    #region BasicNode
    [Serializable]
    public class BasicNode : Node
    {
    }
    #endregion

    public class BasicNodeWithPorts : Node
    {
        #region BasicPorts
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("Input").Build();
            context.AddInputPort<int>("a").Build();

            context.AddOutputPort("Output").Build();
            context.AddOutputPort<int>("result").Build();
        }
        #endregion
    }

    #region BasicNodeComplete
    [Serializable]
    public class BasicNodeFinal : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("Input").Build();
            context.AddInputPort<int>("a").Build();

            context.AddOutputPort("Output").Build();
            context.AddOutputPort<int>("result").Build();
        }
    }
    #endregion

    public class NodeWithPortCount : Node
    {
        #region PortCountOption
        const string k_PortCountName = "PortCount";

        protected override void OnDefineOptions(INodeOptionDefinition context)
        {
            context.AddNodeOption<int>(k_PortCountName, "Port Count",
                defaultValue: 2, attributes: new [] { new DelayedAttribute() });
        }
        #endregion

        #region PortCountPorts
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var portCountOption = GetNodeOptionByName(k_PortCountName);
            portCountOption.TryGetValue<int>(out var portCount);
            for (var i = 0; i < portCount; i++)
            {
                context.AddInputPort<Vector2>($"{i}").Build();
            }

            context.AddOutputPort<Vector2>("result").Build();
        }
        #endregion
    }

    public class NodeWithPortTypeOption : Node
    {
        #region PortTypeOption
        enum PortTypes
        {
            Vec2,
            Vec3
        }

        const string k_PortTypeName = "PortType";

        protected override void OnDefineOptions(INodeOptionDefinition context)
        {
            context.AddNodeOption<PortTypes>(k_PortTypeName, "Port Type");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var portTypeOption = GetNodeOptionByName(k_PortTypeName);
            portTypeOption.TryGetValue<PortTypes>(out var portType);

            if (portType == PortTypes.Vec3)
            {
                context.AddOutputPort<Vector3>("result").Build();
            }
            else
            {
                context.AddOutputPort<Vector2>("result").Build();
            }
        }
        #endregion
    }

    #region NodeWithOptions
    [Serializable]
    public class NodeWithOptions : Node
    {
        enum PortTypes
        {
            Vec2,
            Vec3
        }

        const string k_PortCountName = "PortCount";
        const string k_PortTypeName = "PortType";

        protected override void OnDefineOptions(INodeOptionDefinition context)
        {
            context.AddNodeOption<int>(k_PortCountName,"Port Count",
                defaultValue: 2, attributes: new [] { new DelayedAttribute() });
            context.AddNodeOption<PortTypes>(k_PortTypeName, "Port Type");
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var portCountOption = GetNodeOptionByName(k_PortCountName);
            portCountOption.TryGetValue<int>(out var portCount);
            for (var i = 0; i < portCount; i++)
            {
                context.AddInputPort<Vector2>($"{i}").Build();
            }

            var portTypeOption = GetNodeOptionByName(k_PortTypeName);
            portTypeOption.TryGetValue<PortTypes>(out var portType);

            if (portType == PortTypes.Vec3)
            {
                context.AddOutputPort<Vector3>("result").Build();
            }
            else
            {
                context.AddOutputPort<Vector2>("result").Build();
            }
        }
    }
    #endregion
}
