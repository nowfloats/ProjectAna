using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Net.Http;
using ANAConversationSimulator.Models.Chat;
using Newtonsoft.Json.Linq;
using ANAConversationSimulator.Models.Chat.Sections;
using Windows.UI.Xaml;
using Windows.ApplicationModel;
using ANAConversationSimulator.Helpers;
using ANAConversationSimulator.UserControls;
using Quobject.SocketIoClientDotNet.Client;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ANAConversationSimulator.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public static MainPageViewModel CurrentInstance { get; set; }
        public MainPageViewModel()
        {
            CurrentInstance = this;

            Utils.InitMemoryStack();
        }

        public string PageTitle => $"{Package.Current.DisplayName} {Utils.VersionDisplay()}";

        ObservableCollection<Section> _chatThread = new ObservableCollection<Section>();
        public ObservableCollection<Section> ChatThread { get { return _chatThread; } set { Set(ref _chatThread, value); } }

        ObservableCollection<Button> _currentClickButtons = new ObservableCollection<Button>();
        public ObservableCollection<Button> CurrentClickButtons { get { return _currentClickButtons; } set { Set(ref _currentClickButtons, value); } }

        ObservableCollection<Button> _currentTextInputButtons = new ObservableCollection<Button>();
        public ObservableCollection<Button> CurrentTextInputButtons { get { return _currentTextInputButtons; } set { Set(ref _currentTextInputButtons, value); } }

        private string currentAPI = "";
        private JArray chatNodes;
        public async Task LoadNodesAsync()
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    if (Utils.APISettings.Values.TryGetValue("API", out object savedAPI) && !string.IsNullOrWhiteSpace(savedAPI + ""))
                    {
                        currentAPI = savedAPI + "";
                        var resp = await hc.GetStringAsync(currentAPI);
                        chatNodes = JArray.Parse(resp);
                    }
                    else
                        UpdateAPI();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }
        public async Task JoinNodesFromFlowAsync(string chatFlowUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chatFlowUrl))
                {
                    Utils.ShowDialog("Given chat flow url is empty! Please provide a correct chat flow url in the button");
                    return;
                }
                using (var hc = new HttpClient())
                {
                    var resp = await hc.GetStringAsync(chatFlowUrl);
                    foreach (var newChatNode in JArray.Parse(resp))
                        chatNodes.Add(newChatNode);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowDialog(ex.Message);
            }
        }
        public JToken GetNodeById(string nodeId)
        {
            return chatNodes.Children().FirstOrDefault(x => x["Id"].ToString() == nodeId);
        }
        public async void StartChatting()
        {
            ClearChatThread();
            SetupSocketConnection();
            ToggleTyping(true);
            await LoadNodesAsync();
            if (chatNodes == null || chatNodes.Count == 0)
                return;

            ProcessNode(chatNodes[0]);
        }
        private DispatcherTimer buttonTimeoutTimer;
        public async void ProcessNode(JToken node, JToken section = null)
        {
            if (node == null)
            {
                Utils.ShowDialog("Node not found!");
                return;
            }
            ClearButtonTimer();

            //Replacing verbs
            node = JToken.Parse(VerbProcessor.Process(node.ToString()));

            var parsedNode = node.ToObject<ChatNode>();
            if (parsedNode.Buttons != null && parsedNode.Buttons.Count > 0)
                ClearButtons();

            if (parsedNode.NodeType == NodeTypeEnum.ApiCall)
            {
                ToggleTyping(true);
                try
                {
                    var paramDict = new Dictionary<string, object>();
                    if (parsedNode.RequiredVariables != null)
                        foreach (var reqParam in parsedNode.RequiredVariables)
                        {
                            if (reqParam == "HISTORY") //Custom Variable
                                paramDict[reqParam] = ChatThread.Where(x => x.SectionType != SectionTypeEnum.Typing).ToArray();
                            else
                                paramDict[reqParam] = ButtonActionHelper.GetSavedValue(reqParam);
                        }
                    var nextNodeId = parsedNode.NextNodeId; //Default
                    switch (parsedNode.ApiMethod.ToUpper())
                    {
                        case "GET":
                            {
                                var query = string.Join("&", paramDict.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value + "")}"));
                                var api = string.IsNullOrWhiteSpace(query) ? parsedNode.ApiUrl : parsedNode.ApiUrl + (parsedNode.ApiUrl?.Contains("?") == true ? "&" : "?") + query;

                                var resp = await APIHelper.HitAsync<JObject>(api);

                                if (!string.IsNullOrWhiteSpace(resp["NextNodeId"] + ""))
                                    nextNodeId = resp["NextNodeId"] + "";

                                ButtonActionHelper.HandleSaveMultiple(resp.ToObject<Dictionary<string, object>>());
                                var apiNextNodeId = ExtractNextNodeIdFromAPIResp(parsedNode, resp);
                                if (!string.IsNullOrWhiteSpace(apiNextNodeId))
                                    nextNodeId = apiNextNodeId;
                            }
                            break;
                        case "POST":
                            {
                                var resp = await APIHelper.HitPostAsync<Dictionary<string, object>, JObject>(parsedNode.ApiUrl, paramDict);
                                if (!string.IsNullOrWhiteSpace(resp["NextNodeId"] + ""))
                                    nextNodeId = resp["NextNodeId"] + "";
                                var apiNextNodeId = ExtractNextNodeIdFromAPIResp(parsedNode, resp);
                                if (!string.IsNullOrWhiteSpace(apiNextNodeId))
                                    nextNodeId = apiNextNodeId;
                            }
                            break;
                        default:
                            Utils.ShowDialog($"{parsedNode.ApiMethod} ApiType Unknown!");
                            break;
                    }
                    NavigateToNode(nextNodeId);
                }
                catch (HttpRequestException ex)
                {
                    ToggleTyping(false);
                    Utils.ShowDialog(ex.ToString());
                    NavigateToNode(parsedNode.NextNodeId);
                }
                catch (Exception ex)
                {
                    ToggleTyping(false);
                    Utils.ShowDialog(ex.ToString());
                    NavigateToNode(parsedNode.NextNodeId);
                }
            }
            else if (node["Sections"] == null || node["Sections"].Children().Count() == 0)
            {
                ToggleTyping(false);
                await ProcessButtonsAsync(node);
            }
            else if (node["Sections"] != null && node["Sections"].Children().Count() > 0)
            {
                var sectionsSource = node["Sections"];
                var currentSectionSource = section ?? sectionsSource.First;

                //Replacing verbs
                currentSectionSource = JToken.Parse(VerbProcessor.Process(currentSectionSource.ToString()));

                SectionTypeEnum secType = (SectionTypeEnum)Enum.Parse(typeof(SectionTypeEnum), currentSectionSource["SectionType"].ToString());
                Section parsedSection = null;
                bool showTyping = false;
                switch (secType)
                {
                    case SectionTypeEnum.Image:
                        parsedSection = currentSectionSource.ToObject<ImageSection>();
                        showTyping = true;
                        break;
                    case SectionTypeEnum.Text:
                        parsedSection = currentSectionSource.ToObject<TextSection>();
                        break;
                    case SectionTypeEnum.Gif:
                        parsedSection = currentSectionSource.ToObject<GifSection>();
                        showTyping = true;
                        break;
                    case SectionTypeEnum.Video:
                        parsedSection = currentSectionSource.ToObject<VideoSection>();
                        break;
                    case SectionTypeEnum.Audio:
                        parsedSection = currentSectionSource.ToObject<AudioSection>();
                        break;
                    case SectionTypeEnum.EmbeddedHtml:
                        parsedSection = currentSectionSource.ToObject<EmbeddedHtmlSection>();
                        break;
                    case SectionTypeEnum.PrintOTP:
                        parsedSection = currentSectionSource.ToObject<PrintOTPSection>();
                        break;
                    case SectionTypeEnum.Carousel:
                        {
                            parsedSection = currentSectionSource.ToObject<CarouselSection>();
                            (parsedSection as CarouselSection).Items = VerbProcessor.ProcessCarousalItems((parsedSection as CarouselSection).Items, parsedNode);
                        }
                        break;
                    case SectionTypeEnum.Link:
                    case SectionTypeEnum.Graph:
                        Utils.ShowDialog($"{secType} Coming soon!");
                        break;
                    default:
                        break;
                }
#if DEBUG
                if (Debugger.IsAttached)
                {
                    if (parsedSection != null)
                        parsedSection.DelayInMs = 0;
                }
#endif
                if (parsedSection != null)
                {
                    if (parsedSection.DelayInMs > 50 || showTyping) //Add 'typing' bubble if delay is greater than 50 ms
                        ToggleTyping(true);

                    //Wait for delay MilliSeconds and then continue with chat
                    Dispatcher.Dispatch(async () =>
                    {
                        var precacheSucess = await PrecacheSection(parsedSection);
                        //Remove 'typing' bubble
                        ToggleTyping(false);
                        var sectionIndex = (sectionsSource.Children().ToList().FindIndex(x => x["_id"].ToString() == parsedSection._id));
                        if (precacheSucess)
                        {
                            if (sectionIndex == 0) //First section in node, send View Event
                            {
                                await Task.Run(async () =>
                                {
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                                    {
                                        try
                                        {
                                            await APIHelper.TrackEvent(Utils.GetViewEvent(parsedNode.Id, Utils.DeviceId));
                                        }
                                        catch (Exception ex)
                                        {
                                            await Utils.ShowDialogAsync(ex.ToString());
                                        }
                                    });
                                });
                            }

                            if (parsedNode.NodeType == NodeTypeEnum.Card)
                            {
                                parsedSection.Title = VerbProcessor.Process(parsedNode.CardHeader);
                                parsedSection.Caption = VerbProcessor.Process(parsedNode.CardFooter);

                                if (parsedNode.Placement == null || parsedNode.Placement == Placement.Incoming)
                                    AddIncommingSection(parsedSection);
                                else if (parsedNode.Placement == Placement.Outgoing)
                                    AddOutgoingSection(parsedSection);
                                else if (parsedNode.Placement == Placement.Center)
                                    AddCenterSection(parsedSection);
                            }
                            else
                                AddIncommingSection(parsedSection);
                        }
                        var remainingSections = sectionsSource.Children().Count() - (sectionIndex + 1);
                        if (remainingSections > 0)
                        {
                            var nextSection = sectionsSource.ElementAt(sectionIndex + 1);
                            ProcessNode(node, nextSection);
                        }
                        else
                            await ProcessButtonsAsync(node);

                    }, parsedSection.DelayInMs);
                }
            }
        }
        public void ClearButtonTimer()
        {
            if (buttonTimeoutTimer != null)
            {
                buttonTimeoutTimer.Stop();
                buttonTimeoutTimer = null;
            }
        }
        public void ClearButtons()
        {
            CurrentClickButtons.Clear();
            CurrentTextInputButtons.Clear();
        }
        public void ClearChatThread()
        {
            ChatThread.Clear();
            ClearButtonTimer();
            ClearButtons();
        }
        public async Task ProcessButtonsAsync(JToken node)
        {
            ClearButtonTimer();
            var parsedNode = node.ToObject<ChatNode>();
            if (parsedNode.Buttons == null || parsedNode.Buttons.Count == 0)
                return;

            ClearButtons();
            var allButtons = node["Buttons"].ToObject<List<Button>>();
            foreach (var btnSrc in allButtons.Where(x => x.Kind == ButtonKind.ClickInput))
            {
                var btn = btnSrc.DeepCopy();
                if (btn.DoesRepeat)
                {
                    var repeatOn = ButtonActionHelper.GetSavedArray(btn.RepeatOn);
                    if (repeatOn != null)
                    {
                        var max = btn.MaxRepeats == 0 ? repeatOn.Count - 1 : btn.MaxRepeats;
                        for (int i = btn.StartPosition; i <= max; i++)
                        {
                            var b = btn.DeepCopy();
                            b.VariableName = node["VariableName"] + "";
                            b.NodeId = parsedNode.Id;
                            ButtonActionHelper.ClearSavedValue(b.RepeatAs);
                            ButtonActionHelper.HandleSaveTextInput(b.RepeatAs, repeatOn[i] + "");
                            b.ButtonName = VerbProcessor.Process(b.ButtonName, false);
                            b.ButtonText = VerbProcessor.Process(b.ButtonText, false);
                            ButtonActionHelper.ClearSavedValue(b.RepeatAs);
                            CurrentClickButtons.Add(b);
                        }
                    }
                }
                else
                {
                    btn.VariableName = node["VariableName"] + "";
                    btn.NodeId = parsedNode.Id;
                    btn.ButtonName = VerbProcessor.Process(btn.ButtonName);
                    btn.ButtonText = VerbProcessor.Process(btn.ButtonText);
                    CurrentClickButtons.Add(btn);
                }
            }

            foreach (var btn in allButtons.Where(x => x.Kind == ButtonKind.TextInput))
            {
                btn.VariableName = node["VariableName"] + "";
                btn.NodeId = parsedNode.Id;
                btn.ButtonName = VerbProcessor.Process(btn.ButtonName);
                btn.ButtonText = VerbProcessor.Process(btn.ButtonText);
                try
                {
                    if (btn.ButtonType == ButtonTypeEnum.GetItemFromSource)
                    {
                        btn.ItemsSource = (await APIHelper.HitAsync<Dictionary<string, string>>(btn.Url));
                        btn.Items = btn.ItemsSource;
                    }
                }
                catch
                {
#if DEBUG
                    if (Debugger.IsAttached)
                    {
                        btn.ItemsSource = JsonConvert.DeserializeObject<Dictionary<string, string>>("{\r\n    \"ADVERTISING & MARKETING SERVICES\": \"Advertising & Marketing Services\",\r\n    \"AEROSPACE\": \"Aerospace\",\r\n    \"ARCHITECTURE\": \"Architecture\",\r\n    \"ARTS & CRAFTS\": \"Arts & Crafts\",\r\n    \"AUTOMOTIVE\": \"Automotive\",\r\n    \"BANK\": \"Bank\",\r\n    \"BIOTECHNOLOGY\": \"Biotechnology\",\r\n    \"BOOK STORE\": \"Book Store\",\r\n    \"BLOGS\": \"Blogs\",\r\n    \"BUSINESS SERVICES\": \"Business Services\",\r\n    \"CARS\": \"Cars\",\r\n    \"USED CARS\": \"Used Cars\",\r\n    \"CARGO & LOGISTICS\": \"Cargo & Logistics\",\r\n    \"CATERING\": \"Catering\",\r\n    \"CHEMICALS\": \"Chemicals\",\r\n    \"COLLEGE\": \"College\",\r\n    \"COMMUNITY\": \"Community\",\r\n    \"COMPUTERS\": \"Computers\",\r\n    \"CONSTRUCTION MATERIAL\": \"Construction Material\",\r\n    \"CONSULTANTS\": \"Consultants\",\r\n    \"EDUCATION\": \"Education\",\r\n    \"ELECTRONICS\": \"Electronics\",\r\n    \"ENERGY\": \"Energy\",\r\n    \"ENTERTAINMENT\": \"Entertainment\",\r\n    \"EQUIPMENT\": \"Equipment\",\r\n    \"EVENT PLANNING SERVICES\": \"Event Planning Services\",\r\n    \"EVENTS\": \"Events\",\r\n    \"EXPORTS\": \"Exports\",\r\n    \"FASHION - APPAREL\": \"Fashion - Apparel\",\r\n    \"FASHION - FOOTWEAR\": \"Fashion - Footwear\",\r\n    \"F&B - BAKERY\": \"F&B - Bakery\",\r\n    \"F&B - BARS\": \"F&B - Bars\",\r\n    \"F&B - CAFE\": \"F&B - Cafe\",\r\n    \"F&B - RESTAURANTS\": \"F&B - Restaurants\",\r\n    \"FREELANCER\": \"Freelancer\",\r\n    \"FLOWER SHOP\": \"Flower Shop\",\r\n    \"FINANCIAL SERVICES\": \"Financial Services\",\r\n    \"FURNITURE\": \"Furniture\",\r\n    \"GENERAL SERVICES\": \"General Services\",\r\n    \"GIFTS & NOVELTIES\": \"Gifts & Novelties\",\r\n    \"GROCERY\": \"Grocery\",\r\n    \"GAMING\": \"Gaming\",\r\n    \"GARDEN\": \"Garden\",\r\n    \"HOME FURNISHINGS\": \"Home Furnishings\",\r\n    \"HEALTH & FITNESS\": \"Health & Fitness\",\r\n    \"HOME APPLIANCES\": \"Home Appliances\",\r\n    \"HARDWARE & SANITARY-WARE\": \"Hardware & Sanitary-Ware\",\r\n    \"HOME MAINTENANCE\": \"Home Maintenance\",\r\n    \"HOME CARE\": \"Home Care\",\r\n    \"HOTEL & MOTELS\": \"Hotel & Motels\",\r\n    \"HOSPITAL\": \"Hospital\",\r\n    \"HYPERMARKET\": \"Hypermarket\",\r\n    \"INSURANCE\": \"Insurance\",\r\n    \"INTERIOR DESIGN\": \"Interior Design\",\r\n    \"KINDER GARTEN\": \"Kinder Garten\",\r\n    \"KIDS GOODS\": \"Kids Goods\",\r\n    \"KITCHEN EQUIPMENT\": \"Kitchen Equipment\",\r\n    \"LEGAL SERVICES\": \"Legal Services\",\r\n    \"MANUFACTURERS\": \"Manufacturers\",\r\n    \"MEDICAL - DENTAL\": \"Medical - Dental\",\r\n    \"MEDICAL - GENERAL\": \"Medical - General\",\r\n    \"MEDIA & NEWS\": \"Media & News\",\r\n    \"MINING\": \"Mining\",\r\n    \"NATURAL & AYURVEDA\": \"Natural & Ayurveda\",\r\n    \"NON-PROFIT ORGANIZATION\": \"Non-Profit Organization\",\r\n    \"OFFICE SUPPLIES\": \"Office Supplies\",\r\n    \"OTHER RETAIL\": \"Other Retail\",\r\n    \"OUTDOOR & SPORTING GOODS\": \"Outdoor & Sporting Goods\",\r\n    \"PETS\": \"Pets\",\r\n    \"PHOTOGRAPHY\": \"Photography\",\r\n    \"POLITICAL ORGANIZATION\": \"Political Organization\",\r\n    \"REAL ESTATE & CONSTRUCTION\": \"Real Estate & Construction\",\r\n    \"RELIGIOUS ORGANIZATION\": \"Religious Organization\",\r\n    \"SPA\": \"Spa\",\r\n    \"SPORTS\": \"Sports\",\r\n    \"SHOPPING COMPLEX\": \"Shopping Complex\",\r\n    \"SOFTWARE\": \"Software\",\r\n    \"SCIENCE & ENGINEERING\": \"Science & Engineering\",\r\n    \"SECURITY\": \"Security\",\r\n    \"TELECOMMUNICATION\": \"Telecommunication\",\r\n    \"TOURISM\": \"Tourism\",\r\n    \"TRADING\": \"Trading\",\r\n    \"TRANSPORTATION SERVICE\": \"Transportation Service\",\r\n    \"TRAINING INSTITUTES\": \"Training Institutes\",\r\n    \"TUITIONS & COACHING\": \"Tuitions & Coaching\",\r\n    \"WATCHES\": \"Watches\",\r\n    \"WATCHES & JEWELRY\": \"Watches & Jewelry\"\r\n}");
                        btn.Items = btn.ItemsSource;
                    }
#endif
                }
                CurrentTextInputButtons.Add(btn);
            }
            //Handling node timeout to default button
            var defaultBtn = allButtons.FirstOrDefault(x => x.DefaultButton && !Utils.IGNORED_DEFAULT_BUTTONS.Contains(x.ButtonType));
            if (defaultBtn != null && parsedNode.TimeoutInMs >= 0)
            {
                buttonTimeoutTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(parsedNode.TimeoutInMs)
                };
                buttonTimeoutTimer.Tick += (s, e) =>
                {
                    //ClearButtons();
                    ClearButtonTimer();
                    defaultBtn.Action.Execute(defaultBtn);
                };
                buttonTimeoutTimer.Start();
            }
        }
        public void ToggleTyping(bool show)
        {
            var incommingChat = ChatThread.Where(x => x.Direction == MessageDirection.In);
            var alreadyVisible = incommingChat.Count() > 0 && incommingChat.Last().SectionType == SectionTypeEnum.Typing;
            if (!alreadyVisible && show)
                ChatThread.Add(new Section() { SectionType = SectionTypeEnum.Typing, _id = Guid.NewGuid().ToString() });
            if (alreadyVisible && !show)
                ChatThread.Remove(incommingChat.Last());
        }
        public void AddOutgoingSection(Section sec)
        {
            if (sec == null) return;
            sec.Direction = MessageDirection.Out;
            sec.DelayInMs = 0;
            sec.Hidden = false;
            if (sec is TextSection textSec)
                textSec.Text = VerbProcessor.Process(textSec.Text);
            sec.Sno = (ChatThread.Count + 1);
            ChatThread.Add(sec);

            RemoveOldCenterSectionsFromThread();
        }
        public void AddIncommingSection(Section sec)
        {
            if (sec == null) return;

            sec.Direction = MessageDirection.In;
            if (sec is TextSection textSec)
                textSec.Text = VerbProcessor.Process(textSec.Text);

            sec.Sno = (ChatThread.Count + 1);
            ChatThread.Add(sec);

            RemoveOldCenterSectionsFromThread();
        }
        public void AddCenterSection(Section sec)
        {
            if (sec == null) return;

            sec.Direction = MessageDirection.AwkwardCenter;
            if (sec is TextSection textSec)
                textSec.Text = VerbProcessor.Process(textSec.Text);

            sec.Sno = (ChatThread.Count + 1);
            ChatThread.Add(sec);

            RemoveOldCenterSectionsFromThread();
        }
        public void RemoveOldCenterSectionsFromThread()
        {
            var sectionsToRemove = ChatThread.Where(x => x.Direction == MessageDirection.AwkwardCenter && ChatThread.LastOrDefault() != x).ToList();
            foreach (var section in sectionsToRemove)
                ChatThread.Remove(section);
        }

        public async Task<bool> PrecacheSection(Section sec)
        {
            try
            {
                if (sec is ImageSection imgSec)
                    await APIHelper.HitAsync(imgSec.Url); //Precache
                else if (sec is GifSection gifSec)
                    await APIHelper.HitAsync(gifSec.Url); //Precache
                return true;
            }
            catch { return false; }
        }
        public void NavigateToNode(string nextNodeId)
        {
            if (!string.IsNullOrWhiteSpace(nextNodeId))
                ProcessNode(GetNodeById(nextNodeId));
        }
        private string ExtractNextNodeIdFromAPIResp(ChatNode node, JObject resp)
        {
            if (node.Buttons == null)
                return null;
            return node.Buttons.FirstOrDefault(btn =>
                !string.IsNullOrWhiteSpace(btn.APIResponseMatchKey) && //resp[btn.APIResponseMatchKey] != null &&
                resp.SelectToken(btn.APIResponseMatchKey) + "" == btn.APIResponseMatchValue + "")
                    ?.NextNodeId;
        }
        #region Default
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {

            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void Reset() => StartChatting();

        public void ShowMemoryStack() => NavigationService.Navigate(typeof(Views.MemoryStackPage));

        public async void UpdateAPI()
        {
            Utils.APISettings.Values.TryGetValue("UploadFileAPI", out object UploadFileAPI);
            Utils.APISettings.Values.TryGetValue("ActivityTrackAPI", out object ActivityTrackAPI);
            Utils.APISettings.Values.TryGetValue("SocketServer", out object SocketServer);

            InputContentDialog icd = new InputContentDialog()
            {
                ChatFlowAPI = currentAPI,
                UploadFileAPI = UploadFileAPI + "",
                ActivityTrackAPI = ActivityTrackAPI + "",
                SocketServer = SocketServer + ""
            };

            icd.Closed += (s, e) =>
            {
                if (e.Result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    Utils.APISettings.Values["API"] = icd.ChatFlowAPI;
                    Utils.APISettings.Values["UploadFileAPI"] = icd.UploadFileAPI;
                    Utils.APISettings.Values["ActivityTrackAPI"] = icd.ActivityTrackAPI;
                    Utils.APISettings.Values["SocketServer"] = icd.SocketServer;

                    Reset();
                }
            };
            ToggleTyping(false);
            await icd.ShowAsync();
        }
        #endregion

        #region Agent Chat
        public void AgentChat()
        {
            var first = chatNodes.FirstOrDefault(x => x["Id"] + "" == "INIT_CHAT_NODE");
            if (first != null)
            {
                ProcessNode(first);
            }
            else
            {
                Utils.ShowDialog("Chat Init node not found");
            }
        }

        private Socket socket;
        public void SetupSocketConnection()
        {
            if (socket != null)
                socket.Close();
            Utils.APISettings.Values.TryGetValue("SocketServer", out object socketServer);
            if (string.IsNullOrWhiteSpace(socketServer + ""))
            {
                Utils.ShowDialog("Socket Server is not set. Please go to Menu(...) -> Update APIs and set it.");
                return;
            }
            socket = IO.Socket(socketServer + "", new IO.Options { Reconnection = true, AutoConnect = true });

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Debug.WriteLine("Connected");
                socket.Emit("join", Utils.DeviceId);
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                Debug.WriteLine("Disconnected");
            });

            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                var dataString = JsonConvert.SerializeObject(data, Formatting.Indented);
                Debug.WriteLine(dataString);

                Dispatcher.Dispatch(() =>
                {
                    if (data is JObject jData)
                    {
                        chatNodes.Add(jData);
                        ProcessNode(jData);
                    }
                });
            });
        }
        #endregion
    }
}