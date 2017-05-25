using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Helpers;
using ANAConversationPlatform.Models.AgentChat;
using ANAConversationPlatform.Models.Sections;
using ANAConversationPlatform.Models;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;
using static ANAConversationPlatform.Helpers.Constants;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class AgentChatController : Controller
    {
        private readonly ILogger<AgentChatController> _log;

        public AgentChatController(ILogger<AgentChatController> log)
        {
            _log = log;
        }

        [HttpGet]
        public async Task<ActionResult> UserInput(string CHAT_USER_ID, string CHAT_USER_TOKEN, string TEXT, string AGENT)
        {
            var client = new RocketChatSDK(CHAT_USER_ID, CHAT_USER_TOKEN);
            await client.PostMessage(TEXT, "@" + AGENT);
            return Ok(new { });
        }

        [HttpPost]
        public async Task<ActionResult> SubmitHistory([FromBody]SubmitChatHistoryModel history)
        {
            if (string.IsNullOrWhiteSpace(history.CHAT_USER_ID) || string.IsNullOrWhiteSpace(history.CHAT_USER_TOKEN))
                return BadRequest("Empty chat user id or chat user token");
            if (history == null || history.HISTORY == null || history.HISTORY.Count <= 0)
                return Ok();

            try
            {
                var merchant = new RocketChatSDK(history.CHAT_USER_ID, history.CHAT_USER_TOKEN);
                var parsedHistory = history.HISTORY.ToObject<List<Section>>();

                foreach (var section in history.HISTORY.OrderBy(x => (int)x["Sno"]))
                {
                    PostMessageRequest req = null;
                    switch (section["SectionType"].ToString().ParseEnum<SectionTypeEnum>())
                    {
                        case SectionTypeEnum.Text:
                            var txtSection = section.ToObject<TextSection>();
                            if (!txtSection.Hidden)
                                req = new PostMessageRequest
                                {
                                    Text = txtSection.Text
                                };
                            break;
                        case SectionTypeEnum.Image:
                            var imgSection = section.ToObject<ImageSection>();
                            if (!imgSection.Hidden)
                                req = new PostMessageRequest
                                {
                                    Text = null,
                                    Attachments = new[]
                                    {
                                        new Attachment
                                        {
                                            Collapsed = false,
                                            ImageUrl = imgSection.Url,
                                            Title = imgSection.Title,
                                            Text = imgSection.Caption
                                        }
                                    }
                                };
                            break;
                        case SectionTypeEnum.Gif:
                            var gifSection = section.ToObject<GifSection>();

                            if (!gifSection.Hidden)
                                req = new PostMessageRequest
                                {
                                    Text = null,
                                    Attachments = new[]
                                    {
                                        new Attachment
                                        {
                                            Collapsed = false,
                                            ImageUrl = gifSection.Url,
                                            Title = gifSection.Title,
                                            Text = gifSection.Caption
                                        }
                                    }
                                };
                            break;
                        case SectionTypeEnum.Video:
                            var vidSection = section.ToObject<VideoSection>();
                            if (!vidSection.Hidden)
                                req = new PostMessageRequest
                                {
                                    Text = null,
                                    Attachments = new[]
                                    {
                                        new Attachment
                                        {
                                            Collapsed = false,
                                            VideoUrl = vidSection.Url,
                                            Title = vidSection.Title,
                                            Text = vidSection.Caption
                                        }
                                    }
                                };
                            break;
                        default:
                            req = new PostMessageRequest
                            {
                                Text = $"[Unsupported section type: {section["SectionType"]}]",
                            };
                            break;
                    }
                    if (section["Direction"].ToString().ParseEnum<MessageDirection>() == MessageDirection.Out)
                    {
                        req.Channel = "@" + history.AGENT;
                        await merchant.PostMessage(req);
                    }
                    else if (section["Direction"].ToString().ParseEnum<MessageDirection>() == MessageDirection.In)
                    {
                        var room = (await RocketChatSDK.Admin.GetInstantMessageThreads()).Ims.Where(x => x.Usernames != null).FirstOrDefault(x => x.Usernames.Contains(history.AGENT) && x.Usernames.Contains(history.DEVICE_ID));
                        if (room == null)
                        {
                            await merchant.PostMessage("----", "@" + history.AGENT);
                            room = (await RocketChatSDK.Admin.GetInstantMessageThreads()).Ims.Where(x => x.Usernames != null).FirstOrDefault(x => x.Usernames.Contains(history.AGENT) && x.Usernames.Contains(history.DEVICE_ID));
                        }
                        if (room != null)
                        {
                            req.Alias = "ANA Bunny";
                            req.RoomId = room.Id;
                            req.Avatar = $"{Request.Scheme}://{Request.Host}{(string.IsNullOrWhiteSpace(Request.PathBase.ToString()) ? "" : $"/{Request.PathBase}")}/favicon.ico";
                            await RocketChatSDK.Admin.AdminPostMessage(req);
                        }
                        else
                            _log.LogError("Room not found. Agent: {0}", history.AGENT);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId((int)LoggerEventId.AGENT_CHAT_SUBMIT_HISTORY_ERR), ex, "Submit History Exception: {0}", ex.Message);
            }

            return Ok(new { });
        }

        [HttpGet]
        public async Task<ActionResult> CreateUserSessionForHChat(string DEVICE_ID, string PERSON_NAME)
        {
            try
            {
                var userLogin = await RocketChatSDK.Admin.CreateUserIfNotExists(PERSON_NAME, DEVICE_ID);
                var resp = new
                {
                    CHAT_USER_ID = userLogin.UserId,
                    CHAT_USER_TOKEN = userLogin.Token,
                    AGENT = await RocketChatSDK.Admin.FindAgentForNewChat()
                };
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId((int)LoggerEventId.AGENT_CHAT_CREATE_USER_ERR), ex, "Create User Session For HChat: {0}", ex.Message);
                return Ok(new { });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CallbackReceivedFromRocketChatServerAsync([FromBody]RocketChatCallback info)
        {
            try
            {
                if (info.UserName == RocketChatSDK.Settings.APIUserName) return Ok(); //To skip outgoing msgs from device to be sent to the device back again

                var chatNode = new ChatNode("HUMAN_CHAT_NODE_" + ObjectId.GenerateNewId().ToString())
                {
                    Buttons = null, //If buttons are sent null, existing buttons should remain in the client
                    Emotion = EmotionEnum.Cool,
                    Name = "HumanChatNode",
                    NodeType = NodeTypeEnum.Combination,
                    Sections = new List<Section>(new[] { new TextSection(ObjectId.GenerateNewId().ToString(), 0, info.Text) }),
                    TimeoutInMs = 0,
                    VariableName = "TEXT"
                };

                var otherUserName = await RocketChatSDK.Admin.GetInstantMessageInfo(info.ChannelId);
                var targetUser = otherUserName.Usernames.First(x => x != info.UserName && x != RocketChatSDK.Settings.APIUserName);
                await LiveClientSocketsHelper.PushToDeviceAsync(targetUser, chatNode);
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId((int)LoggerEventId.AGENT_CHAT_CALLBACK_RECEIVED_ERR), ex, "CallbackReceivedFromRocketChatServerAsync: {0}", ex.Message);
            }
            return Ok();
        }
    }
}