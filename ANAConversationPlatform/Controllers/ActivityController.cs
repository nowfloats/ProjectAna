using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Models.Activity;
using ANAConversationPlatform.Helpers;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ActivityController : Controller
    {
        [HttpGet]
        public ActionResult Summary(string nodeIds)
        {
            var nodeIdList = nodeIds.Split(',');

            return Ok(new { Message = "Done" });
        }

        [HttpPost]
        public ActionResult Track([FromBody]ChatActivityEvent activityEvent)
        {
            MongoHelper.InsertActivityEvent(activityEvent);
            return Ok();
        }
    }
}