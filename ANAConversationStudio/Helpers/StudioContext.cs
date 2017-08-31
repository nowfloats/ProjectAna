using ANAConversationStudio.Models.Chat;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ANAConversationStudio.Helpers
{
    public class StudioContext : INotifyPropertyChanged
    {
        #region Private
        private const string LoadProjectsAPI = "api/Project/List";
        private const string SaveProjectsAPI = "api/Project/Save";

        private const string SaveChatFlowPackAPI = "api/Conversation/SaveChatFlow";
        private const string FetchChatFlowPackAPI = "api/Conversation/FetchChatFlow?projectId={projectId}";
        private const string ReceiveFileAPI = "api/Services/ReceiveFile?fileName={fileName}";

        private StudioContext() { }
        private StudioContext(ChatServerConnection conn) { ChatServer = conn; }

        private async Task<T> Hit<T>(string api, bool strictTypeNames = false) where T : APIResponse
        {
            try
            {
                if (api == null) return null;

                api = ChatServer.ServerUrl?.TrimEnd('/') + "/" + api;
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ChatServer.APIKey}:{ChatServer.APISecret}"));
                    T resp = default(T);
                    if (strictTypeNames)
                        resp = BsonSerializer.Deserialize<T>(await wc.DownloadStringTaskAsync(api));
                    else
                    {
                        wc.Headers[HttpRequestHeader.Accept] = "application/json";
                        resp = JsonConvert.DeserializeObject<T>(await wc.DownloadStringTaskAsync(api));
                    }
                    resp.Status = true;
                    return resp;
                }
            }
            catch (WebException ex)
            {
                using (var resp = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var respParsed = JsonConvert.DeserializeObject<T>(await resp.ReadToEndAsync());
                    if (respParsed == default(T))
                    {
                        respParsed = Activator.CreateInstance<T>();
                        respParsed.Message = ex.Message;
                    }
                    respParsed.Status = false;
                    return respParsed;
                }
            }
            catch (Exception ex)
            {
                var respParsed = Activator.CreateInstance<T>();
                respParsed.Message = ex.Message;
                respParsed.Status = false;
                return respParsed;
            }
        }
        private async Task<T> HitPost<T, TReq>(string api, TReq requestData, bool strictTypeNames = false) where T : APIResponse
        {
            try
            {
                if (api == null) return null;

                api = ChatServer.ServerUrl?.TrimEnd('/') + "/" + api;
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ChatServer.APIKey}:{ChatServer.APISecret}"));
                    T resp = default(T);
                    if (strictTypeNames)
                    {
                        resp = BsonSerializer.Deserialize<T>(await wc.UploadStringTaskAsync(api, requestData.ToJson()));
                    }
                    else
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        wc.Headers[HttpRequestHeader.Accept] = "application/json";
                        resp = JsonConvert.DeserializeObject<T>(await wc.UploadStringTaskAsync(api, JsonConvert.SerializeObject(requestData)));
                    }
                    resp.Status = true;
                    return resp;
                }
            }
            catch (WebException ex)
            {
                using (var resp = new StreamReader(ex.Response.GetResponseStream()))
                {
                    T respParsed = Activator.CreateInstance<T>();
                    if (strictTypeNames)
                        respParsed.Message = await resp.ReadToEndAsync();
                    else
                        respParsed = JsonConvert.DeserializeObject<T>(await resp.ReadToEndAsync());
                    respParsed.Status = false;
                    return respParsed;
                }
            }
            catch (Exception ex)
            {
                T respParsed = Activator.CreateInstance<T>();
                respParsed.Status = false;
                respParsed.Message = ex.Message;
                return respParsed;
            }
        }
        private async Task<T> HitPostData<T>(string api, Stream dataStream) where T : APIResponse
        {
            try
            {
                if (api == null) return null;

                api = ChatServer.ServerUrl?.TrimEnd('/') + "/" + api;
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ChatServer.APIKey}:{ChatServer.APISecret}"));
                    wc.Headers[HttpRequestHeader.Accept] = "application/json";

                    byte[] reqData = new byte[dataStream.Length];
                    dataStream.Read(reqData, 0, reqData.Length);

                    var respBytes = await wc.UploadDataTaskAsync(api, reqData);
                    var resp = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(respBytes));
                    resp.Status = true;
                    return resp;
                }
            }
            catch (WebException ex)
            {
                using (var resp = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var respParsed = JsonConvert.DeserializeObject<T>(await resp.ReadToEndAsync());
                    if (respParsed == null)
                    {
                        respParsed = Activator.CreateInstance<T>();
                        respParsed.Message = ex.Message;
                    }
                    respParsed.Status = false;
                    return respParsed;
                }
            }
            catch (Exception ex)
            {
                T respParsed = Activator.CreateInstance<T>();
                respParsed.Status = false;
                respParsed.Message = ex.Message;
                return respParsed;
            }
        }

        private async Task<bool> LoadProjectsAsync()
        {
            var projsResp = await Hit<DataListResponse<ANAProject>>(LoadProjectsAPI);
            if (projsResp.Status)
            {
                ChatFlowProjects = new ObservableCollection<ANAProject>(projsResp.Data);
                try
                {
                    if (ChatFlowProjects == null || ChatFlowProjects.Count <= 0)
                    {
                        ChatFlowProjects = new ObservableCollection<ANAProject> { new ANAProject { Name = "My First ANA Project", _id = ObjectId.GenerateNewId().ToString() } };
                        await SaveProjectsAsync();
                    }
                }
                catch { }
                return true;
            }
            MessageBox.Show("Error: " + projsResp.Message, "Unable to load projects from chat server.");
            return false;
        }
        #endregion

        #region Static

        #region Static Property Changed
        public static event EventHandler StaticPropertyChanged;
        private static void OnStaticPropertyChanged(object sender) => StaticPropertyChanged?.Invoke(sender, new EventArgs());
        #endregion
        private static StudioContext _Current;
        public static StudioContext Current
        {
            get { return _Current; }
            private set
            {
                if (_Current != value)
                {
                    _Current = value;

                    OnStaticPropertyChanged(_Current);
                }
            }
        }

        public static async Task<bool> LoadFromChatServerConnectionAsync(ChatServerConnection conn)
        {
            var s = new StudioContext(conn);
            var done = await s.LoadProjectsAsync();
            if (done)
                Current = s;
            return done;
        }
        public static void ClearCurrent()
        {
            Current = null;
        }
        public static bool IsProjectLoaded(bool showMsg)
        {
            if (Current?.ChatServer?.ServerUrl == null)
            {
                if (showMsg)
                    MessageBox.Show("No chat flow is loaded. Please load a chat flow and try again.");
                return false;
            }
            return true;
        }

        public static string CurrentProjectUrl()
        {
            return Current?.ChatServer?.ServerUrl + "/api/Conversation/chat?projectId=" + CurrentProjectId();
        }

        public static string CurrentProjectId()
        {
            return Current?.ChatFlow?.ProjectId;
        }

        public static ANAProject CurrentProject()
        {
            return Current?.ChatFlowProjects?.FirstOrDefault(x => x._id == Current?.ChatFlow?.ProjectId);
        }

        #endregion

        #region Public
        private ChatFlowPack _ChatFlow;
        public ChatFlowPack ChatFlow
        {
            get { return _ChatFlow; }
            set
            {
                if (_ChatFlow != value)
                {
                    _ChatFlow = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChatServerConnection _ChatServer;
        public ChatServerConnection ChatServer
        {
            get { return _ChatServer; }
            set
            {
                if (_ChatServer != value)
                {
                    _ChatServer = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ANAProject> _ChatFlowProjects;
        public ObservableCollection<ANAProject> ChatFlowProjects
        {
            get { return _ChatFlowProjects; }
            set
            {
                if (_ChatFlowProjects != value)
                {
                    _ChatFlowProjects = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task<bool> LoadChatFlowAsync(string projectId)
        {
            if (ChatFlowProjects != null && ChatFlowProjects.Any(x => x._id == projectId))
            {
                var chatFlowResp = await Hit<DataResponse<ChatFlowPack>>(FetchChatFlowPackAPI.Replace("{projectId}", projectId), true);
                if (chatFlowResp.Status)
                {
                    ChatFlow = chatFlowResp.Data;
                    ChatFlowBuilder.Build(ChatFlow);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> SaveChatFlowAsync()
        {
            if (ChatFlow != null)
            {
                var saveChatFlowResp = await HitPost<DataResponse<ChatFlowPack>, ChatFlowPack>(SaveChatFlowPackAPI, ChatFlow, true);
                if (saveChatFlowResp.Status)
                {
                    ChatFlow = saveChatFlowResp.Data;
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> SaveProjectsAsync()
        {
            if (ChatFlowProjects != null)
            {
                var saveProjectsResp = await HitPost<DataListResponse<ANAProject>, List<ANAProject>>(SaveProjectsAPI, ChatFlowProjects.ToList());
                if (saveProjectsResp.Status)
                {
                    ChatFlowProjects = new ObservableCollection<ANAProject>(saveProjectsResp.Data);
                    return true;
                }
            }
            return false;
        }

        public async Task<UploadFileResponse> UploadFile(string srcFileFullPath)
        {
            var api = ReceiveFileAPI.Replace("{fileName}", Uri.EscapeDataString(Path.GetFileName(srcFileFullPath)));
            if (!File.Exists(srcFileFullPath))
                return null;
            using (var fs = File.OpenRead(srcFileFullPath))
                return await HitPostData<UploadFileResponse>(api, fs);
        }

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion 

        #endregion
    }

    public class APIResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }

    public class DataListResponse<T> : APIResponse
    {
        public List<T> Data { get; set; }
    }

    public class DataResponse<T> : APIResponse
    {
        public T Data { get; set; }
    }

    public class UploadFileResponse : APIResponse
    {
        public string Url { get; set; }
    }
}