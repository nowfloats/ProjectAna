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

namespace ANAConversationSimulator.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public static MainPageViewModel CurrentInstance { get; set; }
        public MainPageViewModel()
        {
            CurrentInstance = this;
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
        public JToken GetNodeById(string nodeId)
        {
            return chatNodes.Children().FirstOrDefault(x => x["Id"].ToString() == nodeId);
        }
        public async void StartChatting()
        {
            ClearChatThread();
            ToggleTyping(true);
            await LoadNodesAsync();
            if (chatNodes == null || chatNodes.Count == 0)
                return;

            ProcessNode(chatNodes[0]);
        }
        private DispatcherTimer buttonTimeoutTimer;
        public async void ProcessNode(JToken node, JToken section = null)
        {
            if (node == null) return;
            ClearButtons();
            //Replaceing verbs
            node = JToken.Parse(VerbProcessor.Process(node.ToString()));

            var parsedNode = node.ToObject<ChatNode>();
            if (parsedNode.NodeType == NodeTypeEnum.ApiCall)
            {
                ToggleTyping(true);
                try
                {
                    var paramDict = new Dictionary<string, object>();
                    foreach (var reqParam in parsedNode.RequiredVariables)
                        paramDict[reqParam] = ButtonActionHelper.GetSavedValue(reqParam);
                    var nextNodeId = parsedNode.NextNodeId; //Default
                    switch (parsedNode.ApiMethod.ToUpper())
                    {
                        case "GET":
                            {
                                var query = string.Join("&", paramDict.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value + "")}"));
                                var api = string.IsNullOrWhiteSpace(query) ? parsedNode.ApiUrl : parsedNode.ApiUrl + "?" + query;

                                var resp = await APIHelper.HitAsync<Dictionary<string, object>>(api);
                                if (resp.ContainsKey("NextNodeId"))
                                    nextNodeId = resp["NextNodeId"] + "";
                                ButtonActionHelper.HandleSaveMultiple(resp);
                            }
                            break;
                        case "POST":
                            {
                                var resp = await APIHelper.HitPostAsync<Dictionary<string, object>, Dictionary<string, object>>(parsedNode.ApiUrl, paramDict);
                                if (resp.ContainsKey("NextNodeId"))
                                    nextNodeId = resp["NextNodeId"] + "";
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
            else if (node["Sections"] != null && node["Sections"].Children().Count() > 0)
            {
                var sectionsSource = node["Sections"];
                var currentSectionSource = section ?? sectionsSource.First;

                //Replaceing verbs
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
                    case SectionTypeEnum.Link:
                    case SectionTypeEnum.Graph:
                    case SectionTypeEnum.Carousel:
                        Utils.ShowDialog($"{secType} Coming soon!");
                        break;
                    default:
                        break;
                }
                if (parsedSection != null)
                {
                    if (parsedSection.DelayInMs > 50 || showTyping) //Add 'typing' bubble if delay is grather than 50 ms
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
                                    try
                                    {
                                        await APIHelper.TrackEvent(Utils.GetViewEvent(parsedNode.Id, Utils.DeviceId));
                                    }
                                    catch (Exception ex)
                                    {
                                        await Utils.ShowDialogAsync(ex.ToString());
                                    }
                                });
                            }
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

        public void ClearButtons()
        {
            CurrentClickButtons.Clear();
            CurrentTextInputButtons.Clear();
            if (buttonTimeoutTimer != null)
            {
                buttonTimeoutTimer.Stop();
                buttonTimeoutTimer = null;
            }
        }
        public void ClearChatThread()
        {
            ChatThread.Clear();
            ClearButtons();
        }
        public async Task ProcessButtonsAsync(JToken node)
        {
            ClearButtons();
            var parsedNode = node.ToObject<ChatNode>();
            var allButtons = node["Buttons"].ToObject<List<Button>>();
            foreach (var btn in allButtons.Where(x => x.Kind == ButtonKind.ClickInput))
            {
                btn.VariableName = node["VariableName"] + "";
                btn.NodeId = parsedNode.Id;
                btn.ButtonName = VerbProcessor.Process(btn.ButtonName);
                btn.ButtonText = VerbProcessor.Process(btn.ButtonText);
                CurrentClickButtons.Add(btn);
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
                catch { }
                CurrentTextInputButtons.Add(btn);
            }
            //Handling node timeout to default button
            var defaultBtn = allButtons.FirstOrDefault(x => x.DefaultButton);
            if (defaultBtn != null && parsedNode.TimeoutInMs > 0)
            {
                buttonTimeoutTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(parsedNode.TimeoutInMs)
                };
                buttonTimeoutTimer.Tick += (s, e) =>
                {
                    ClearButtons();
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
            ChatThread.Add(sec);
        }
        public void AddIncommingSection(Section sec)
        {
            if (sec == null) return;

            sec.Direction = MessageDirection.In;
            if (sec is TextSection textSec)
                textSec.Text = VerbProcessor.Process(textSec.Text);

            ChatThread.Add(sec);
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

            InputContentDialog icd = new InputContentDialog()
            {
                ChatFlowAPI = currentAPI,
                UploadFileAPI = UploadFileAPI + "",
                ActivityTrackAPI = ActivityTrackAPI + ""
            };

            icd.Closed += (s, e) =>
            {
                if (e.Result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    Utils.APISettings.Values["API"] = icd.ChatFlowAPI;
                    Utils.APISettings.Values["UploadFileAPI"] = icd.UploadFileAPI;
                    Utils.APISettings.Values["ActivityTrackAPI"] = icd.ActivityTrackAPI;

                    Reset();
                }
            };
            ToggleTyping(false);
            await icd.ShowAsync();
        }
        #endregion
    }
}