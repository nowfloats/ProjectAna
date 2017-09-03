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
            return LatestLink(isSetup: false);
        }

        [HttpGet]
        public ActionResult LatestSetup()
        {
            return LatestLink(isSetup: true);
        }

        private ActionResult LatestLink(bool isSetup = false)
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
                        DownloadLink = Url.Link(nameof(StudioDownload), new { version = latest.ToString(), isSetup = isSetup }),
                        Version = latest
                    });
                }
            }
            return BadRequest(new { Message = "Studio Distribution Status Unknown" });
        }

        [HttpGet]
        [Route(nameof(StudioDownload), Name = nameof(StudioDownload))]
        public ActionResult StudioDownload(string version, bool isSetup = false)
        {
            try
            {
                var v = Version.Parse(version);
                var fileName = Path.Combine(Utils.Settings.StudioDistFolder, v.ToString() + (isSetup ? ".msi" : ".zip"));
                if (System.IO.File.Exists(fileName))
                    return File(System.IO.File.OpenRead(fileName), (isSetup ? "application/octet-stream" : "application/zip"), Path.GetFileName(fileName));
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