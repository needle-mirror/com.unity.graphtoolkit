using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor.Implementation;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    public abstract partial class Node
    {
        [NonSerialized]
        internal AbstractNodeModel m_Implementation;

        internal class PortDefinitionContext : IPortDefinitionContext
        {
            class PortBuilder :
                IOutputPortBuilder,
                IInputPortBuilder,
                ITypedInputPortBuilder,
                ITypedOutputPortBuilder
            {
                PortDefinitionContext m_PortsDefinitionContext;

                string m_PortName;
                PortDirection m_Direction;

                string m_DisplayName;
                PortOrientation m_Orientation;
                PortConnectorUI m_ConnectorUI;
                List<Attribute> m_Attributes = new ();

                internal Type portType;
                internal object defaultValue;

                internal object typedBuilder;

                internal void Reset()
                {
                    m_PortName = null;
                    m_Direction = PortDirection.None;

                    m_DisplayName = null;
                    portType = null;
                    defaultValue = null;
                    m_Orientation = PortOrientation.Horizontal;
                    m_ConnectorUI = PortConnectorUI.Circle;
                    m_Attributes.Clear();
                    typedBuilder = null;
                }

                internal IInputPortBuilder AddInputPort(PortDefinitionContext portsDefinition, string portName)
                {
                    m_PortsDefinitionContext = portsDefinition;
                    m_PortName = portName;
                    m_Direction = PortDirection.Input;
                    return this;
                }

                internal IOutputPortBuilder AddOutputPort(PortDefinitionContext portsDefinition, string portName)
                {
                    m_PortsDefinitionContext = portsDefinition;
                    m_PortName = portName;
                    m_Direction = PortDirection.Output;
                    return this;
                }

                IOutputPortBuilder IPortBuilder<IOutputPortBuilder>.WithDisplayName(string displayName) => WithDisplayName(displayName);
                ITypedOutputPortBuilder IPortBuilder<ITypedOutputPortBuilder>.WithDisplayName(string displayName) => WithDisplayName(displayName);
                IInputPortBuilder IPortBuilder<IInputPortBuilder>.WithDisplayName(string displayName) => WithDisplayName(displayName);
                ITypedInputPortBuilder IPortBuilder<ITypedInputPortBuilder>.WithDisplayName(string displayName) => WithDisplayName(displayName);

                IOutputPortBuilder IPortBuilder<IOutputPortBuilder>.WithConnectorUI(PortConnectorUI connectorUI) => WithConnectorUI(connectorUI);
                ITypedOutputPortBuilder IPortBuilder<ITypedOutputPortBuilder>.WithConnectorUI(PortConnectorUI connectorUI) => WithConnectorUI(connectorUI);
                IInputPortBuilder IPortBuilder<IInputPortBuilder>.WithConnectorUI(PortConnectorUI connectorUI) => WithConnectorUI(connectorUI);
                ITypedInputPortBuilder IPortBuilder<ITypedInputPortBuilder>.WithConnectorUI(PortConnectorUI connectorUI) => WithConnectorUI(connectorUI);

                IInputPortBuilder IInputBasePortBuilder<IInputPortBuilder>.Delayed() => Delayed();
                ITypedInputPortBuilder IInputBasePortBuilder<ITypedInputPortBuilder>.Delayed() => Delayed();

                IOutputPortBuilder IOutputPortBuilder.WithDataType(Type portType) => WithDataType(portType);
                IOutputPortBuilder<T> IOutputPortBuilder.WithDataType<T>() => WithDataType<T>();

                ITypedInputPortBuilder IInputPortBuilder.WithDataType(Type portType) => WithDataType(portType);

                IInputPortBuilder<T> IInputPortBuilder.WithDataType<T>() => WithDataType<T>();

                ITypedInputPortBuilder ITypedInputPortBuilder.WithDefaultValue(object defaultValue) => WithDefaultValue(defaultValue);

                public PortBuilder WithDisplayName(string displayName)
                {
                    this.m_DisplayName = displayName;
                    return this;
                }

                PortBuilder WithDataType(Type portType)
                {
                    this.portType = portType;

                    return this;
                }

                PortBuilder<T> WithDataType<T>()
                {
                    WithDataType(typeof(T));
                    return m_PortsDefinitionContext.GetFreeTypedBuilder<T>(this);
                }

                PortBuilder WithDefaultValue(object defaultValue)
                {
                    if( portType != null && defaultValue != null && ! portType.IsAssignableFrom(defaultValue.GetType()) )
                    {
                        throw new ArgumentException($"Default value type {defaultValue} is not assignable to port type {portType}");
                    }
                    this.defaultValue = defaultValue;
                    return this;
                }

                public PortBuilder WithOrientation(PortOrientation orientation)
                {
                    m_Orientation = orientation;
                    return this;
                }

                public PortBuilder WithConnectorUI(PortConnectorUI connectorUI)
                {
                    m_ConnectorUI = connectorUI;
                    return this;
                }

                public PortBuilder Delayed()
                {
                    if(!m_Attributes.Any(t => t is DelayedAttribute))
                        m_Attributes.Add(new DelayedAttribute());
                    return this;
                }

                public IPort Build()
                {
                    IPort result;

                    var attributesArray = m_Attributes.Count > 0 ? m_Attributes.ToArray() : null;
                    if (m_Direction == PortDirection.Input)
                        result = m_PortsDefinitionContext.portsDefinition.AddInputPort(m_DisplayName ?? m_PortName, portType, m_PortName, m_Orientation, attributesArray, defaultValue);
                    else
                        result = m_PortsDefinitionContext.portsDefinition.AddOutputPort(m_DisplayName ?? m_PortName, portType, m_PortName, m_Orientation, attributesArray);

                    if( result is PortModelImp portModel )
                    {
                        portModel.ConnectorUI = m_ConnectorUI;
                    }
                    m_PortsDefinitionContext.ReleaseBuilder(this);

                    return result;
                }
            }

            class PortBuilder<TData> : IOutputPortBuilder<TData>, IInputPortBuilder<TData>
            {
                public PortBuilder parent;

                IOutputPortBuilder<TData> IPortBuilder<IOutputPortBuilder<TData>>.WithDisplayName(string displayName) => WithDisplayName(displayName);
                IInputPortBuilder<TData> IPortBuilder<IInputPortBuilder<TData>>.WithDisplayName(string displayName) => WithDisplayName(displayName);

                IOutputPortBuilder<TData> IPortBuilder<IOutputPortBuilder<TData>>.WithConnectorUI(PortConnectorUI connectorUI) => WithConnectorUI(connectorUI);
                IInputPortBuilder<TData> IPortBuilder<IInputPortBuilder<TData>>.WithConnectorUI(PortConnectorUI connectorUI) => WithConnectorUI(connectorUI);

                IInputPortBuilder<TData> IInputBasePortBuilder<IInputPortBuilder<TData>>.Delayed() => Delayed();

                IInputPortBuilder<TData> IInputPortBuilder<TData>.WithDefaultValue(TData defaultValue) => WithDefaultValue(defaultValue);

                PortBuilder<TData> WithDisplayName(string displayName)
                {
                    parent.WithDisplayName(displayName);
                    return this;
                }

                PortBuilder<TData> WithOrientation(PortOrientation orientation)
                {
                    parent.WithOrientation(orientation);
                    return this;
                }

                PortBuilder<TData> WithConnectorUI(PortConnectorUI connectorUI)
                {
                    parent.WithConnectorUI(connectorUI);
                    return this;
                }

                PortBuilder<TData> Delayed()
                {
                    parent.Delayed();
                    return this;
                }

                PortBuilder<TData> WithDefaultValue(TData defaultValue)
                {
                    parent.defaultValue = defaultValue;
                    return this;
                }

                public IPort Build()
                {
                    return parent.Build();
                }
            }

            public IPortsDefinition portsDefinition;

            List<PortBuilder> m_PortBuilderPool = new ();

            Dictionary<Type, List<object>> m_TypedPortBuilderPools = new ();

            List<PortBuilder> m_UsedPortBuilder = new List<PortBuilder>();

            PortBuilder GetFreeBuilder()
            {
                PortBuilder builder;
                if(m_PortBuilderPool.Count > 0)
                {
                    builder = m_PortBuilderPool[^1];
                    m_PortBuilderPool.RemoveAt(m_PortBuilderPool.Count - 1);
                }
                else
                {
                    builder = new PortBuilder(); //TODO : pool
                }
                m_UsedPortBuilder.Add(builder);
                return builder;
            }

            void ReleaseBuilder(PortBuilder builder)
            {
                if (builder == null)
                    return;

                if (builder.typedBuilder != null)
                {
                    ReleaseTypedBuilder(builder.portType, builder.typedBuilder);
                }

                if (m_UsedPortBuilder.Remove(builder))
                {
                    m_PortBuilderPool.Add(builder);
                    builder.Reset();
                }
            }

            PortBuilder<T> GetFreeTypedBuilder<T>(PortBuilder parent)
            {
                PortBuilder<T> result;
                if (! m_TypedPortBuilderPools.TryGetValue(typeof(T), out var builderPool) || builderPool.Count == 0)
                {
                    builderPool = new List<object>();
                    m_TypedPortBuilderPools[typeof(T)] = builderPool;

                     result = new PortBuilder<T>();
                }
                else
                {
                    result = (PortBuilder<T>)builderPool[^1];
                    builderPool.RemoveAt(builderPool.Count - 1);
                }
                parent.typedBuilder = result;
                result.parent = parent;
                return result;
            }

            void ReleaseTypedBuilder(Type type, object typedBuilder)
            {
                m_TypedPortBuilderPools[type].Add(typedBuilder);
            }

            public IInputPortBuilder AddInputPort(string portName)
            {
                return GetFreeBuilder().AddInputPort(this, portName);
            }
            public IOutputPortBuilder AddOutputPort(string portName)
            {
                return GetFreeBuilder().AddOutputPort(this, portName);
            }

            public void Finish()
            {
                while (m_UsedPortBuilder.Count > 0)
                {
                    m_UsedPortBuilder[0].Build();
                }
            }
        }

        static PortDefinitionContext s_PortDefinitionContext = new ();

        internal void CallOnDefineNode(IPortsDefinition context)
        {
            s_PortDefinitionContext.portsDefinition = context;
            OnDefinePorts(s_PortDefinitionContext);
            s_PortDefinitionContext.Finish();
        }

        internal void CallOnDefineOptions(INodeOptionDefinition context)
        {
            OnDefineOptions(context);
        }

        internal void SetImplementation(AbstractNodeModel implementation)
        {
            m_Implementation = implementation;
        }
    }
}
