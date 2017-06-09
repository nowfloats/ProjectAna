using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Helpers;
using System.Diagnostics;
using MongoDB.Bson;
using Newtonsoft.Json;
using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using Microsoft.Extensions.Logging;
using static ANAConversationPlatform.Helpers.Constants;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ConversationController : Controller
    {
        private readonly ILogger<ConversationController> _log;

        public ConversationController(ILogger<ConversationController> log)
        {
            _log = log;
        }

        [HttpGet]
        public ActionResult Chat()
        {
            try
            {
                var chatNodes = MongoHelper.RetrieveRecordsFromChatNode();
                if (chatNodes == null || chatNodes.Count == 0)
                    return Ok(new object[] { });

                return Json(chatNodes, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter> { new CustomStringEnumConverter() }
                });
            }
            catch (System.Exception ex)
            {
                _log.LogError(new EventId((int)LoggerEventId.CHAT_ACTION_ERROR), ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult RefreshContent()
        {
            MongoHelper.RefreshContentInMemory();
            return Ok(new { Message = "Done" });
        }

        [HttpGet]
        public ActionResult HybridChat()
        {
            try
            {
                var chatNodes = MongoHelper.RetrieveRecordsFromChatNode();

                chatNodes.AddRange(new[] {
                new ChatNode("INIT_CHAT_NODE")
                {
                    ApiMethod = "GET",
                    ApiUrl = Url.Action("CreateUserSessionForHChat", "AgentChat", new { }, Request.Scheme),
                    Emotion = EmotionEnum.Cool,
                    Name = "Send Chat Text Node",
                    NodeType = NodeTypeEnum.ApiCall,
                    NextNodeId = "SEND_CHAT_HISTORY_TO_SERVER",
                    RequiredVariables = new[] { "DEVICE_ID", "PERSON_NAME" },
                },
                new ChatNode("SEND_CHAT_HISTORY_TO_SERVER")
                {
                    ApiMethod = "POST",
                    ApiUrl = Url.Action("SubmitHistory", "AgentChat", new { }, Request.Scheme),
                    Emotion = EmotionEnum.Cool,
                    Name = "Send Chat History To Server",
                    NodeType = NodeTypeEnum.ApiCall,
                    NextNodeId = "GET_CHAT_TEXT_NODE",
                    RequiredVariables = new[] { "CHAT_USER_ID", "CHAT_USER_TOKEN", "DEVICE_ID", "AGENT", "HISTORY" },
                },
                new ChatNode("GET_CHAT_TEXT_NODE")
                {
                    Buttons = new List<Button>(new[]{
                        new Button("NEW BTN ID", "ChatText", "Send", EmotionEnum.Cool, ButtonTypeEnum.GetText, "SEND_CHAT_TEXT_NODE", false, false)
                    }),
                    Emotion = EmotionEnum.Cool,
                    Name = "Chat Input",
                    NodeType = NodeTypeEnum.Combination,
                    Sections = new List<Section>(),
                    TimeoutInMs = 0,
                    VariableName = "TEXT"
                },
                new ChatNode("SEND_CHAT_TEXT_NODE")
                {
                    ApiMethod = "GET",
                    ApiUrl = Url.Action("UserInput", "AgentChat", new { }, Request.Scheme),
                    Buttons = null,
                    Emotion = EmotionEnum.Cool,
                    Name = "Send Chat Text Node",
                    NodeType = NodeTypeEnum.ApiCall,
                    NextNodeId = "CONTINUE_CHAT_NODE",
                    RequiredVariables = new[] { "CHAT_USER_ID", "CHAT_USER_TOKEN", "AGENT", "TEXT" },
                },
                new ChatNode("CONTINUE_CHAT_NODE")
                {
                    Buttons = new List<Button>(new[]{
                        new Button("NEW BTN ID", "ChatText", "Send", EmotionEnum.Cool, ButtonTypeEnum.GetText, "SEND_CHAT_TEXT_NODE", false, false)
                    }),
                    Emotion = EmotionEnum.Cool,
                    Name = "Chat Input Continue",
                    NodeType = NodeTypeEnum.Combination,
                    Sections = new List<Section>(),
                    TimeoutInMs = 0,
                    VariableName = "TEXT"
                }
            });

                if (chatNodes == null || chatNodes.Count == 0)
                    return Ok(new object[] { });

                return Json(chatNodes, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter> { new CustomStringEnumConverter() }
                });
            }
            catch (System.Exception ex)
            {
                _log.LogError(new EventId((int)LoggerEventId.HYBRID_CHAT_ACTION_ERROR), ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}