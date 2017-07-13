using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Helpers;
using ANAConversationPlatform.Models;
using System.Collections.Generic;

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
            return BadRequest(new { Message = "Unable to list the projects!" });
        }

        [HttpPost]
        public ActionResult Save([FromBody] List<ANAProject> projects)
        {
            var projs = MongoHelper.SaveProjects(projects);
            if (projs != null)
                return Ok(new { Message = "Saved", Data = projects });

            return BadRequest(new { Message = "Unable to save!" });
        }
    }
}