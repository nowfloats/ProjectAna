using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Models.Activity;
using ANAConversationPlatform.Helpers;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ActivityController : Controller
    {
        [HttpPost]
        public ActionResult Track([FromBody]ChatActivityEvent activityEvent)
        {
            MongoHelper.InsertActivityEvent(activityEvent);
            return Ok();
        }
    }
}