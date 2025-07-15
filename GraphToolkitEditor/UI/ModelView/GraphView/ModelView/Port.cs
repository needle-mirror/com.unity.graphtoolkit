using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.InternalBridge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

// ReSharper disable InconsistentNaming

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// UI for a <see cref="PortModel"/>.
    /// Allows connection of <see cref="Wire"/>s.
    /// Handles dropping of elements on top of them to create a wire.
    /// </summary>
    [UnityRestricted]
    internal class Port : ModelView, ISelectionDraggerTarget
    {
        /// <summary>
        /// The USS class name added to a <see cref="Port"/>.
        /// </summary>
        public static readonly string ussClassName = "ge-port";

        /// <summary>
        /// The USS class name added to ports that will connect if the mouse is released at a current position.
        /// </summary>
        public static readonly string willConnectUssClassName = ussClassName.WithUssModifier("will-connect");

        /// <summary>
        /// The USS class name added to ports that are connected.
        /// </summary>
        public static readonly string connectedUssClassName = ussClassName.WithUssModifier(GraphElementHelper.connectedUssModifier);

        /// <summary>
        /// The USS class name added to ports that are not connected.
        /// </summary>
        public static readonly string notConnectedUssClassName = ussClassName.WithUssModifier(GraphElementHelper.notConnectedUssModifier);

        /// <summary>
        /// The USS class name added to ports that are inputs.
        /// </summary>
        public static readonly string inputUssClassName = ussClassName.WithUssModifier("direction-input");

        /// <summary>
        /// The USS class name added to ports that are outputs.
        /// </summary>
        public static readonly string outputUssClassName = ussClassName.WithUssModifier("direction-output");

        /// <summary>
        /// The USS class name added to ports that have no capacity.
        /// </summary>
        public static readonly string capacityNoneUssClassName = ussClassName.WithUssModifier("capacity-none");

        /// <summary>
        /// The USS class name added to ports that are hidden.
        /// </summary>
        public static readonly string hiddenUssClassName = ussClassName.WithUssModifier(GraphElementHelper.hiddenUssModifier);

        /// <summary>
        /// The USS class name added to indicate when dropping a wire on a port is permitted.
        /// </summary>
        public static readonly string dropHighlightAcceptedClass = ussClassName.WithUssModifier("drop-highlighted");

        /// <summary>
        /// The USS class name added to polymorphic ports.
        /// </summary>
        public static readonly string polymorphicUssClassName = ussClassName.WithUssModifier("polymorphic");

        /// <summary>
        /// The USS class name added to expandable ports.
        /// </summary>
        public static readonly string expandableUssClassName = ussClassName.WithUssModifier("expandable");

        /// <summary>
        /// The USS class name added to indicate when dropping a wire on a port is forbidden.
        /// </summary>
        public static readonly string dropHighlightDeniedClass = dropHighlightAcceptedClass.WithUssModifier("denied");

        /// <summary>
        /// The USS class name used as a prefix for the <see cref="PortModel.DataTypeHandle"/> of ports.
        /// </summary>
        public static readonly string portDataTypeClassNamePrefix = ussClassName.WithUssModifier(GraphElementHelper.dataTypeClassUssModifierPrefix);

        /// <summary>
        /// The USS class name used as a prefix for the <see cref="PortModel.PortType"/> of ports.
        /// </summary>
        public static readonly string portTypeUssClassNamePrefix = ussClassName.WithUssModifier("type-");

        /// <summary>
        /// The USS class name added to vertical ports.
        /// </summary>
        public static readonly string verticalUssClassName = ussClassName.WithUssModifier(GraphElementHelper.verticalUssModifier);

        /// <summary>
        /// The name of the <see cref="ModelViewPart"/> for the connector on the port.
        /// </summary>
        public static readonly string connectorPartName = "connector-container";

        /// <summary>
        /// The name of the <see cref="ModelViewPart"/> for the constant editor on the port.
        /// </summary>
        public static readonly string constantEditorPartName = "constant-editor";

        public static readonly Vector2 minHitBoxSize = new Vector2(24, 24);

        static readonly CustomStyleProperty<Color> k_PortColorProperty = new CustomStyleProperty<Color>("--port-color");

        protected string m_CurrentDropHighlightClass = dropHighlightAcceptedClass;

        string m_CurrentTypeClassName;

        bool m_Hovering;
        bool m_WillConnect;

        WireConnector m_WireConnector;
        VisualElement m_ConnectorCache;
        protected Label m_ConnectorLabel;

        TypeHandle m_CurrentTypeHandle;

        /// <summary>
        /// The default port color.
        /// </summary>
        public static Color DefaultPortColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return new Color(193 / 255f, 193 / 255f, 193 / 255f);
                }

                return new Color(90 / 255f, 90 / 255f, 90 / 255f);
            }
        }

        /// <summary>
        /// The <see cref="GraphView"/> this port belongs to.
        /// </summary>
        public GraphView GraphView => RootView as GraphView;

        public PortModel PortModel => Model as PortModel;

        public WireConnector WireConnector
        {
            get => m_WireConnector;
            protected set => ConnectorElement.ReplaceManipulator(ref m_WireConnector, value);
        }

        public VisualElement ConnectorElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the port will be connected during an edge drag if the mouse is released where it is.
        /// </summary>
        public bool WillConnect
        {
            get => m_WillConnect;
            set
            {
                if (m_WillConnect != value)
                {
                    m_WillConnect = value;
                    EnableInClassList(willConnectUssClassName, value);
                    GetConnector()?.MarkDirtyRepaint();
                }
            }
        }

        public Color PortColor { get; protected set; } = DefaultPortColor;

        /// <inheritdoc />
        public virtual bool CanAcceptDrop(IReadOnlyList<GraphElementModel> droppedElements)
        {
            // Only one element can be dropped at once.
            if (droppedElements.Count != 1)
                return false;

            // The elements that can be dropped: a variable declaration from the Blackboard and any node with a single input or output (eg.: variable and constant nodes).
            switch (droppedElements[0])
            {
                case VariableDeclarationModelBase variableDeclaration:
                    return CanAcceptDroppedVariable(variableDeclaration);
                case ISingleInputPortNodeModel:
                case ISingleOutputPortNodeModel:
                    return GetPortToConnect(droppedElements[0]) != null;
                default:
                    return false;
            }
        }

        bool CanAcceptDroppedVariable(VariableDeclarationModelBase variableDeclaration)
        {
            if (variableDeclaration is IPlaceholder)
                return false;

            if (PortModel.Capacity == PortCapacity.None)
                return false;

            if (!variableDeclaration.IsInputOrOutput)
                return PortModel.Direction == PortDirection.Input && variableDeclaration.DataType == PortModel.DataTypeHandle;

            if (PortModel.DataTypeHandle != variableDeclaration.DataType)
                return false;

            var isInput = variableDeclaration.Modifiers == ModifierFlags.Read;
            if (isInput && PortModel.Direction != PortDirection.Input || !isInput && PortModel.Direction != PortDirection.Output)
                return false;

            return true;
        }

        /// <inheritdoc />
        public virtual void ClearDropHighlightStatus()
        {
            RemoveFromClassList(m_CurrentDropHighlightClass);
        }

        /// <inheritdoc />
        public virtual void SetDropHighlightStatus(IReadOnlyList<GraphElementModel> dropCandidates)
        {
            m_CurrentDropHighlightClass = CanAcceptDrop(dropCandidates) ? dropHighlightAcceptedClass : dropHighlightDeniedClass;
            AddToClassList(m_CurrentDropHighlightClass);
        }

        /// <inheritdoc />
        public virtual void PerformDrop(IReadOnlyList<GraphElementModel> dropCandidates)
        {
            if (GraphView == null)
                return;

            var selectable = dropCandidates.Single();
            var portToConnect = GetPortToConnect(selectable);
            Assert.IsNotNull(portToConnect);

            var toPort = portToConnect.Direction == PortDirection.Input ? portToConnect : PortModel;
            var fromPort = portToConnect.Direction == PortDirection.Input ? PortModel : portToConnect;

            GraphView.Dispatch(new CreateWireCommand(toPort, fromPort, true));
        }

        /// <inheritdoc />
        protected override void BuildPartList()
        {
            var connectionPart = PortModel.IsPolymorphic
                ? PortConnectorPolymorphicPart.Create(connectorPartName, Model, this, ussClassName)
                : PortConnectorWithIconPart.Create(connectorPartName, Model, this, ussClassName);

            PartList.AppendPart(connectionPart);
            PartList.AppendPart(PortConstantEditorPart.Create(constantEditorPartName, Model, this, ussClassName));
        }

        public Label Label
        {
            get;
            private set;
        }

        /// <inheritdoc />
        protected override void PostBuildUI()
        {
            var connectorPart = (PortConnectorPart)PartList.GetPart(connectorPartName);

            ConnectorElement = connectorPart.Root ?? this;
            WireConnector = new WireConnector(GraphView);

            Label = ConnectorElement.Q<Label>(GraphElementHelper.labelName);

            var constantPart = (PortConstantEditorPart)PartList.GetPart(constantEditorPartName);
            constantPart.SetDragZone(Label);

            AddToClassList(ussClassName);
            this.AddPackageStylesheet("Port.uss");
        }

        /// <inheritdoc />
        public override bool HasModelDependenciesChanged()
        {
            return true;
        }

        /// <inheritdoc/>
        public override void AddModelDependencies()
        {
            foreach (var wireModel in PortModel.GetConnectedWires())
            {
                AddDependencyToWireModel(wireModel);
            }

            // The value editor need to be refreshed to enable or disable its fields, if there is an editor and an ancestor or descendant port is changed.
            if (PortModel.Direction == PortDirection.Input)
            {
                var parentPort = PortModel.ParentPort;
                while (parentPort != null)
                {
                    Dependencies.AddModelDependency(parentPort);
                    parentPort = parentPort.ParentPort;
                }

                AddSubPorts(PortModel);
            }

            void AddSubPorts(PortModel portModel)
            {
                foreach (var subPort in portModel.SubPorts)
                {
                    Dependencies.AddModelDependency(subPort);
                    AddSubPorts(subPort);
                }
            }
        }

        /// <summary>
        /// Add <paramref name="wireModel"/> as a model dependency to this element.
        /// </summary>
        /// <param name="wireModel">The model to add as a dependency.</param>
        public void AddDependencyToWireModel(WireModel wireModel)
        {
            // When wire is created/deleted, port connector needs to be updated (filled/unfilled).
            Dependencies.AddModelDependency(wireModel);
        }

        protected override void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            base.OnCustomStyleResolved(evt);
            var currentColor = PortColor;

            if (evt.customStyle.TryGetValue(k_PortColorProperty, out var portColorValue))
                PortColor = portColorValue;

            if (currentColor != PortColor && PartList.GetPart(connectorPartName) is PortConnectorWithIconPart portConnector)
            {
                portConnector.DoCompleteUpdate();
            }
        }

        static List<(int id, string className)> s_PortTypeClassNameCache = new List<(int, string)>(4)
        {
            (PortType.Default.Id, GenerateClassNameForPortType(PortType.Default)),
            (PortType.MissingPort.Id, GenerateClassNameForPortType(PortType.MissingPort)),
        };

        static string GenerateClassNameForPortType(PortType t)
        {
            return portTypeUssClassNamePrefix + GetClassNameSuffixForType(t);
        }

        /// <summary>
        /// Assigns an icon to a port item in the item library.
        /// </summary>
        /// <param name="itemIconContainer">The container of item's icon.</param>
        public virtual void AssignIconToPortItemInLibrary(VisualElement itemIconContainer)
        {
            if (PortModel.DataTypeHandle == TypeHandle.ExecutionFlow)
            {
                GetTriangleConnectorForItemLibrary(itemIconContainer);
                return;
            }

            var imageIcon = new Image();
            imageIcon.AddPackageStylesheet("TypeIcons.uss");
            imageIcon.tintColor = PortColor;

            RegisterCallback<CustomStyleResolvedEvent>(evt =>
            {
                if (evt.customStyle.TryGetValue(new CustomStyleProperty<Color>("--port-color"), out var portColorValue))
                    imageIcon.tintColor = portColorValue;
            });

            RootView.TypeHandleInfos.AddUssClasses(GraphElementHelper.iconDataTypeClassPrefix, imageIcon, PortModel.DataTypeHandle);

            itemIconContainer.Add(imageIcon);
        }

        protected void GetTriangleConnectorForItemLibrary(VisualElement iconContainer)
        {
            iconContainer.AddPackageStylesheet("PortConnectorPart.uss");
            iconContainer.AddPackageStylesheet("Port.uss");
            iconContainer.AddToClassList(ussClassName);
            iconContainer.AddToClassList(ussClassName.WithUssElement(connectorPartName));
            iconContainer.AddToClassList(outputUssClassName);
            iconContainer.AddToClassList(notConnectedUssClassName);
            if (PortModel.Orientation == PortOrientation.Vertical)
                iconContainer.AddToClassList(verticalUssClassName);

            var connector = new VisualElement { name = PortConnectorPart.connectorUssName };
            connector.AddToClassList(ussClassName.WithUssElement(PortConnectorPart.connectorUssName));
            connector.AddToClassList(PortConnectorPart.ussClassName.WithUssElement(PortConnectorPart.connectorUssName));
            iconContainer.Add(connector);
            connector.RegisterCallbackOnce<GeometryChangedEvent>(_ =>
            {
                connector.generateVisualContent = OnGenerateTriangleConnectorVisualContent;
            });
            RegisterCallbackOnce<CustomStyleResolvedEvent>(evt =>
            {
                if (evt.customStyle.TryGetValue(new CustomStyleProperty<Color>("--port-color"), out var portColorValue))
                {
                    connector.style.borderBottomColor = portColorValue;
                    connector.style.borderLeftColor = portColorValue;
                    connector.style.borderRightColor = portColorValue;
                    connector.style.borderTopColor = portColorValue;
                }

                if (evt.customStyle.TryGetValue(new CustomStyleProperty<Color>("--port-background"), out var portBackgroundValue))
                    connector.style.backgroundColor = portBackgroundValue;
            });
        }

        protected static string GetClassNameSuffixForType(PortType t)
        {
            return t.ToString().ToLower();
        }

        static string GetCachedClassNameForPortType(PortType t)
        {
            var cacheIndex = -1;
            for (int i = 0; i < s_PortTypeClassNameCache.Count; i++)
            {
                if (s_PortTypeClassNameCache[i].id == t.Id)
                {
                    cacheIndex = i;
                    break;
                }
            }

            if (cacheIndex == -1)
            {
                cacheIndex = s_PortTypeClassNameCache.Count;
                s_PortTypeClassNameCache.Add((t.Id, GenerateClassNameForPortType(t)));
            }

            return s_PortTypeClassNameCache[cacheIndex].className;
        }

        static bool PortHasOption(PortModelOptions portOptions, PortModelOptions optionToCheck)
        {
            return (portOptions & optionToCheck) != 0;
        }

        /// <inheritdoc />
        public override void UpdateUIFromModel(UpdateFromModelVisitor visitor)
        {
            var wasUpdatedAtLeastOnce = m_CurrentTypeClassName != null;
            // Speed-up when creating many ports:
            // The first time the port is updated, avoid trying to remove classes we know aren't there
            void EnableClass(string className, bool condition)
            {
                if (wasUpdatedAtLeastOnce)
                    EnableInClassList(className, condition);
                else if (condition)
                {
                    AddToClassList(className);
                }
            }

            var portIsConnected = PortModel.IsConnected();
            EnableClass(connectedUssClassName, portIsConnected);
            EnableClass(notConnectedUssClassName, !portIsConnected);

            var constantPart = (PortConstantEditorPart)PartList.GetPart(constantEditorPartName);
            constantPart.SetDragZone(Label);

            if (visitor.ChangeHints.HasChange(ChangeHint.Data))
            {
                var parentNode = GetParentNode(this);
                parentNode?.DisableCullingForFrame();

                this.PreallocForMoreClasses(6); // hidden, connected, direction, capacity, datatype, type
                var hidden = PortModel != null && PortHasOption(PortModel.Options, PortModelOptions.Hidden);
                EnableClass(hiddenUssClassName, hidden);

                EnableClass(inputUssClassName, PortModel.Direction == PortDirection.Input);
                EnableClass(outputUssClassName, PortModel.Direction == PortDirection.Output);
                EnableClass(capacityNoneUssClassName, PortModel.Capacity == PortCapacity.None);
                EnableClass(verticalUssClassName, PortModel.Orientation == PortOrientation.Vertical);
                EnableClass(expandableUssClassName, PortModel.IsExpandable);
                EnableClass(polymorphicUssClassName, PortModel.PolymorphicPortHandler != null);

                if (!wasUpdatedAtLeastOnce || m_CurrentTypeHandle != PortModel.DataTypeHandle)
                {
                    if (wasUpdatedAtLeastOnce)
                    {
                        RootView.TypeHandleInfos.RemoveUssClasses(portDataTypeClassNamePrefix, this, m_CurrentTypeHandle);
                    }
                    m_CurrentTypeHandle = PortModel.DataTypeHandle;
                    RootView.TypeHandleInfos.AddUssClasses(portDataTypeClassNamePrefix, this, m_CurrentTypeHandle);
                }

                this.ReplaceAndCacheClassName(GetCachedClassNameForPortType(PortModel.PortType), ref m_CurrentTypeClassName);

                var connector = GetConnector();
                if (connector != null)
                {
                    var currentGen = connector.generateVisualContent;
                    var newGen = GetConnectorVisualContentGenerator();
                    if (currentGen != newGen)
                    {
                        connector.generateVisualContent = newGen;
                        connector.MarkDirtyRepaint();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the visual content generator for the port connector.
        /// </summary>
        /// <returns>The connector visual content generator.</returns>
        protected virtual Action<MeshGenerationContext> GetConnectorVisualContentGenerator()
        {
            if (PortModel.DataTypeHandle == TypeHandle.ExecutionFlow)
                return OnGenerateTriangleConnectorVisualContent;

            return OnGenerateCircleConnectorVisualContent;
        }

        public Vector3 GetGlobalCenter()
        {
            if (m_CullingReference != null)
                return m_CullingReference.LocalToWorld(m_CachedPortCenter);
            var connector = GetConnector();
            var localCenter = new Vector2(connector.layout.width * .5f, connector.layout.height * .5f);
            return connector.LocalToWorld(localCenter);
        }

        public VisualElement GetConnector()
        {
            if (m_ConnectorCache == null)
            {
                var portConnector = PartList.GetPart(connectorPartName) as PortConnectorPart;
                m_ConnectorCache = portConnector?.Connector ?? portConnector?.Root ?? this;
            }

            return m_ConnectorCache;
        }

        /// <summary>
        /// Whether the mouse is Hovering the port.
        /// </summary>
        public bool Hovering
        {
            get => m_Hovering;
            set
            {
                m_Hovering = value;
                GetConnector()?.MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Whether the port cap should actually be visible.
        /// </summary>
        public bool IsCapVisible => Hovering || WillConnect || PortModel.IsConnected();

        /// <summary>
        /// Adds a menu item to create a variable node and to connect it to the port.
        /// </summary>
        /// <param name="evt">The <see cref="ContextualMenuPopulateEvent"/> event.</param>
        /// <param name="isInputOrOutput">Whether the created variable is an input or an output to a subgraph.</param>
        /// <param name="sectionName">The name of the section in the Blackboard in which the variable declaration should be created.</param>
        protected void AddCreateVariableFromPortMenuItem(ContextualMenuPopulateEvent evt, bool isInputOrOutput, string sectionName = null)
        {
            evt.menu.AppendAction("Create Variable from port", _ =>
            {
                var blackboardSection = GraphView.GraphModel.GetSectionModel(string.IsNullOrEmpty(sectionName) ? GraphModel.DefaultSectionName : sectionName);
                var modifierFlags = !isInputOrOutput ? ModifierFlags.None :
                    PortModel.Direction == PortDirection.Input ? ModifierFlags.Read : ModifierFlags.Write;
                GraphView.Dispatch(CreateNodeCommand.OnPort(PortModel, blackboardSection, modifierFlags: modifierFlags));
            });
        }

        /// <summary>
        /// Adds a menu item to create a variable node and to connect it to the port.
        /// </summary>
        /// <param name="evt">The <see cref="ContextualMenuPopulateEvent"/> event.</param>
        protected virtual void AddCreateVariableFromPortMenuItem(ContextualMenuPopulateEvent evt)
        {
            // Don't show this item for an output port in a container graph to prevent creating output nodes
            if (PortModel.Direction != PortDirection.Output || !PortModel.GraphModel.IsContainerGraph())
                AddCreateVariableFromPortMenuItem(evt, PortModel.Direction == PortDirection.Output);
        }

        /// <inheritdoc />
        protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (evt.currentTarget is not Port)
                return;

            AddCreateVariableFromPortMenuItem(evt);

            evt.menu.AppendAction("Create Node from port", menuAction =>
            {
                ShowItemLibrary();
            });

            evt.menu.AppendSeparator();

            var connectedPortsCount = PortModel.GetConnectedPorts().Count;
            evt.menu.AppendAction("Disconnect Wire" + (connectedPortsCount > 1 ? "s" : ""), _ =>
            {
                GraphView.Dispatch(new DisconnectWiresOnPortCommand(PortModel));
            }, connectedPortsCount == 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

            evt.StopPropagation();
        }

        void ShowItemLibrary()
        {
            var portPosition = GetGlobalCenter();

            if (PortModel.Direction == PortDirection.Input)
            {
                ItemLibraryService.ShowInputToGraphNodes(GraphView, new[] { PortModel }, portPosition, item =>
                {
                    if (item is GraphNodeModelLibraryItem nodeItem)
                        GraphView.Dispatch(CreateNodeCommand.OnPort(nodeItem, PortModel, Vector2.zero, autoAlign: true));
                });
            }
            else
            {
                ItemLibraryService.ShowOutputToGraphNodes(GraphView, new[] { PortModel }, portPosition, item =>
                {
                    if (item is GraphNodeModelLibraryItem nodeItem)
                        GraphView.Dispatch(CreateNodeCommand.OnPort(nodeItem, PortModel, Vector2.zero, autoAlign: true));
                });
            }
        }

        /// <summary>
        /// Gets the port hit box.
        /// </summary>
        /// <param name="port">The <see cref="Port"/> to get the hit box for.</param>
        /// <param name="isCreatingFrom">Whether the user is creating a wire from the port. Else, the user is plugging a wire into the port. </param>
        public static Rect GetPortHitBoxBounds(Port port, bool isCreatingFrom = false)
        {
            if (port.worldBound.height <= 0 || port.worldBound.width <= 0)
                return Rect.zero;

            var node = GetParentNode(port);
            if (node is null)
                return Rect.zero;

            // If the wire is being plugged into a capsule node, the hit box is the whole node.
            if (port.PortModel.NodeModel is ISingleInputPortNodeModel or ISingleOutputPortNodeModel && !isCreatingFrom)
                return node.worldBound;

            var minWorldHitBoxSize = MathUtils.MultiplyVector2(port.worldTransform, minHitBoxSize);
            var connector = port.GetConnector();
            var direction = port.PortModel.Direction;
            var orientation = port.PortModel.Orientation;

            var portConnectorPart = port.PartList.GetPart(connectorPartName) as PortConnectorPart;
            var portConnector = portConnectorPart?.Root;
            if (portConnector is null)
                return Rect.zero;

            var hitBoxRect = isCreatingFrom ? GetCreateFromPortHitBox() : GetConnectToPortHitBox();
            AdjustHitBoxRect();

            return hitBoxRect;

            Rect GetCreateFromPortHitBox()
            {
                float x;
                float y;
                var hitBoxSize = minWorldHitBoxSize;
                if (orientation == PortOrientation.Horizontal)
                {
                    // If the port has an icon, use it to compute the create from hit box.
                    if (portConnectorPart is PortConnectorWithIconPart portConnectorWithIcon &&
                        portConnectorWithIcon.Icon.layout.size != Vector2.zero && !port.PortModel.IsPolymorphic)
                    {
                        var iconPos = portConnectorWithIcon.Icon.worldBound;
                        var margin = iconPos.size.x * 0.2f;
                        x = direction == PortDirection.Input ? node.worldBound.xMin : iconPos.xMin - margin;
                        y = portConnector.worldBound.yMin;
                        hitBoxSize = new Vector2(direction == PortDirection.Input ? iconPos.xMax + margin - x : node.worldBound.xMax - x, portConnector.worldBound.size.y);
                    }
                    else
                    {
                        // If the port has no icon, use the minWorldHitBoxSize.
                        x = direction == PortDirection.Input ? node.worldBound.xMin : node.worldBound.xMax - minWorldHitBoxSize.x;
                        y = connector.worldBound.center.y - minWorldHitBoxSize.y * 0.5f;
                    }
                }
                else
                {
                    x = connector.worldBound.center.x - minWorldHitBoxSize.x * 0.5f;
                    y = direction == PortDirection.Input ? node.worldBound.yMin : node.worldBound.yMax - minWorldHitBoxSize.y;
                }
                if (port.PortModel.IsExpandable && float.IsFinite(portConnectorPart.ExpandToggle.layout.xMin) && port.PortModel.Orientation == PortOrientation.Horizontal)
                {
                    if (port.PortModel.Direction == PortDirection.Output)
                    {
                        if (!port.PortModel.IsPolymorphic)
                        {
                            var expandToggleX = portConnectorPart.ExpandToggle.worldBound.xMax;
                            var newSize = x + hitBoxSize.x - expandToggleX;
                            x = expandToggleX;
                            hitBoxSize.x = newSize;
                        }
                    }
                    else
                        hitBoxSize.x = portConnectorPart.ExpandToggle.worldBound.xMin - x;
                }

                return new Rect(new Vector2(x, y), hitBoxSize);
            }

            Rect GetConnectToPortHitBox()
            {
                float x;
                float y;
                var hitBoxSize = minWorldHitBoxSize;
                var offset = minWorldHitBoxSize.x * 0.5f;
                if (orientation == PortOrientation.Horizontal)
                {
                    x = direction == PortDirection.Input ? node.worldBound.xMin - offset : portConnector.worldBound.xMin;
                    y = connector.worldBound.center.y - minWorldHitBoxSize.y * 0.5f;
                    hitBoxSize = new Vector2(direction == PortDirection.Input ? portConnector.worldBound.xMax - x
                        : node.worldBound.xMax + offset - x, minWorldHitBoxSize.y);
                }
                else
                {
                    // Rotate the hit box for vertical ports.
                    hitBoxSize.x = minHitBoxSize.y;
                    hitBoxSize.y = minHitBoxSize.x;
                    hitBoxSize = MathUtils.MultiplyVector2(port.worldTransform, hitBoxSize);
                    x = connector.worldBound.center.x - hitBoxSize.x * 0.5f;
                    y = direction == PortDirection.Input ? node.worldBound.yMin - offset : node.worldBound.yMax - offset;
                }

                return new Rect(new Vector2(x, y), hitBoxSize);
            }

            void AdjustHitBoxRect()
            {
                // If the port's rect is bigger than the default hit box rect, size up the hit box.
                // Do not adjust horizontally, we do not want to include the label and constant editor.
                if (orientation is PortOrientation.Horizontal)
                {
                    if (hitBoxRect.size.y < port.worldBound.size.y)
                    {
                        hitBoxRect.yMin = port.worldBound.yMin;
                        hitBoxRect.yMax = port.worldBound.yMax;
                    }
                }

                // Make sure that the port is covered properly by the hit box.
                if (direction == PortDirection.Input)
                {
                    if (hitBoxRect.yMax < port.worldBound.yMax)
                        hitBoxRect.yMax = port.worldBound.yMax;
                }
                else
                {
                    if (hitBoxRect.yMin > port.worldBound.yMin)
                        hitBoxRect.yMin = port.worldBound.yMin;
                }
            }
        }

        PortModel GetPortToConnect(GraphElementModel selectable)
        {
            return (selectable as PortNodeModel)?.GetPortFitToConnectTo(PortModel);
        }

        protected void OnGenerateTriangleConnectorVisualContent(MeshGenerationContext mgc)
        {
            OnGenerateExecutionConnectorVisualContent(mgc, PortColor, PortModel?.Orientation ?? PortOrientation.Horizontal, GetConnector().layout, IsCapVisible);
        }

        internal static void OnGenerateExecutionConnectorVisualContent(MeshGenerationContext mgc, Color portColor, PortOrientation orientation, Rect connectorRect, bool isCapVisible)
        {
            mgc.painter2D.strokeColor = portColor;
            mgc.painter2D.lineJoin = LineJoin.Round;

            var paintRect = connectorRect;
            paintRect.position = Vector2.zero;

            MakeTriangle(mgc.painter2D, paintRect, orientation);
            mgc.painter2D.lineWidth = 1.0f;
            mgc.painter2D.Stroke();

            if (isCapVisible)
            {
                paintRect.position += orientation == PortOrientation.Horizontal ? new Vector2(1.33f, 2) : new Vector2(2, 1.33f);
                paintRect.size -= Vector2.one * 4;
                mgc.painter2D.fillColor = portColor;
                MakeTriangle(mgc.painter2D, paintRect, orientation);
                mgc.painter2D.Fill();
            }
        }

        protected void OnGenerateCircleConnectorVisualContent(MeshGenerationContext mgc)
        {
            mgc.painter2D.strokeColor = PortColor;
            mgc.painter2D.lineJoin = LineJoin.Round;

            var paintRect = GetConnector().localBound;
            paintRect.position = Vector2.zero;
            paintRect.position += Vector2.one * 0.5f;
            paintRect.size -= Vector2.one;

            MakeCircle(mgc.painter2D, paintRect);
            mgc.painter2D.lineWidth = 1.0f;
            mgc.painter2D.Stroke();

            if (IsCapVisible)
            {
                paintRect.position += Vector2.one * 1.5f;
                paintRect.size -= Vector2.one * 3;
                mgc.painter2D.fillColor = PortColor;
                MakeCircle(mgc.painter2D, paintRect);
                mgc.painter2D.Fill();
            }
        }

        static void MakeTriangle(Painter2D painter2D, Rect paintRect, PortOrientation orientation)
        {
            painter2D.BeginPath();
            if (orientation == PortOrientation.Horizontal)
            {
                painter2D.MoveTo(new Vector2(paintRect.xMin, paintRect.yMin));
                painter2D.LineTo(new Vector2(paintRect.xMax, paintRect.center.y));
                painter2D.LineTo(new Vector2(paintRect.xMin, paintRect.yMax));
                painter2D.LineTo(new Vector2(paintRect.xMin, paintRect.yMin));
            }
            else
            {
                painter2D.MoveTo(new Vector2(paintRect.xMin, paintRect.yMin));
                painter2D.LineTo(new Vector2(paintRect.xMax, paintRect.yMin));
                painter2D.LineTo(new Vector2(paintRect.center.x, paintRect.yMax));
                painter2D.LineTo(new Vector2(paintRect.xMin, paintRect.yMin));
            }
            painter2D.ClosePath();
        }

        void MakeCircle(Painter2D painter2D, Rect paintRect)
        {
            painter2D.BeginPath();
            painter2D.Arc(paintRect.center, paintRect.width * 0.5f, 0, Angle.Turns(1));
            painter2D.ClosePath();
        }

        static NodeView GetParentNode(Port port)
        {
            return port.PortModel.NodeModel.GetView<NodeView>(port.RootView);
        }

        VisualElement m_CullingReference;
        Vector2 m_CachedPortCenter;

        /// <summary>
        /// Prepare the port for culling by saving their global center.
        /// </summary>
        /// <param name="cullingReference">A visual element that will not be culled and can be used as a reference for the port location.</param>
        public virtual void PrepareCulling(VisualElement cullingReference)
        {
            m_CachedPortCenter = cullingReference.WorldToLocal(GetGlobalCenter());
            m_CullingReference = cullingReference;
        }

        /// <summary>
        /// Clear culling cache.
        /// </summary>
        public virtual void ClearCulling()
        {
            m_CullingReference = null;
        }
    }
}
