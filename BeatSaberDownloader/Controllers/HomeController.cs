using Microsoft.AspNetCore.Mvc;

namespace BeatSaberDownloader.Server.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("robots.txt")]
        public IActionResult Get()
        {
            return File(System.IO.File.OpenRead("robots.txt"), "text/plain", "robots.txt");
        }
    }
}
