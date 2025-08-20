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

            List<PortBuilder> m_UsedPortBuilder = new();

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

        internal class OptionDefinitionContext : IOptionDefinitionContext
        {
            class OptionBuilder : IOptionBuilder
            {
                OptionDefinitionContext m_OptionsDefinitionContext;

                string m_OptionName;
                string m_DisplayName;
                string m_Tooltip;
                int m_Order;
                List<Attribute> m_Attributes = new ();
                bool m_ShowInInspectorOnly;
                object m_DefaultValue;

                internal Type OptionType;
                internal object TypedBuilder;

                internal void Reset()
                {
                    m_OptionName = null;
                    m_DisplayName = null;
                    m_Tooltip = null;
                    m_Order = 0;
                    m_Attributes.Clear();
                    OptionType = null;
                    m_DefaultValue = null;
                    TypedBuilder = null;
                }

                internal IOptionBuilder AddOption(OptionDefinitionContext optionsDefinition, string optionName, Type dataType)
                {
                    m_OptionsDefinitionContext = optionsDefinition;
                    m_OptionName = optionName;
                    OptionType = dataType;
                    return this;
                }

                public IOptionBuilder WithDisplayName(string displayName)
                {
                    this.m_DisplayName = displayName;
                    return this;
                }

                public IOptionBuilder WithTooltip(string tooltip)
                {
                    this.m_Tooltip = tooltip;
                    return this;
                }

                public IOptionBuilder WithDefaultValue(object defaultValue)
                {
                    if (defaultValue != null && !OptionType.IsInstanceOfType(defaultValue))
                    {
                        throw new ArgumentException($"Default value type {defaultValue} is not assignable to option type {OptionType}");
                    }
                    this.m_DefaultValue = defaultValue;
                    return this;
                }

                public IOptionBuilder Delayed()
                {
                    if (!m_Attributes.Any(t => t is DelayedAttribute))
                        m_Attributes.Add(new DelayedAttribute());
                    return this;
                }

                public IOptionBuilder ShowInInspectorOnly()
                {
                    m_ShowInInspectorOnly = true;
                    return this;
                }

                public INodeOption Build()
                {
                    var attributesArray = m_Attributes.Count > 0 ? m_Attributes.ToArray() : null;
                    var result = m_OptionsDefinitionContext.OptionsDefinition.AddNodeOption(m_OptionName, OptionType,
                        m_DisplayName, m_Tooltip, m_ShowInInspectorOnly, m_Order, attributesArray, m_DefaultValue);
                    m_OptionsDefinitionContext.ReleaseBuilder(this);

                    return result;
                }
            }

            class OptionBuilder<TData> : IOptionBuilder<TData>
            {
                public OptionBuilder parent;

                internal void Reset() => parent.Reset();

                internal IOptionBuilder<TData> AddOption(OptionDefinitionContext optionsDefinition, string optionName)
                {
                    parent.AddOption(optionsDefinition, optionName, typeof(TData));
                    return this;
                }

                public IOptionBuilder<TData> WithDisplayName(string displayName)
                {
                    parent.WithDisplayName(displayName);
                    return this;
                }

                public IOptionBuilder<TData> WithTooltip(string tooltip)
                {
                    parent.WithTooltip(tooltip);
                    return this;
                }

                public IOptionBuilder<TData> WithDefaultValue(TData defaultValue)
                {
                    parent.WithDefaultValue(defaultValue);
                    return this;
                }

                public IOptionBuilder<TData> Delayed()
                {
                    parent.Delayed();
                    return this;
                }

                public IOptionBuilder<TData> ShowInInspectorOnly()
                {
                    parent.ShowInInspectorOnly();
                    return this;
                }

                public INodeOption Build() => parent.Build();
            }

            public IOptionsDefinition OptionsDefinition;

            List<OptionBuilder> m_Pool = new();
            List<OptionBuilder> m_Used = new();
            Dictionary<Type, List<object>> m_TypedBuilderPools = new();

            OptionBuilder GetFreeBuilder()
            {
                OptionBuilder builder;
                if (m_Pool.Count > 0)
                {
                    builder = m_Pool[^1];
                    m_Pool.RemoveAt(m_Pool.Count - 1);
                }
                else
                {
                    builder = new OptionBuilder(); //TODO : pool
                }
                m_Used.Add(builder);
                return builder;
            }

            void ReleaseBuilder(OptionBuilder builder)
            {
                if (builder == null)
                    return;

                if (builder.TypedBuilder != null)
                {
                    ReleaseTypedBuilder(builder.OptionType, builder.TypedBuilder);
                }

                if (m_Used.Remove(builder))
                {
                    m_Pool.Add(builder);
                    builder.Reset();
                }
            }

            OptionBuilder<T> GetFreeTypedBuilder<T>(OptionBuilder parent)
            {
                OptionBuilder<T> result;
                if (!m_TypedBuilderPools.TryGetValue(typeof(T), out var builderPool) || builderPool.Count == 0)
                {
                    builderPool = new List<object>();
                    m_TypedBuilderPools[typeof(T)] = builderPool;

                    result = new OptionBuilder<T>();
                }
                else
                {
                    result = (OptionBuilder<T>)builderPool[^1];
                    builderPool.RemoveAt(builderPool.Count - 1);
                }
                parent.TypedBuilder = result;
                result.parent = parent;
                return result;
            }

            void ReleaseTypedBuilder(Type type, object typedBuilder)
            {
                m_TypedBuilderPools[type].Add(typedBuilder);
            }

            public IOptionBuilder AddOption(string name, Type dataType)
            {
                return GetFreeBuilder().AddOption(this, name, dataType);
            }

            public IOptionBuilder<T> AddOption<T>(string name)
            {
                return GetFreeTypedBuilder<T>(GetFreeBuilder()).AddOption(this, name);
            }

            public void Finish()
            {
                while (m_Used.Count > 0)
                {
                    m_Used[0].Build();
                }
            }
        }

        static PortDefinitionContext s_PortDefinitionContext = new();
        static OptionDefinitionContext s_OptionDefinitionContext = new();

        internal void CallOnDefineNode(IPortsDefinition context)
        {
            s_PortDefinitionContext.portsDefinition = context;
            OnDefinePorts(s_PortDefinitionContext);
            s_PortDefinitionContext.Finish();
        }

        internal void CallOnDefineOptions(IOptionsDefinition context)
        {
            s_OptionDefinitionContext.OptionsDefinition = context;
            OnDefineOptions(s_OptionDefinitionContext);
            s_OptionDefinitionContext.Finish();
        }

        internal void SetImplementation(AbstractNodeModel implementation)
        {
            m_Implementation = implementation;
        }
    }
}
