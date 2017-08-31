using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ANAConversationPlatform.Helpers;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class ServicesController : Controller
    {
        private IHostingEnvironment _env;
        public ServicesController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public ActionResult ReceiveFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest(new { Message = "fileName cannot be empty" });

            fileName = Path.GetFileNameWithoutExtension(fileName) + "-" + Guid.NewGuid() + Path.GetExtension(fileName);

            using (var fileStream = Request.Body)
            {
                if (fileStream == null)
                    return BadRequest(new { Message = "Unable to read file content" });

                var fullFileName = Path.Combine(_env.WebRootPath, Constants.CHAT_MEDIA_FOLDER_NAME, fileName);
                var done = FileSaveHelper.Save(fullFileName, fileStream);
                if (done)
                    return Ok(new
                    {
                        Url = $"{Request.Scheme}://{Request.Host}" + Url.Content($"~/{Constants.CHAT_MEDIA_FOLDER_NAME}/{fileName}")
                    });
                else
                    return StatusCode(500, "Unable to save received file");
            }
        }
    }
}