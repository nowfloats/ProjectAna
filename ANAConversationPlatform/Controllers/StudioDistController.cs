using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ANAConversationPlatform.Helpers;
using System.IO;

namespace ANAConversationPlatform.Controllers
{
    [Produces("application/json")]
    public class StudioDistController : Controller
    {
        [HttpGet]
        public ActionResult Latest()
        {
            if (!string.IsNullOrWhiteSpace(Utils.Settings?.StudioDistFolder))
            {
                Version v;
                var fileVersions = Directory.EnumerateFiles(Utils.Settings?.StudioDistFolder)
                    .Where(x => Version.TryParse(Path.GetFileNameWithoutExtension(x), out v))
                    .Select(x => Version.Parse(Path.GetFileNameWithoutExtension(x))).ToList();

                if (fileVersions.Count > 0)
                {
                    var latest = fileVersions.OrderByDescending(x => x).First();
                    return Ok(new
                    {
                        DownloadLink = Url.Link(nameof(StudioDownload), new { version = latest.ToString() }),
                        Version = latest
                    });
                }
            }
            return BadRequest("Studio Distribution Status Unknown");
        }

        [HttpGet]
        [Route(nameof(StudioDownload), Name = nameof(StudioDownload))]
        public ActionResult StudioDownload(string version)
        {
            try
            {
                var v = Version.Parse(version);
                var fileName = Path.Combine(Utils.Settings.StudioDistFolder, v.ToString() + ".zip");
                if (System.IO.File.Exists(fileName))
                    return File(System.IO.File.OpenRead(fileName), "application/zip", Path.GetFileName(fileName));
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}