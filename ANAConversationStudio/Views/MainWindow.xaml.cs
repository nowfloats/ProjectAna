using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetworkUI;
using ZoomAndPan;
using System.Collections;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Helpers;
using MongoDB.Bson;
using ANAConversationStudio.ViewModels;
using System.Reflection;
using Xceed.Wpf.Toolkit;
using System.Linq;
using System.Windows.Media;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ANAConversationStudio.Views
{
    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Current = this;
            InitializeComponent();
        }

        public static MainWindow Current { get; set; }
        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)DataContext;
            }
        }

        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkControl);

            //
            // Delegate the real work to the view model.
            //
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void networkControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var draggedOverConnector = (ConnectorViewModel)e.DraggedOverConnector;
            object feedbackIndicator = null;
            bool connectionOk = true;

            this.ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

            //
            // Return the feedback object to NetworkView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            //
            e.FeedbackIndicator = feedbackIndicator;

            //
            // Let NetworkView know if the connection is ok or not ok.
            //
            e.ConnectionOk = connectionOk;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            Point curDragPoint = Mouse.GetPosition(networkControl);
            var connection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            var newConnection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);

        }

        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.DeleteSelectedNodes();
            ResetEditors();
        }

        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNode();
        }

        /// <summary>
        /// Event raised to delete a node.
        /// </summary>
        private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var node = (NodeViewModel)e.Parameter;
            this.ViewModel.DeleteNode(node);
            ResetEditors();
        }

        /// <summary>
        /// Event raised to delete a connection.
        /// </summary>
        private void DeleteConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var connection = (ConnectionViewModel)e.Parameter;
            this.ViewModel.DeleteConnection(connection);
        }

        /// <summary>
        /// Creates a new node in the network at the current mouse location.
        /// </summary>
        private void CreateNode()
        {
            var newNodePosition = Mouse.GetPosition(networkControl);
            this.ViewModel.CreateNode(new ChatNode() { Id = ObjectId.GenerateNewId().ToString() }, newNodePosition);
        }

        /// <summary>
        /// Event raised when the size of a node has changed.
        /// </summary>
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            var element = (FrameworkElement)sender;
            var node = (NodeViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CloneSelectedNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cloneSelectedNodes();
        }
        private void cloneSelectedNodes()
        {
            if (networkControl.SelectedNode == null)
            {
                System.Windows.MessageBox.Show("Please select a node to clone it", "No node selected");
                return;
            }
            if (networkControl.SelectedNodes != null)
                foreach (var node in networkControl.SelectedNodes)
                    Shortcuts.CloneNode(node as NodeViewModel);
        }
        private void CloneSelectedNodeClick(object sender, RoutedEventArgs e)
        {
            cloneSelectedNodes();
        }

        private void ConvSimMenuClick(object sender, RoutedEventArgs e)
        {
            var p = System.Diagnostics.Process.Start("anaconsim://");
        }

        private async void UpdateMenuClick(object sender, RoutedEventArgs e)
        {
            var updateInfo = (AutoUpdateResponse)UpdateMenuItem.Tag;
            if (updateInfo != null)
            {
                using (var wc = new WebClient())
                {
                    var tempFile = Path.GetTempFileName();
                    File.Delete(tempFile);
                    wc.DownloadProgressChanged += (s, args) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Status($"Downloading {updateInfo.Version}: {args.ProgressPercentage}% ...");
                        });
                    };
                    Status("Downloading...");
                    await wc.DownloadFileTaskAsync(updateInfo.DownloadLink, tempFile);
                    Status("Download Complete");
                    if (System.Windows.MessageBox.Show("Update is ready to be installed. Click ok to install. It will close the application. You can start the studio as soon as it's extraction is done", "Update", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        var tempPath = Path.GetTempPath();
                        var srcPath = Path.Combine(Environment.CurrentDirectory, "Tools");
                        foreach (string newPath in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
                            File.Copy(newPath, Path.Combine(tempPath, Path.GetFileName(newPath)), true);

                        var extractorFilePath = Path.Combine(tempPath, "extract.bat");

                        var commandLine = $"\"{tempFile}\" \"{Environment.CurrentDirectory}\"";
                        AskAlert = false;
                        Application.Current.Exit += (s, args) =>
                        {
                            var psi = new ProcessStartInfo
                            {
                                Arguments = commandLine,
                                FileName = extractorFilePath,
                                WorkingDirectory = tempPath
                            };
                            Process.Start(psi);
                        };
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(Utilities.Settings.UpdateDetails);
            sw.ShowDialog();
            if (sw.Save)
            {
                Utilities.Settings.UpdateDetails = sw.Config;
                Utilities.Settings.Save(App.Cryptio);
            }
        }

        private void NextNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var position = 1;
            if (Keyboard.IsKeyDown(Key.D2))
                position = 2;
            if (Keyboard.IsKeyDown(Key.D3))
                position = 3;
            if (Keyboard.IsKeyDown(Key.D4))
                position = 4;
            GotoNextNode(position);
        }

        private void PrevNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var position = 1;

            if (Keyboard.IsKeyDown(Key.D2))
                position = 2;
            if (Keyboard.IsKeyDown(Key.D3))
                position = 3;
            if (Keyboard.IsKeyDown(Key.D4))
                position = 4;
            GotoPreviousNode(position);
        }

        private void NewChatFlowClick(object sender, RoutedEventArgs e)
        {
            ChatFlowsManagerWindow w = new ChatFlowsManagerWindow();
            w.ShowDialog();
        }

        private void ManageChatFlowsClick(object sender, RoutedEventArgs e)
        {
            ChatFlowsManagerWindow w = new ChatFlowsManagerWindow();
            w.ShowDialog();
        }

        private void ConvSimWithChatMenuClick(object sender, RoutedEventArgs e)
        {
            StartChatInSimulator();
        }

        private void StartChatInSimulator()
        {
            if (StudioContext.Current?.ChatServer?.ServerUrl == null)
            {
                System.Windows.MessageBox.Show("No project is loaded at the moment");
                return;
            }
            Process.Start("anaconsim://app?chatflow=" + Uri.EscapeDataString(StudioContext.Current.ChatServer.ServerUrl + "/api/Conversation/chat?projectId=" + StudioContext.Current.ChatFlow.ProjectId));
        }

        private void StartInSimulator_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StartChatInSimulator();
        }
    }

    /// <summary>
    /// This is a partial implementation of MainWindow that just contains most of the zooming and panning functionality.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect prevZoomRect;

        /// <summary>
        /// Save the previous content scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool prevZoomRectSet = false;

        /// <summary>
        /// Event raised on mouse down in the NetworkView.
        /// </summary> 
        private void networkControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            networkControl.Focus();
            Keyboard.Focus(networkControl);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(networkControl);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left &&
                     (Keyboard.Modifiers & ModifierKeys.Control) == 0)
            {
                //
                // Initiate panning, when control is not held down.
                // When control is held down left dragging is used for drag selection.
                // After panning has been initiated the user must drag further than the threshold value to actually start drag panning.
                //
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                networkControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the NetworkView.
        /// </summary>
        private void networkControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Panning)
                {
                    //
                    // Panning was initiated but dragging was abandoned before the mouse
                    // cursor was dragged further than the threshold distance.
                    // This means that this basically just a regular left mouse click.
                    // Because it was a mouse click in empty space we need to clear the current selection.
                    //
                }
                else if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect();
                }

                //
                // Reenable clearing of selection when empty space is clicked.
                // This is disabled when drag panning is in progress.
                //
                networkControl.IsClearSelectionOnEmptySpaceClickEnabled = true;

                //
                // Reset the override cursor.
                // This is set to a special cursor while drag panning is in progress.
                //
                Mouse.OverrideCursor = null;

                networkControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the NetworkView.
        /// </summary>
        private void networkControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (Math.Abs(dragOffset.X) > dragThreshold ||
                    Math.Abs(dragOffset.Y) > dragThreshold)
                {
                    //
                    // The user has dragged the cursor further than the threshold distance, initiate
                    // drag panning.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragPanning;
                    networkControl.IsClearSelectionOnEmptySpaceClickEnabled = false;
                    Mouse.OverrideCursor = Cursors.ScrollAll;
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragPanning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(networkControl);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                    Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragZooming;
                    Point curContentMousePoint = e.GetPosition(networkControl);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                //
                Point curContentMousePoint = e.GetPosition(networkControl);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel.
        /// </summary>
        private void networkControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(networkControl);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(networkControl);
                ZoomOut(curContentMousePoint);
            }
        }

        /// <summary>
        /// Event raised when the user has double clicked in the zoom and pan control.
        /// </summary>
        private void networkControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Pan on double click disabled
            //if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            //{
            //    Point doubleClickPoint = e.GetPosition(networkControl);
            //    zoomAndPanControl.AnimatedSnapTo(doubleClickPoint);
            //}

            OpenNodeEditor();
        }

        private void OpenNodeEditor()
        {
            if (this.ViewModel.SelectedChatNode != null)
                NodeEditorLayoutAnchorable.IsActive = true;
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var o = networkControl.SelectedNode;

            ZoomIn(new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut(new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'JumpBackToPrevZoom' command was executed.
        /// </summary>
        private void JumpBackToPrevZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JumpBackToPrevZoom();
        }

        /// <summary>
        /// Determines whether the 'JumpBackToPrevZoom' command can be executed.
        /// </summary>
        private void JumpBackToPrevZoom_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = prevZoomRectSet;
        }

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void FitContent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (this.ViewModel.Network == null) return;

                IList nodes = null;

                if (networkControl.SelectedNodes.Count > 0)
                {
                    nodes = networkControl.SelectedNodes;
                }
                else
                {
                    nodes = this.ViewModel.Network.Nodes;
                    if (nodes.Count == 0)
                    {
                        return;
                    }
                }

                SavePrevZoomRect();

                Rect actualContentRect = DetermineAreaOfNodes(nodes);

                //
                // Inflate the content rect by a fraction of the actual size of the total content area.
                // This puts a nice border around the content we are fitting to the viewport.
                //
                actualContentRect.Inflate(networkControl.ActualWidth / 40, networkControl.ActualHeight / 40);

                zoomAndPanControl.AnimatedZoomTo(actualContentRect);
            }
            catch { }
        }

        /// <summary>
        /// Determine the area covered by the specified list of nodes.
        /// </summary>
        private Rect DetermineAreaOfNodes(IList nodes)
        {
            NodeViewModel firstNode = (NodeViewModel)nodes[0];
            Rect actualContentRect = new Rect(firstNode.X, firstNode.Y, firstNode.Size.Width, firstNode.Size.Height);

            for (int i = 1; i < nodes.Count; ++i)
            {
                NodeViewModel node = (NodeViewModel)nodes[i];
                Rect nodeRect = new Rect(node.X, node.Y, node.Size.Width, node.Size.Height);
                actualContentRect = Rect.Union(actualContentRect, nodeRect);
            }
            return actualContentRect;
        }

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel.Network == null) return;

            SavePrevZoomRect();

            zoomAndPanControl.AnimatedScaleToFit();
        }

        /// <summary>
        /// The 'OneHundredPercent' command was executed.
        /// </summary>
        private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ViewModel.Network == null) return;

            SavePrevZoomRect();

            zoomAndPanControl.AnimatedZoomTo(1.0);
        }

        /// <summary>
        /// Jump back to the previous zoom level.
        /// </summary>
        private void JumpBackToPrevZoom()
        {
            zoomAndPanControl.AnimatedZoomTo(prevZoomScale, prevZoomRect);

            ClearPrevZoomRect();
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            if (this.ViewModel.Network == null) return;

            zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            if (this.ViewModel.Network == null) return;

            zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Initialize the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retreive the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            zoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            FadeOutDragZoomRect();
        }

        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, Border.OpacityProperty, 0.0, 0.1,
                delegate (object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        //
        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);
            prevZoomScale = zoomAndPanControl.ContentScale;
            prevZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }
    }

    /// <summary>
    /// Extended code for Conversation Studio
    /// </summary>
    public partial class MainWindow : Window
    {
        public CollectionControl NodeCollectionControl
        {
            get { return SectionButtonEditor.Content as CollectionControl; }
            set
            {
                SectionButtonEditor.Content = null;
                SectionButtonEditor.Content = value;
            }
        }
        private bool AskPass()
        {
            if (!Settings.IsEncrypted())
            {
                SetPassword sp = new SetPassword();
                sp.ShowDialog();
                return sp.Success;
            }
            else
            {
                EnterPassword wp = new EnterPassword();
                wp.ShowDialog();
                return wp.Success;
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            if (!AskPass()) return;

            this.IsEnabled = true;
            AskToSelectChatServer();
            LoadProjects();
            CheckForUpdates();
            UpdateTitle();
            #region Overview Windows Commented
            //OverviewWindow overviewWindow = new OverviewWindow();
            //overviewWindow.Left = this.Left;
            //overviewWindow.Top = this.Top + this.Height + 5;
            //overviewWindow.Owner = this;
            //overviewWindow.DataContext = this.ViewModel; // Pass the view model onto the overview window.
            //overviewWindow.Show(); 
            #endregion
        }
        private void UpdateTitle()
        {
            var chatServerName = StudioContext.Current?.ChatServer?.Name;
            var chatProjectName = StudioContext.Current?.ChatFlowProjects?.FirstOrDefault(x => x._id == StudioContext.Current?.ChatFlow?.ProjectId)?.Name;

            var title = $"{chatServerName} : {chatProjectName}";
            if (string.IsNullOrWhiteSpace(chatServerName) || string.IsNullOrWhiteSpace(chatProjectName))
                Title = $"ANA Conversation Studio {GetVersion()}";
            else
                Title = $"{title} - ANA Conversation Studio {GetVersion()}";
        }
        private void AskToSelectChatServer()
        {
            SaveChatServersManager man = new SaveChatServersManager();
            man.ShowDialog();
        }

        private Version GetVersion() => Assembly.GetExecutingAssembly().GetName().Version;
        private async void CheckForUpdates()
        {
            var info = await Utilities.GetLatestVersionInfo();
            if (info != null && info.Version > GetVersion())
            {
                HelpMenu.Background = new SolidColorBrush(Colors.Crimson);
                HelpMenu.Foreground = new SolidColorBrush(Colors.White);
                UpdateMenuItem.Header = $"Update {info.Version} available!";
                UpdateMenuItem.Foreground = new SolidColorBrush(Colors.Black);
                UpdateMenuItem.IsEnabled = true;
                UpdateMenuItem.Tag = info;
            }
            else
            {
                HelpMenu.Background = null;
                HelpMenu.Foreground = new SolidColorBrush(Colors.Black);
                UpdateMenuItem.Header = $"No update available!";
                UpdateMenuItem.IsEnabled = false;
            }
        }
        public bool AskAlert = true;
        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!AskAlert) return;
            if (StudioContext.Current == null) return;
            var op = System.Windows.MessageBox.Show("Save changes?", "Hold on!", MessageBoxButton.YesNoCancel);
            if (op == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (op == MessageBoxResult.Yes)
            {
                await SaveEditsAsync();
            }
        }

        private void networkControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (networkControl.SelectedNode != null)
            {
                var node = (networkControl.SelectedNode as NodeViewModel);
                if (this.ViewModel.SelectedChatNode != null)
                    this.ViewModel.SelectedChatNode.PropertyChanged -= SelectedChatNode_PropertyChanged; //Remove old event handler

                this.ViewModel.SelectedChatNode = null;
                this.ViewModel.SelectedChatNode = node.ChatNode;
                this.ViewModel.SelectedChatNode.PropertyChanged += SelectedChatNode_PropertyChanged;
                NodeCollectionControl = null;
            }
        }

        private void SelectedChatNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var senderNode = sender as ChatNode;
            if (e.PropertyName == nameof(ChatNode.IsStartNode)) //When is start node is changed
                this.ViewModel.Network.Nodes.Select(x => x.ChatNode).Where(x => x.Id != senderNode.Id && x.IsStartNode).ToList().ForEach(x => x.IsStartNode = false);
        }

        private async void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            await SaveEditsAsync();
        }
        private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await SaveEditsAsync();
        }

        #region TODO: Window Layout Save and Load Functions 
        //private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (File.Exists(Constants.WindowLayoutFileName))
        //    {
        //        try
        //        {
        //            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
        //            layoutSerializer.Deserialize(Constants.WindowLayoutFileName);
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Windows.MessageBox.Show(ex.StackTrace, "Window Layout Read Error: " + ex.Message);
        //        }
        //    }
        //}

        //private void LoadWindowLayout()
        //{
        //    try
        //    {
        //        XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
        //        layoutSerializer.Serialize(Constants.WindowLayoutFileName);
        //    }
        //    catch { }
        //} 
        #endregion

        private async Task SaveEditsAsync()
        {
            try
            {
                if (this.ViewModel.Network == null) return;
                await this.ViewModel.SaveLoadedChat();
                StatusTextBlock.Text = "Saved at " + DateTime.Now.ToShortTimeString();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.StackTrace, "Error: " + ex.Message);
            }
        }

        public bool LoadFileMenuSavedConnections()
        {
            if (StudioContext.Current?.ChatFlowProjects == null || StudioContext.Current?.ChatFlowProjects.Count <= 0)
            {
                SavedConnectionsFileMenu.IsEnabled = false;
                return false;
            }
            SavedConnectionsFileMenu.ItemsSource = StudioContext.Current.ChatFlowProjects;
            SavedConnectionsFileMenu.IsEnabled = true;
            return true;
        }
        private async void LoadProjects()
        {
            if (LoadFileMenuSavedConnections())
                await LoadProjectAsync(StudioContext.Current.ChatFlowProjects.First());
        }
        public void Status(string txt)
        {
            StatusTextblock.Text = txt;
        }
        private async void SavedConnectionClick(object sender, RoutedEventArgs e)
        {
            await LoadProjectAsync((sender as MenuItem).Tag as ANAProject);
        }

        public async Task LoadProjectAsync(ANAProject proj)
        {
            await StudioContext.Current.LoadChatFlowAsync(proj._id);
            this.ViewModel.LoadNodes();
            Status("Loaded");
            UpdateTitle();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Delay(500).ContinueWith((s) =>
            {
                Dispatcher.Invoke(() => this.FitContent_Executed(null, null));
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void ResetEditors()
        {
            this.ViewModel.SelectedChatNode = null;
            NodeCollectionControl = null;
        }

        public void GotoNextNode(int position = 1)
        {
            try
            {
                if (this.ViewModel.SelectedChatNode != null)
                {
                    var nextNodeIds = new List<string>();
                    if (this.ViewModel.SelectedChatNode.Buttons != null && this.ViewModel.SelectedChatNode.Buttons.Count > 0)
                        nextNodeIds.AddRange(this.ViewModel.SelectedChatNode.Buttons.Select(x => x.NextNodeId));
                    if (!string.IsNullOrWhiteSpace(this.ViewModel.SelectedChatNode.NextNodeId))
                        nextNodeIds.Add(this.ViewModel.SelectedChatNode.NextNodeId);
                    if (position <= nextNodeIds.Count)
                    {
                        var node = networkControl.Nodes.Cast<NodeViewModel>().FirstOrDefault(x => x.ChatNode.Id == nextNodeIds[position - 1]);
                        if (node != null)
                        {
                            networkControl.SelectedNode = node;
                            networkControl.BringSelectedNodesIntoView();
                        }
                    }
                }
            }
            catch { }
        }

        public void GotoPreviousNode(int position = 1)
        {
            try
            {
                if (this.ViewModel.SelectedChatNode != null)
                {
                    var currentNodeId = this.ViewModel.SelectedChatNode.Id;

                    var prevNodeIds = networkControl.Nodes.Cast<NodeViewModel>()
                        .Where(x => x.ChatNode.NextNodeId == currentNodeId ||
                        (x.ChatNode.Buttons?.Count(y => y.NextNodeId == currentNodeId) > 0)).Select(x => x.ChatNode.Id).ToList();

                    if (position <= prevNodeIds.Count)
                    {
                        var node = networkControl.Nodes.Cast<NodeViewModel>().FirstOrDefault(x => x.ChatNode.Id == prevNodeIds[position - 1]);
                        if (node != null)
                        {
                            networkControl.SelectedNode = node;
                            networkControl.BringSelectedNodesIntoView();
                        }
                    }
                }
            }
            catch { }
        }
    }
}