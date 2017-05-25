using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Helpers;
using System.Diagnostics;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ConversationController : Controller
    {
        [HttpGet]
        public ActionResult Chat()
        {
            //string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var chatNodes = MongoHelper.RetrieveRecordsFromChatNode();
            Debug.WriteLine(chatNodes.ToJson());
            if (chatNodes == null || chatNodes.Count == 0)
            {
                //TODO: Log API Response as Not Found
                //Task.Run(() => Logger.APILog("", ipAddress, requestTime, "Not Found", "Chat"));
                return Ok(new object[] { });
            }

            //TODO: Log API Response as Ok
            //Task.Run(() => Logger.APILog("", ipAddress, requestTime, "OK", "Chat"));

            return Json(chatNodes, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new CustomStringEnumConverter() }
            });
        }

        [HttpGet]
        public ActionResult RefreshContent()
        {
            MongoHelper.RefreshContentInMemory();
            return Ok(new { Message = "Done" });
        }
    }
}