using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using ANAConversationStudio.Models;
using System.Windows;
using System.Diagnostics;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Helpers;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MongoDB.Bson;

namespace ANAConversationStudio.ViewModels
{
	/// <summary>
	/// The view-model for the main window.
	/// </summary>
	public partial class MainWindowViewModel : AbstractModelBase
	{
		#region Internal Data Members

		/// <summary>
		/// This is the network that is displayed in the window.
		/// It is the main part of the view-model.
		/// </summary>
		public NetworkViewModel network = null;

		///
		/// The current scale at which the content is being viewed.
		/// 
		private double contentScale = 1;

		///
		/// The X coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		private double contentOffsetX = 0;

		///
		/// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		private double contentOffsetY = 0;

		///
		/// The width of the content (in content coordinates).
		/// 
		private double contentWidth = 1000;

		///
		/// The heigth of the content (in content coordinates).
		/// 
		private double contentHeight = 1000;

		///
		/// The width of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		private double contentViewportWidth = 0;

		///
		/// The height of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		private double contentViewportHeight = 0;

		#endregion Internal Data Members

		/// <summary>
		/// This is the network that is displayed in the window.
		/// It is the main part of the view-model.
		/// </summary>
		public NetworkViewModel Network
		{
			get
			{
				return network;
			}
			set
			{
				network = value;

				OnPropertyChanged("Network");
			}
		}

		///
		/// The current scale at which the content is being viewed.
		/// 
		public double ContentScale
		{
			get
			{
				return contentScale;
			}
			set
			{
				contentScale = value;

				OnPropertyChanged("ContentScale");
			}
		}

		///
		/// The X coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		public double ContentOffsetX
		{
			get
			{
				return contentOffsetX;
			}
			set
			{
				contentOffsetX = value;

				OnPropertyChanged("ContentOffsetX");
			}
		}

		///
		/// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
		/// 
		public double ContentOffsetY
		{
			get
			{
				return contentOffsetY;
			}
			set
			{
				contentOffsetY = value;

				OnPropertyChanged("ContentOffsetY");
			}
		}

		///
		/// The width of the content (in content coordinates).
		/// 
		public double ContentWidth
		{
			get
			{
				return contentWidth;
			}
			set
			{
				contentWidth = value;

				OnPropertyChanged("ContentWidth");
			}
		}

		///
		/// The heigth of the content (in content coordinates).
		/// 
		public double ContentHeight
		{
			get
			{
				return contentHeight;
			}
			set
			{
				contentHeight = value;

				OnPropertyChanged("ContentHeight");
			}
		}

		///
		/// The width of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		public double ContentViewportWidth
		{
			get
			{
				return contentViewportWidth;
			}
			set
			{
				contentViewportWidth = value;

				OnPropertyChanged("ContentViewportWidth");
			}
		}

		///
		/// The heigth of the viewport onto the content (in content coordinates).
		/// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
		/// view-model so that the value can be shared with the overview window.
		/// 
		public double ContentViewportHeight
		{
			get
			{
				return contentViewportHeight;
			}
			set
			{
				contentViewportHeight = value;

				OnPropertyChanged("ContentViewportHeight");
			}
		}

		/// <summary>
		/// Called when the user has started to drag out a connector, thus creating a new connection.
		/// </summary>
		public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
		{
			//
			// Create a new connection to add to the view-model.
			//
			var connection = new ConnectionViewModel();
			connection.ConnectionChanged += Utilities.ConnectionViewModelConnectionChanged;

			if (draggedOutConnector.Type == ConnectorType.Output)
			{
				//
				// The user is dragging out a source connector (an output) and will connect it to a destination connector (an input).
				//
				connection.SourceConnector = draggedOutConnector;
				connection.DestConnectorHotspot = curDragPoint;
			}
			else
			{
				//
				// The user is dragging out a destination connector (an input) and will connect it to a source connector (an output).
				//
				connection.DestConnector = draggedOutConnector;
				connection.SourceConnectorHotspot = curDragPoint;
			}

			//
			// Add the new connection to the view-model.
			//
			this.Network.Connections.Add(connection);

			return connection;
		}

