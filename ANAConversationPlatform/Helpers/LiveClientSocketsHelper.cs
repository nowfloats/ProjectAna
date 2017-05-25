using ANAConversationPlatform.Models.AgentChat;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Helpers
{
    public static class LiveClientSocketsHelper
    {
        const string API_PUSH = "push";
        const string API_ASK_HISTORY = "ask_history";

        public static LiveClientSocketsServerSettings Settings { get; set; }

        public static async Task<int> PushToDeviceAsync<T>(string userId, T msg)
        {
            using (var hc = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Post, Settings.Server + API_PUSH)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        Name = userId,
                        Message = msg
                    }), System.Text.Encoding.UTF8, "application/json")
                };
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Settings.AuthBasic);
                var resp = await hc.SendAsync(req);
                return (int)resp.StatusCode;
            }
        }

        public static async Task<int> AskChatHistoryFromDevice(string userId)
        {
            using (var hc = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Get, $"{Settings.Server + API_ASK_HISTORY}?name=" + Uri.EscapeDataString(userId));
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Settings.AuthBasic);
                var resp = await hc.SendAsync(req);
                return (int)resp.StatusCode;
            };
        }
    }
}
