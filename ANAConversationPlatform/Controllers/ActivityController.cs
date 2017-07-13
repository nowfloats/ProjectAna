using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Models.Activity;
using ANAConversationPlatform.Helpers;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ActivityController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Track([FromBody]ChatActivityEvent activityEvent)
        {
            await MongoHelper.InsertActivityEventAsync(activityEvent);
            return Ok();
        }
    }
}