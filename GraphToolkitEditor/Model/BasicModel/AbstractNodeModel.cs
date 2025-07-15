using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base class for a model that represents a node in a graph.
    /// </summary>
    [Serializable]
    [UnityRestricted]
    internal abstract class AbstractNodeModel : GraphElementModel, IHasTitle, IMovable, IHasElementColor
    {
        [SerializeField, HideInInspector]
        Vector2 m_Position;

        [SerializeField, HideInInspector]
        protected string m_Title;

        [SerializeField, HideInInspector]
        protected string m_Tooltip;

        SpawnFlags m_SpawnFlags = SpawnFlags.Default;

        [SerializeReference, HideInInspector]
        NodePreviewModel m_NodePreviewModel;

        [SerializeField, HideInInspector]
        ModelState m_State;

        internal static string titleFieldName = nameof(m_Title);
        internal static string positionFieldName = nameof(m_Position);

        /// <summary>
        /// Whether the node allows self-connection.
        /// </summary>
        public abstract bool AllowSelfConnect { get; }

        /// <summary>
        /// The type of the node icon as a string.
        /// </summary>
        public abstract string IconTypeString { get; set; }

        /// <inheritdoc />
        public abstract ElementColor ElementColor { get; }

        /// <inheritdoc />
        public abstract void SetColor(Color color);

        /// <inheritdoc />
        public abstract Color DefaultColor { get; }

        /// <inheritdoc />
        public abstract bool UseColorAlpha { get; }

        /// <summary>
        /// The state of the node model.
        /// </summary>
        /// <remarks>Setter implementations must set the <see cref="ChangeHint.Data"/> change hint.</remarks>
        public virtual ModelState State
        {
            get => m_State;
            set
            {
                if (m_State == value)
                    return;
                if (!IsDisableable())
                    return;
                m_State = value;
                GraphModel?.CurrentGraphChangeDescription.AddChangedModel(this, ChangeHint.Data);
            }
        }

        /// <inheritdoc />
        /// <remarks>Setter implementations must set the <see cref="ChangeHint.Data"/> change hint.</remarks>
        public virtual string Title
        {
            get => m_Title;
            set
            {
                if (m_Title == value)
                    return;
                m_Title = value;
                GraphModel?.CurrentGraphChangeDescription.AddChangedModel(this, ChangeHint.Data);
            }
        }

        /// <summary>
        /// The subtitle of the node.
        /// </summary>
        /// <remarks>Setter implementations must set the <see cref="ChangeHint.Data"/> change hints.</remarks>
        public virtual string Subtitle => GetType().GetAttribute<LibraryItemAttribute>()?.Subtitle;

        /// <summary>
        /// The tooltip to display when hovering on the node.
        /// </summary>
        /// <remarks>Setter implementations must set the <see cref="ChangeHint.Style"/> change hints.</remarks>
        public virtual string Tooltip
        {
            get => string.IsNullOrEmpty(m_Tooltip) ? Title : m_Tooltip;
            set
            {
                if (m_Tooltip == value)
                    return;
                m_Tooltip = value;
                GraphModel?.CurrentGraphChangeDescription.AddChangedModel(this, ChangeHint.Style);
            }
        }

        /// <inheritdoc />
        /// <remarks>Setter implementations must set the <see cref="ChangeHint.Layout"/> change hint.</remarks>
        public virtual Vector2 Position
        {
            get => m_Position;
            set
            {
                if (!IsMovable())
                    return;

                if (m_Position == value)
                    return;

                m_Position = value;
                GraphModel?.CurrentGraphChangeDescription.AddChangedModel(this, ChangeHint.Layout);
            }
        }

        /// <summary>
        /// The flags specifying how the node is to be spawned.
        /// </summary>
        public virtual SpawnFlags SpawnFlags
        {
            get => m_SpawnFlags;
            set => m_SpawnFlags = value;
        }

        /// <summary>
        /// Whether the object was deleted from the graph.
        /// </summary>
        public bool Destroyed { get; private set; }

        /// <summary>
        /// Whether the node has a preview or not.
        /// </summary>
        /// <remarks>The preview section of a node provides a visual representation of the graph's state up to that point.</remarks>
        public abstract bool HasNodePreview { get; }

        /// <summary>
        /// The preview of the node.
        /// </summary>
        public NodePreviewModel NodePreviewModel => HasNodePreview ? m_NodePreviewModel : null;

        /// <inheritdoc />
        public override IEnumerable<GraphElementModel> DependentModels => m_NodePreviewModel != null ? base.DependentModels.Append(m_NodePreviewModel) : base.DependentModels;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractNodeModel"/> class.
        /// </summary>
        protected AbstractNodeModel()
        {
            m_Capabilities.AddRange(new[]
            {
                Editor.Capabilities.Deletable,
                Editor.Capabilities.Droppable,
                Editor.Capabilities.Copiable,
                Editor.Capabilities.Selectable,
                Editor.Capabilities.Movable,
                Editor.Capabilities.Collapsible,
                Editor.Capabilities.Colorable,
                Editor.Capabilities.Ascendable,
                Editor.Capabilities.Disableable
            });
        }

        /// <summary>
        /// Called on deletion of the node.
        /// </summary>
        public virtual void OnDeleteNode()
        {
            Destroyed = true;

            if (m_NodePreviewModel != null)
                GraphModel.CurrentGraphChangeDescription.AddDeletedModel(m_NodePreviewModel);
        }

        /// <summary>
        /// Gets all wires connected to this node.
        /// </summary>
        /// <returns>All <see cref="WireModel"/> connected to this node.</returns>
        public abstract IEnumerable<WireModel> GetConnectedWires();

        /// <summary>
        /// Called on creation of the node.
        /// </summary>
        public virtual void OnCreateNode()
        {
            if (HasNodePreview && SpawnFlags != SpawnFlags.Orphan)
                AddNodePreview();
        }

        /// <summary>
        /// Called on duplication of the node.
        /// </summary>
        /// <param name="sourceNode">Model of the node duplicated.</param>
        public virtual void OnDuplicateNode(AbstractNodeModel sourceNode)
        {
            if (sourceNode is null)
                return;

            Title = sourceNode.Title;
            if (sourceNode.HasNodePreview && sourceNode.NodePreviewModel is not null)
            {
                var nodePreview = AddNodePreview();
                nodePreview.OnDuplicateNodePreview(sourceNode.NodePreviewModel);
            }
        }

        /// <inheritdoc />
        public virtual void Move(Vector2 delta)
        {
            if (!IsMovable())
                return;

            Position += delta;
        }

        /// <summary>
        /// Creates a node preview.
        /// </summary>
        /// <returns>The newly created node preview.</returns>
        protected virtual NodePreviewModel CreateNodePreview()
        {
            return new NodePreviewModel();
        }

        internal void SyncNodePreview()
        {
            if (HasNodePreview && m_NodePreviewModel == null)
                AddNodePreview();
            else if (!HasNodePreview && m_NodePreviewModel != null)
            {
                m_NodePreviewModel = null;
            }
        }

        /// <summary>
        /// Creates and adds a node preview to the node.
        /// </summary>
        /// <returns>The newly created node preview.</returns>
        protected virtual NodePreviewModel AddNodePreview()
        {
            var nodePreviewModel = CreateNodePreview();
            m_NodePreviewModel = nodePreviewModel;
            nodePreviewModel.OnCreateNodePreview(this);

            GraphModel.RegisterNodePreview(m_NodePreviewModel);
            GraphModel.CurrentGraphChangeDescription.AddNewModel(m_NodePreviewModel);

            return nodePreviewModel;
        }
    }
}