		/// <summary>
		/// Called to query the application for feedback while the user is dragging the connection.
		/// </summary>
		public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out object feedbackIndicator, out bool connectionOk)
		{
			if (draggedOutConnector == draggedOverConnector)
			{
				//
				// Can't connect to self!
				// Provide feedback to indicate that this connection is not valid!
				//
				feedbackIndicator = new ConnectionBadIndicator();
				connectionOk = false;
			}
			else
			{
				var sourceConnector = draggedOutConnector;
				var destConnector = draggedOverConnector;

				//
				// Only allow connections from output connector to input connector (ie each
				// connector must have a different type).
				//
				//(Also only allocation from one node to another, never one node back to the same node.) Condition Removed
				//connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode && 
				connectionOk = sourceConnector.Type != destConnector.Type;

				if (connectionOk)
				{
					// 
					// Yay, this is a valid connection!
					// Provide feedback to indicate that this connection is ok!
					//
					feedbackIndicator = new ConnectionOkIndicator();
				}
				else
				{
					//
					// Connectors with the same connector type (eg input & input, or output & output)
					// can't be connected.
					// Only connectors with separate connector type (eg input & output).
					// Provide feedback to indicate that this connection is not valid!
					//
					feedbackIndicator = new ConnectionBadIndicator();
				}
			}
		}

		/// <summary>
		/// Called as the user continues to drag the connection.
		/// </summary>
		public void ConnectionDragging(Point curDragPoint, ConnectionViewModel connection)
		{
			if (connection.DestConnector == null)
			{
				connection.DestConnectorHotspot = curDragPoint;
			}
			else
			{
				connection.SourceConnectorHotspot = curDragPoint;
			}
		}

		/// <summary>
		/// Called when the user has finished dragging out the new connection.
		/// </summary>
		public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
		{
			if (connectorDraggedOver == null)
			{
				//
				// The connection was unsuccessful.
				// Maybe the user dragged it out and dropped it in empty space.
				//
				this.Network.Connections.Remove(newConnection);
				return;
			}

			//
			// Only allow connections from output connector to input connector (ie each
			// connector must have a different type).
			// 
			//(Also only allocation from one node to another, never one node back to the same node.) Condition Removed
			//connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode && 
			bool connectionOk = connectorDraggedOut.Type != connectorDraggedOver.Type;

			if (!connectionOk)
			{
				//
				// Connections between connectors that have the same type,
				// eg input -> input or output -> output, are not allowed,
				// Remove the connection.
				//
				this.Network.Connections.Remove(newConnection);
				return;
			}

			//
			// The user has dragged the connection on top of another valid connector.
			//

			//
			// Remove any existing connection between the same two connectors.
			//
			var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
			if (existingConnection != null)
			{
				this.Network.Connections.Remove(existingConnection);
			}

			//
			// Finalize the connection by attaching it to the connector
			// that the user dragged the mouse over.
			//
			if (newConnection.DestConnector == null)
			{
				newConnection.DestConnector = connectorDraggedOver;
			}
			else
			{
				newConnection.SourceConnector = connectorDraggedOver;
			}
		}

		/// <summary>
		/// Retrieve a connection between the two connectors.
		/// Returns null if there is no connection between the connectors.
		/// </summary>
		public ConnectionViewModel FindConnection(ConnectorViewModel connector1, ConnectorViewModel connector2)
		{
			Trace.Assert(connector1.Type != connector2.Type);

			//
			// Figure out which one is the source connector and which one is the
			// destination connector based on their connector types.
			//
			var sourceConnector = connector1.Type == ConnectorType.Output ? connector1 : connector2;
			var destConnector = connector1.Type == ConnectorType.Output ? connector2 : connector1;

			//
			// Now we can just iterate attached connections of the source
			// and see if it each one is attached to the destination connector.
			//

			foreach (var connection in sourceConnector.AttachedConnections)
			{
				if (connection.DestConnector == destConnector)
				{
					//
					// Found a connection that is outgoing from the source connector
					// and incoming to the destination connector.
					//
					return connection;
				}
			}

			return null;
		}

