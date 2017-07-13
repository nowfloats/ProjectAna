using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Helpers;
using ANAConversationPlatform.Models;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ProjectController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> List()
        {
            var projs = await MongoHelper.GetProjectsAsync();
            if (projs != null)
                return Json(new { Message = "Projects list", Data = projs });
            return BadRequest("Unable to list the projects!");
        }

        [HttpPost]
        public async Task<ActionResult> Save([FromBody] ANAProject project)
        {
            var projs = await MongoHelper.SaveProjectAsync(project);
            if (projs != null)
                return Ok(new { Message = "Saved", Data = project });

            return BadRequest("Unable to save!");
        }
    }
}