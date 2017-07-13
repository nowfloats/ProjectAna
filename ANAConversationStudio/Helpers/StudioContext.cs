using ANAConversationStudio.Models.Chat;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ANAConversationStudio.Helpers
{
    public class StudioContext
    {
        public static StudioContext Current { get; set; }
        private StudioContext(ChatServerConnection conn) { ChatServer = conn; }

        public static async Task<bool> LoadFromChatServerConnectionAsync(ChatServerConnection conn)
        {
            var s = new StudioContext(conn);
            var done = await s.LoadProjectsAsync();
            if (done)
                Current = s;
            return done;
        }
        public ChatServerConnection ChatServer { get; set; }
        public List<ANAProject> ChatFlowProjects { get; set; }
        public ChatFlowPack ChatFlow { get; set; }

        private async Task<bool> LoadProjectsAsync()
        {
            var projsResp = await Hit<DataListResponse<ANAProject>>(LoadProjectsAPI);
            if (projsResp.Status)
            {
                ChatFlowProjects = projsResp.Data;
                return true;
            }
            MessageBox.Show("Error: " + projsResp.Message, "Unable to load projects from chat server.");
            return false;
        }

        public async Task<bool> LoadChatFlowAsync(string projectId)
        {
            if (ChatFlowProjects != null && ChatFlowProjects.Any(x => x._id == projectId))
            {
                var chatFlowResp = await Hit<DataResponse<ChatFlowPack>>(FetchChatFlowPackAPI.Replace("{projectId}", projectId), true);
                if (chatFlowResp.Status)
                {
                    ChatFlow = chatFlowResp.Data;
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
            if (ChatFlow != null)
            {
                var saveProjectsResp = await HitPost<DataListResponse<ANAProject>, List<ANAProject>>(SaveProjectsAPI, ChatFlowProjects);
                if (saveProjectsResp.Status)
                {
                    ChatFlowProjects = saveProjectsResp.Data;
                    return true;
                }
            }
            return false;
        }


        private const string LoadProjectsAPI = "api/Project/List";
        private const string SaveProjectsAPI = "api/Project/Save";

        private const string SaveChatFlowPackAPI = "api/Conversation/SaveChatFlow";
        private const string FetchChatFlowPackAPI = "api/Conversation/FetchChatFlow?projectId={projectId}";

        private async Task<T> Hit<T>(string api, bool strictTypeNames = false) where T : APIResponse
        {
            try
            {
                if (api == null) return null;

                api = ChatServer.ServerUrl?.TrimEnd('/') + "/" + api;
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ChatServer.AuthUser}:{ChatServer.AuthKey}"));
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
                    respParsed.Status = false;
                    return respParsed;
                }
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
                    wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ChatServer.AuthUser}:{ChatServer.AuthKey}"));
                    T resp = default(T);
                    if (strictTypeNames)
                    {
                        //wc.Headers[HttpRequestHeader.ContentType] = "text/plain";
                        //wc.Headers[HttpRequestHeader.Accept] = "text/plain";
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
}