		/// <summary>
		/// Delete the currently selected nodes from the view-model.
		/// </summary>
		public void DeleteSelectedNodes()
		{
			if (MessageBox.Show("Do you want to delete " + this.Network.Nodes.Count(x => x.IsSelected) + " selected node(s)?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				// Take a copy of the selected nodes list so we can delete nodes while iterating.
				var nodesCopy = this.Network.Nodes.ToArray();
				foreach (var node in nodesCopy)
				{
					if (node.IsSelected)
					{
						DeleteNode(node, false);
					}
				}
			}
		}

		/// <summary>
		/// Delete the node from the view-model.
		/// Also deletes any connections to or from the node.
		/// </summary>
		public void DeleteNode(NodeViewModel node, bool askConfirmation = true)
		{
			if (askConfirmation)
				if (MessageBox.Show("Do you want to delete '" + node.Name + "' node?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
					return;

			//
			// Remove all connections attached to the node.
			//
			this.Network.Connections.RemoveRange(node.AttachedConnections);

			//
			// Remove the node from the network.
			//
			this.Network.Nodes.Remove(node);
		}

		#region Default
		///// <summary>
		///// Create a node and add it to the view-model.
		///// </summary>
		//public NodeViewModel CreateNode(string name, Point nodeLocation, bool centerNode)
		//{
		//    var node = new NodeViewModel(name);
		//    node.X = nodeLocation.X;
		//    node.Y = nodeLocation.Y;

		//    node.InputConnectors.Add(new ConnectorViewModel("Input"));
		//    node.OutputConnectors.Add(new ConnectorViewModel("Button 1"));
		//    node.OutputConnectors.Add(new ConnectorViewModel("Button 2"));

		//    #region MyRegion
		//    //if (centerNode)
		//    //{
		//    //    // 
		//    //    // We want to center the node.
		//    //    //
		//    //    // For this to happen we need to wait until the UI has determined the 
		//    //    // size based on the node's data-template.
		//    //    //
		//    //    // So we define an anonymous method to handle the SizeChanged event for a node.
		//    //    //
		//    //    // Note: If you don't declare sizeChangedEventHandler before initializing it you will get
		//    //    //       an error when you try and unsubscribe the event from within the event handler.
		//    //    //
		//    //    EventHandler<EventArgs> sizeChangedEventHandler = null;
		//    //    sizeChangedEventHandler =
		//    //        delegate (object sender, EventArgs e)
		//    //        {
		//    //            //
		//    //            // This event handler will be called after the size of the node has been determined.
		//    //            // So we can now use the size of the node to modify its position.
		//    //            //
		//    //            node.X -= node.Size.Width / 2;
		//    //            node.Y -= node.Size.Height / 2;

		//    //            //
		//    //            // Don't forget to unhook the event, after the initial centering of the node
		//    //            // we don't need to be notified again of any size changes.
		//    //            //
		//    //            node.SizeChanged -= sizeChangedEventHandler;
		//    //        };

		//    //    //
		//    //    // Now we hook the SizeChanged event so the anonymous method is called later
		//    //    // when the size of the node has actually been determined.
		//    //    //
		//    //    node.SizeChanged += sizeChangedEventHandler;
		//    //} 
		//    #endregion

		//    //
		//    // Add the node to the view-model.
		//    //
		//    this.Network.Nodes.Add(node);

		//    return node;
		//} 
		#endregion
		public NodeViewModel CreateNode(ChatNode chatNode, Point nodeLocation)
		{
			if (this.Network?.Nodes == null)
			{
				MessageBox.Show("Not connected to chat server or no project is loaded. If this is the first time, create a new project from file menu and get started. Please close the app and try again if the issue continues.");
				return null;
			}
			var node = new NodeViewModel(chatNode, nodeLocation);
			this.Network.Nodes.Add(node);
			return node;
		}

		/// <summary>
		/// Utility method to delete a connection from the view-model.
		/// </summary>
		public void DeleteConnection(ConnectionViewModel connection)
		{
			if (MessageBox.Show("Do you want to remove the connection?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				this.Network.Connections.Remove(connection);
		}
	}

	public partial class MainWindowViewModel : AbstractModelBase
	{
		public MainWindowViewModel()
		{
			StudioContext.StaticPropertyChanged -= StudioContext_StaticPropertyChanged;
			StudioContext.StaticPropertyChanged += StudioContext_StaticPropertyChanged; ;
		}

		private void StudioContext_StaticPropertyChanged(object sender, EventArgs e)
		{
			CurrentStudioContext = StudioContext.Current;
			if (CurrentStudioContext != null)
			{
				CurrentStudioContext.PropertyChanged -= Current_PropertyChanged;
				CurrentStudioContext.PropertyChanged += Current_PropertyChanged;
			}
		}

		private void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(StudioContext.ChatFlow))
			{
				if (CurrentStudioContext.ChatFlow == null)
					ClearDesigner();
			}
		}

		~MainWindowViewModel()
		{
			StudioContext.StaticPropertyChanged -= StudioContext_StaticPropertyChanged;
		}

		private StudioContext _CurrentStudioContext;
		public StudioContext CurrentStudioContext
		{
			get { return _CurrentStudioContext; }
			set
			{
				if (_CurrentStudioContext != value)
				{
					_CurrentStudioContext = value;
					OnPropertyChanged(nameof(CurrentStudioContext));
				}
			}
		}

		private ObservableCollection<ChatFlowSearchItem> _SearchResults;
		public ObservableCollection<ChatFlowSearchItem> SearchResults
		{
			get { return _SearchResults; }
			set
			{
				if (_SearchResults != value)
				{
					_SearchResults = value;
					OnPropertyChanged(nameof(SearchResults));
				}
			}
		}

		public void SearchInNodes(string keywords)
		{
			if (string.IsNullOrWhiteSpace(keywords) || Network?.Nodes == null) return;
			SearchResults = new ObservableCollection<ChatFlowSearchItem>(Network.Nodes.Select(node => node.ChatNode.SearchNode(keywords)).Where(x => x != null));
			if (SearchResults.Count == 0)
				SearchResults = new ObservableCollection<ChatFlowSearchItem>
				{
					new ChatFlowSearchItem { NoResults = true  }
				};
		}

		public void ParseOutChanges()
		{
			StudioContext.Current.ChatFlow.NodeLocations = this.Network.Nodes.ToDictionary(x => x.ChatNode.Id, x => new LayoutPoint(x.X, x.Y));
			StudioContext.Current.ChatFlow.ChatNodes = this.Network.Nodes.Select(x => x.ChatNode).ToList();
			StudioContext.Current.ChatFlow.ChatContent = Utilities.ExtractContentFromChatNodes(StudioContext.Current.ChatFlow.ChatNodes);
		}

		public async Task SaveLoadedChat()
		{
			ParseOutChanges();
			await StudioContext.Current.SaveChatFlowAsync();
		}

		public bool LoadNodesIntoDesigner()
		{
			try
			{
				if (StudioContext.Current?.ChatFlow?.ChatNodes == null)
				{
					MessageBox.Show("Connection with chat server is not established or no project is selected. If this is the first time, create a new project from file menu and get started. Please restart the application and try again if the issue continues.", "Oops!");
					return false;
				}

				this.Network = new NetworkViewModel();
				this.Network.Connections.CollectionChanged += Connections_CollectionChanged;

				var start = new Point(100, 60);
				var allChatNodes = StudioContext.Current.ChatFlow.ChatNodes;
				var uniqueChatNodes = allChatNodes.GroupBy(x => x.Id).SelectMany(x => x).OrderByDescending(x => x.IsStartNode).ToList();

				var nodeVMs = uniqueChatNodes.Select(x => new NodeViewModel(x, StudioContext.Current.ChatFlow.NodeLocations.GetPointForNode(x.Id) ?? new Point(start.X, start.Y += 100))).ToList(); //Dont remove .ToList()
				this.Network.Nodes.AddRange(nodeVMs); //Add all nodes before adding any connections  

				if (Network.Nodes.Count == 0)
					CreateNode(new ChatNode() { Id = ObjectId.GenerateNewId().ToString(), Name = "New Node" }, new Point(ContentWidth / 2, ContentHeight / 2));

				foreach (var node in nodeVMs)
					Utilities.FillConnectionsFromButtonsOfChatNode(node);

				return true;
			}
			catch (Exception ex)
			{
				if (!Utilities.IsDesignMode())
					MessageBox.Show(ex.ToString(), "Initialization error");
			}
			return false;
		}
		public void ClearDesigner()
		{
			this.Network = null;
		}
		private void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			try
			{
				if (!this.Network.ActOnButtonNextNodeIds) return;
				if (e.NewItems != null)
					foreach (ConnectionViewModel item in e.NewItems)
					{
						var sourceBtn = item.SourceConnector.Button;
						if (item.DestConnector != null)
						{
							var destinationNode = item.DestConnector.ParentNode.ChatNode;
							sourceBtn.NextNodeId = destinationNode.Id;
						}
					}
				if (e.OldItems != null)
					foreach (ConnectionViewModel item in e.OldItems)
					{
						var sourceBtn = item.SourceConnector.Button;
						sourceBtn.NextNodeId = null;
					}
			}
			catch (Exception ex)
			{
				if (!Utilities.IsDesignMode())
					MessageBox.Show(ex.ToString(), "Connections Update error");
			}
		}

	}
}
