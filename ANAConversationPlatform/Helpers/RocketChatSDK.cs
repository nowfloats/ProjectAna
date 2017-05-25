using ANAConversationPlatform.Models.AgentChat;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using static ANAConversationPlatform.Helpers.Constants;

namespace ANAConversationPlatform.Helpers
{
    public class RocketChatSDK
    {
        private const string APICreateUser = "api/v1/users.create";
        private const string APIListUser = "api/v1/users.list";
        private const string APIListEveryoneInstantMsg = "api/v1/im.list.everyone?count=0";
        private const string APIPostMessage = "api/v1/chat.postMessage";
        private const string APILogin = "api/v1/login";

        public static ILogger<RocketChatSDK> Logger { get; set; }
        public static AgentChatSettings Settings { get; set; }
        public static RocketChatSDK Admin { get; set; }

        private string UserId { get; set; }
        private string Token { get; set; }

        public RocketChatSDK(string userId, string token)
        {
            Token = token;
            UserId = userId;
        }

        public static async Task<LoginResponse> Login(string userName, string password)
        {
            return await HitPostStringNoAuth<LoginResponse>(APILogin, "user=" + Uri.EscapeDataString(userName) + "&password=" + Uri.EscapeDataString(password));
        }

        public async Task<Im> GetInstantMessageInfo(string imRoomId)
        {
            var allIms = await HitGet<IMListResponse>(APIListEveryoneInstantMsg);
            return allIms.Ims.FirstOrDefault(x => x.Id == imRoomId);
        }

        public async Task<IMListResponse> GetInstantMessageThreads()
        {
            return await HitGet<IMListResponse>(APIListEveryoneInstantMsg);
        }

        public async Task<ListUserResponse> ListUsers()
        {
            return await HitGet<ListUserResponse>(APIListUser);
        }

        public async Task<PostMessageResponse> AdminPostMessage(PostMessageRequest req)
        {
            return await HitPost<PostMessageRequest, PostMessageResponse>(APIPostMessage, req);
        }

        public async Task<string> FindAgentForNewChat()
        {
            var activeUser = Settings.DefaultAgentUserName;
            try
            {
                var allUsers = await ListUsers();
                var onlineUsers = allUsers.Users.Where(x => x.Status == "online" && !x.Roles.Contains("guest") && x.Roles.Contains("user"));
                var userMsgGroup = onlineUsers.ToDictionary(x => x.Username, x => 0);
                var ims = await GetInstantMessageThreads();

                var latestIMThreads = ims.Ims.Where(x => DateTime.Parse(x.UpdatedAt) >= DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(2)));
                foreach (var item in latestIMThreads)
                    foreach (var userName in item.Usernames)
                        if (userMsgGroup.ContainsKey(userName))
                            userMsgGroup[userName]++;

                if (userMsgGroup.Count > 0)
                    activeUser = userMsgGroup.OrderBy(x => x.Value).First().Key;
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.AGENT_CHAT_FIND_AGENT_ERR), ex, "Find Agent For New Chat: {0}", ex.Message);
            }
            return activeUser;
        }

        public async Task<(string Token, string UserId)> CreateUserIfNotExists(string name, string userName)
        {
            var password = $"{userName}@1#A";

            try
            {
                var loginResp = await Login(userName, password);
                if (loginResp != null && loginResp.Status != null && loginResp.Status.ToString() == "success")
                    return (loginResp.Data.AuthToken, loginResp.Data.UserId);
            }
            catch (Exception ex) //Login Failed, Probaly due to no user exists with the given credentials.
            {
                Logger.LogError(new EventId((int)LoggerEventId.AGENT_CHAT_CREATE_USER), ex, "Login Attempt Failed: {0}", ex.Message);
            }

            if (string.IsNullOrWhiteSpace(name))
                name = "Unnamed User";

            var create = new CreateUserRequest
            {
                Active = true,
                Verified = false,
                SendWelcomeEmail = false,
                Email = $"{userName}@email.com", //TODO: hardcoded currently, remove hardcode. Handle existing user with given email 
                Username = userName,
                Name = name,
                JoinDefaultChannels = false,
                Password = password,
                RequirePasswordChange = false,
                Roles = new[] { "guest", "user" } //This indicates the user as a guest but has permissions of a normal user!
            };

            var createdUser = await HitPost<CreateUserRequest, UserResponse>(APICreateUser, create);

            var loginTry2 = await HitPostStringNoAuth<LoginResponse>(APILogin, "user=" + Uri.EscapeDataString(userName) + "&password=" + Uri.EscapeDataString(password));
            return (loginTry2.Data.AuthToken, loginTry2.Data.UserId);
        }

        public async Task<PostMessageResponse> PostMessage(string msg, string channel)
        {
            var req = new PostMessageRequest()
            {
                Channel = channel,
                Text = msg
            };
            return await HitPost<PostMessageRequest, PostMessageResponse>(APIPostMessage, req);
        }

        public async Task<PostMessageResponse> PostMessage(PostMessageRequest req)
        {
            return await HitPost<PostMessageRequest, PostMessageResponse>(APIPostMessage, req);
        }

        #region Helpers
        private async Task<TResponse> HitGet<TResponse>(string api)
        {
            using (var hc = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Get, Settings.APIBase + api);
                req.Headers.Accept.Clear();
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                req.Headers.Add("X-Auth-Token", Token);
                req.Headers.Add("X-User-Id", UserId);
                var resp = await hc.SendAsync(req);
                resp.EnsureSuccessStatusCode();
                var respJson = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(respJson);
            }
        }

        private async Task<TResponse> HitPost<TRequest, TResponse>(string api, TRequest data)
        {
            using (var hc = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Post, Settings.APIBase + api);
                req.Headers.Accept.Clear();
                req.Headers.Add("X-Auth-Token", Token);
                req.Headers.Add("X-User-Id", UserId);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                req.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), System.Text.Encoding.UTF8, "application/json");
                var resp = await hc.SendAsync(req);
                resp.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<TResponse>(await resp.Content.ReadAsStringAsync());
            }
        }

        private static async Task<TResponse> HitPostStringNoAuth<TResponse>(string api, string data)
        {
            try
            {
                using (var hc = new HttpClient())
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, Settings.APIBase + api);
                    req.Headers.Accept.Clear();
                    req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    req.Content = new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                    var resp = await hc.SendAsync(req);
                    resp.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<TResponse>(await resp.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                return default(TResponse);
            }
        }

        #endregion
    }
}
