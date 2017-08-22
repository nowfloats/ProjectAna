using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using System.Windows;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models;
using ANAConversationStudio.Helpers;

namespace ANAConversationStudio.ViewModels
{
    /// <summary>
    /// Defines a node in the view-model.
    /// Nodes are connected to other nodes through attached connectors (aka anchor/connection points).
    /// </summary>
    public sealed class NodeViewModel : AbstractModelBase
    {
        #region Private Data Members

        /// <summary>
        /// The name of the node.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        private double x = 0;

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        private double y = 0;

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        private int zIndex = 0;

        /// <summary>
        /// The size of the node.
        /// 
        /// Important Note: 
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        private Size size = Size.Empty;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> inputConnectors = null;

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> outputConnectors = null;

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        private bool isSelected = false;

        #endregion Private Data Members

        public NodeViewModel()
        {
        }

        private NetworkViewModel _Network;
        public NetworkViewModel Network
        {
            get { return _Network; }
            set
            {
                if (_Network != value)
                {
                    _Network = value;
                    OnPropertyChanged("Network");
                }
            }
        }

        public void InvalidateNode()
        {
            this.Name = ChatNode.Name;
            if (string.IsNullOrWhiteSpace(Name))
            {
                if (ChatNode.Sections != null && ChatNode.Sections.Count > 0)
                    this.Name = string.Join("+", ChatNode.Sections.Select(x => x.SectionType).Distinct());
                else
                    this.Name = ChatNode.NodeType.ToString();
            }
            if (this.Network != null)
            {
                var connsToRemove = this.Network.Connections.Where(x => this.OutputConnectors.Contains(x.DestConnector) || this.OutputConnectors.Contains(x.SourceConnector)).ToArray();
                this.Network.Connections.RemoveRange(connsToRemove);
            }


            this.OutputConnectors.Clear();
            foreach (var button in ChatNode.Buttons)
                this.OutputConnectors.Add(new ConnectorViewModel(button));

            Utilities.FillConnectionsFromButtonsOfChatNode(this);
        }

        private ChatNode _chatNode { get; set; }
        public ChatNode ChatNode
        {
            get
            {
                return _chatNode;
            }
            set
            {
                if (_chatNode == value) return;
                _chatNode = value;

                OnChatNodeChanged();
                OnPropertyChanged("ChatNode");
            }
        }

        private void OnChatNodeChanged()
        {
            if (ChatNode != null)
            {
                ChatNode.PropertyChanged -= ChatNode_PropertyChanged;
                ChatNode.PropertyChanged += ChatNode_PropertyChanged;

                ChatNode.Buttons.CollectionChanged -= Buttons_CollectionChanged;
                ChatNode.Buttons.CollectionChanged += Buttons_CollectionChanged;

                InvalidateNode();
            }
        }

        public NodeViewModel(ChatNode chatNode, Point? location)
        {
            ChatNode = chatNode;
            InputConnectors.Add(new ConnectorViewModel(""));
            if (location != null)
            {
                X = location.Value.X;
                Y = location.Value.Y;
            }
        }

        private void Buttons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateNode();
        }

        private void ChatNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateNode();
        }

        private void NodeViewModel_SizeChanged(object sender, EventArgs e)
        {
            this.X -= this.Size.Width / 2;
            this.Y -= this.Size.Height / 2;
            SizeChanged -= NodeViewModel_SizeChanged;
        }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;

                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                if (x == value)
                {
                    return;
                }

                x = value;

                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (y == value)
                {
                    return;
                }

                y = value;

                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return zIndex;
            }
            set
            {
                if (zIndex == value)
                {
                    return;
                }

                zIndex = value;

                OnPropertyChanged("ZIndex");
            }
        }

        /// <summary>
        /// The size of the node.
        /// 
        /// Important Note: 
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size == value)
                {
                    return;
                }

                size = value;

                SizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Event raised when the size of the node is changed.
        /// The size will change when the UI has determined its size based on the contents
        /// of the nodes data-template.  It then pushes the size through to the view-model
        /// and this 'SizeChanged' event occurs.
        /// </summary>
        public event EventHandler<EventArgs> SizeChanged;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> InputConnectors
        {
            get
            {
                if (inputConnectors == null)
                {
                    inputConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    inputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsAdded);
                    inputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsRemoved);
                }

                return inputConnectors;
            }
        }

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> OutputConnectors
        {
            get
            {
                if (outputConnectors == null)
                {
                    outputConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    outputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsAdded);
                    outputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsRemoved);
                }

                return outputConnectors;
            }
        }

        /// <summary>
        /// A helper property that retrieves a list (a new list each time) of all connections attached to the node. 
        /// </summary>
        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                List<ConnectionViewModel> attachedConnections = new List<ConnectionViewModel>();

                foreach (var connector in this.InputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                foreach (var connector in this.OutputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                return attachedConnections;
            }
        }

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                {
                    return;
                }
                isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }

        #region Private Methods

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void inputConnectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = ConnectorType.Input;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void inputConnectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = ConnectorType.Undefined;
            }
        }

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void outputConnectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = ConnectorType.Output;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void outputConnectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = ConnectorType.Undefined;
                connector.Button = null;
                //foreach (var old in connector.AttachedConnections.Where(x => x.SourceConnector == connector))
                //    connector.AttachedConnections.Remove(old);
            }
        }

        #endregion Private Methods
    }
}
